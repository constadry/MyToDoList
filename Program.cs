using System;
using System.Collections.Generic;

namespace MyToDoList1
{
    internal class StringParser
    {
        private List<string> words;
        public StringParser(string input)
        {
            words = new List<string>(input.Split());
        }

        public List<string> GetList()
        {
            return words;
        }

        public static bool Parseid(string str, out int ID)
        {
            bool isParsable = Int32.TryParse(str, out ID);

            return isParsable;
        }
    }

    internal class CommandExecutor
    {
        private List<string> tasks = new List<string>();
        private List<string> list = new List<string>();
        
        public void SetList(StringParser stringParser)
        {
            list = stringParser.GetList();
        }

        public void Execute()
        {
            if (list[0] == "/add")
            {
                if (list.Count == 2)
                {
                    AddTask();
                }
                else
                {
                    BadCommandFormat();
                }
            }
            
            else if (list[0] == "/all")
            {
                WriteAllTasks();
            }

            else if (list[0] == "/delete")
            {
                DeleteTask();
            }

            else
            {
                BadCommandFormat();
            }
        }

        private void AddTask()
        {
            tasks.Add(list[1]);
        }

        private void WriteAllTasks()
        {
            Console.WriteLine("Name of the task, id");
            for (int i = 0; i < tasks.Count; i++)
            {
                Console.WriteLine(tasks[i] + ", " + Convert.ToString(i));
            }
        }

        private void DeleteTask()
        {
            int ID = -1;
            if (StringParser.Parseid(list[1], out ID))
            {
                // Console.WriteLine(ID);
                tasks.RemoveAt(ID);
            }
            else
            {
                Console.WriteLine("Could note be parsed");
            }
        }
        
        private void BadCommandFormat()
        {
            Console.WriteLine("Please, enter correct command name");
        }
    }
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            string str;
            str = Console.ReadLine();
            StringParser stringParser = new StringParser(str);
            CommandExecutor commandExecutor = new CommandExecutor();
            while (str != "exit")
            {
                commandExecutor.SetList(stringParser);
                commandExecutor.Execute();
                str = Console.ReadLine();
                stringParser = new StringParser(str);
            }
        }
    }
}