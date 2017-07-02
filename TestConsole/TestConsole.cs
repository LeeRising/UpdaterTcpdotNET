using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    internal class TestConsole
    {
        private static void Main(string[] args)
        {
            var dataToByte = Encoding.UTF8.GetBytes("1234567");

            var copiedBytes = dataToByte.Skip(0).Take(4).ToArray();

            Console.WriteLine(copiedBytes[0]);
            Console.ReadKey(true);
        }
    }
}
