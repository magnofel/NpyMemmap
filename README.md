# NpyMemmap
Numpy Memmap arrays wrapper for C#

Usage:

~~~~
double[] arr = {0.1, 0.2, 0.3};
//creating new memmap file
var memmap = new NpyMemmap<double>("test.mmap",3);
//writing double array to file
memmap.WriteToStream(arr);
//reading memmap by index
var val = memmap[0];
~~~~
