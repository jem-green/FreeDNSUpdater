using System;
using System.Text;

namespace FreeDnsConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            FreeDnsConsole freeDnsConsole = new FreeDnsConsole();
            freeDnsConsole.Start();
            Console.WriteLine("Press any key to stop the service.");
            Console.ReadKey();
            freeDnsConsole.Stop();
        }
    }
}
