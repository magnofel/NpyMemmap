using System;

namespace NpyMemmap
{
    class Program
    {
        static void Main(string[] args)
        {
            double[] arr = {0.1, 0.2, 0.3};

            //creating new memmap file
            var memmap = new NpyMemmap<double>("test.mmap",3);
            //writing double array to file
            memmap.WriteToStream(arr);
            
            //reading memmap
            Console.WriteLine(memmap[0]);
            memmap[0] = 0.4;
            Console.WriteLine(memmap[0]);
            Console.WriteLine(memmap[2]);
            Console.ReadKey();
            memmap.Dispose();
        }
    }
}
