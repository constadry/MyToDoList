using System;
using System.Collections.Generic;
using System.IO;

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
        private List<string> completed = new List<string>();
        
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
                if (list.Count == 1)
                {
                    WriteAllTasks();
                }
                else
                {
                    BadCommandFormat();
                }
            }

            else if (list[0] == "/delete")
            {
                if (list.Count == 2)
                {
                    DeleteTask();
                }
                else
                {
                    BadCommandFormat();
                }
            }
            
            else if (list[0] == "/load")
            {
                if (list.Count == 2)
                {
                    LoadTaskFromFile();
                }
                else
                {
                    BadCommandFormat();
                }
            }
            else if (list[0] == "/save")
            {
                if (list.Count == 2)
                {
                    SaveTaskToFile();
                }
                else
                {
                    BadCommandFormat();
                }
            }
            
            else if (list[0] == "/complete")
            {
                if (list.Count == 2)
                {
                    Complete();
                }
                else
                {
                    BadCommandFormat();
                }
            }
            
            else if (list[0] == "/time")
            {
                if (list.Count == 3)
                {
                    AddTimeLimitToTask();
                }
                else
                {
                    BadCommandFormat();
                }
            }
            
            else if (list[0] == "/today")
            {
                TasksForToday();
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
            Console.WriteLine("Completed task:");
            foreach (var task in completed)
            {
                Console.WriteLine(task);
            }
        }
        
        private void DeleteTask()
        {
            int ID = -1;
            if (StringParser.Parseid(list[1], out ID))
            {
                tasks.RemoveAt(ID);
            }
            else
            {
                Console.WriteLine("Could note be parsed");
            }
        }

        private void LoadTaskFromFile()
        {
            if (File.Exists(list[1]))
            {
                string[] fileTasks = File.ReadAllLines(list[1]);
                foreach (var task in fileTasks)
                {
                    tasks.Add(task);
                }
            }
            else
            {
                Console.WriteLine("Could not find this file");
            }
        }

        private void SaveTaskToFile()
        {
            File.WriteAllLines(list[1], tasks);
        }

        private void Complete()
        {
            int ID = -1;
            if (StringParser.Parseid(list[1], out ID))
            {
                completed.Add(list[ID]);
                tasks.RemoveAt(ID);
            }
            else
            {
                Console.WriteLine("Could note be parsed by id");
            }
        }

        private void WriteAllCompleted()
        {
            foreach (var task in completed)
            {
                Console.WriteLine(task);
            }
        }

        private void AddTimeLimitToTask()
        {
            Console.WriteLine("Please, enter the time in date format: MM.dd.yyyy");
            int ID = -1;
            if (StringParser.Parseid(list[1], out ID))
            {
                tasks[ID] += " " + list[2];
            }
            else
            {
                Console.WriteLine("Could note be parsed by id");
            }
        }

        private void TasksForToday()
        {
            foreach (var task in tasks)
            {
                
                List<string> words = new StringParser(task).GetList();
                string dateToday = DateTime.Now.ToString("MM.dd.yyyy");
                if (words.Count == 2)
                {
                    if (words[1] == dateToday)
                    {
                        Console.WriteLine(words[0]);
                    }
                }
                
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