using System;

namespace MyToDoList1
{
    internal static class Program
    {
        private static void AboutTime()
        {
            Console.WriteLine("Write /time id date, to add deadline to the task");
            Console.WriteLine("Date format: MM.dd.yyyy");
        }
        public static void Main()
        {
            AboutTime();
            var input = Console.ReadLine();
            var stringParser = new StringParser(input);
            var commandExecutor = new CommandExecutor();
            while (input != "/exit")
            {
                commandExecutor.SetList(stringParser);
                commandExecutor.Execute();
                input = Console.ReadLine();
                stringParser = new StringParser(input);
            }
        }
    }
}