using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

namespace NpyMemmap
{
    public class NpyMemmap<T> : IDisposable, IEnumerable<T> where T : struct
    {
        MemoryMappedFile mappedFile;
        MemoryMappedViewAccessor accesor;
        private MemoryMappedViewStream mappedViewStream;
        private BinaryWriter binaryStreamWriter;
        long elementSize;
        long numberOfElements;

        public NpyMemmap(string filePath, long numberElements)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException();
            }

            numberOfElements = numberElements;
            elementSize = Marshal.SizeOf(typeof(T));
            long length = numberOfElements * elementSize;
            if (!File.Exists(filePath))
            {
                mappedFile = MemoryMappedFile.CreateFromFile(filePath, FileMode.CreateNew, "mmmap.npy", length);
            }
            else
            {
                mappedFile = MemoryMappedFile.CreateFromFile(filePath);
            }

            mappedViewStream = mappedFile.CreateViewStream();
            binaryStreamWriter = new BinaryWriter(mappedViewStream);
            accesor = mappedFile.CreateViewAccessor(0, length);
        }

        public void WriteToStream(T[] values)
        {
            foreach (var val in values)
            {
                this.WriteToStream(val);
            }
        }

        public void WriteToStream(T value)
        {
            binaryStreamWriter.Write(BitConverter.GetBytes((dynamic)value));
        }

        public long Length
        {
            get { return numberOfElements; }
        }

        public T this[long index]
        {
            set
            {
                if (index < 0 || index > numberOfElements)
                {
                    throw new ArgumentOutOfRangeException();
                }

                accesor.Write<T>(index * elementSize, ref value);
            }

            get
            {
                if (index < 0 || index > numberOfElements)
                {
                    throw new ArgumentOutOfRangeException();
                }

                T value = default(T);
                accesor.Read<T>(index * elementSize, out value);
                return value;
            }
        }

        public void Dispose()
        {
            if (accesor != null)
            {
                accesor.Dispose();
                accesor = null;
            }

            if (mappedFile != null)
            {

                mappedViewStream.Flush();
                mappedViewStream.Dispose();
                mappedFile.Dispose();
                mappedFile = null;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            T value;
            for (int index = 0; index < numberOfElements; index++)
            {
                value = default(T);
                accesor.Read<T>(index * elementSize, out value);
                yield return value;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            T value;
            for (long index = 0; index < numberOfElements; index++)
            {
                value = default(T);
                accesor.Read<T>(index * elementSize, out value);
                yield return value;
            }
        }

        public T[] GetArray()
        {
            if (mappedFile==null)
            {
                throw new ArgumentNullException();
            }
            var elements = new T[numberOfElements];
            accesor.ReadArray<T>(0, elements, 0, (int)numberOfElements);
            return elements;
        }
    }
}