using System;

namespace Ara {
    public static class Source {
        public static void Main(string[] args) {
            Console.WriteLine("Designed and Written by ____________");
            Console.WriteLine("                        NUREN SHAMS/");
            Console.WriteLine();
            //Code start

            Assembler.Assemble();

            //Code end
            Console.Write("\nPress any key to exit. . . ");
            Console.ReadKey(true);
        }
    }
}