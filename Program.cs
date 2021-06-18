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

    internal class Group
    {
        private List<string> _tasks = new List<string>();
        private List<string> _completed = new List<string>();
        private string _name;

        public Group(string name)
        {
            SetName(name);
        }
        public void SetName(string name)
        {
            _name = name;
        }

        public string Name()
        {
            return _name;
        }

        public void AddItem(string task)
        {
            _tasks.Add(task);
        }

        public void DeleteItem(int index)
        {
            _tasks.RemoveAt(index);
        }

        public void Completed()
        {
            foreach (var task in _completed)
            {
                Console.WriteLine(task);
            }
        }

        public void Tasks()
        {
            Console.WriteLine(_name);
            foreach (var task in _tasks)
            {
                Console.WriteLine("  " + task);
            }
        }
        
    }

    internal class Task
    {
        private string Name {
            get;
            set;
        }
        private string Date {
            get;
            set;
        }
        private int ID {
            get;
            set;
        }
        private bool Completed {
            get;
            set;
        }
    }

    internal class CommandExecutor
    {
        private List<string> _tasks = new List<string>();
        private List<string> _list = new List<string>();
        private List<string> _completed = new List<string>();
        
        private List<Group> _groups = new List<Group>();
        
        public void SetList(StringParser stringParser)
        {
            _list = stringParser.GetList();
        }

        public void Execute()
        {
            if (_list[0] == "/add")
            {
                if (_list.Count == 2)
                {
                    AddTask();
                }
                else
                {
                    BadCommandFormat();
                }
            }
            
            else if (_list[0] == "/all")
            {
                if (_list.Count == 1)
                {
                    WriteAllTasks();
                }
                else
                {
                    BadCommandFormat();
                }
            }

            else if (_list[0] == "/delete")
            {
                if (_list.Count == 2)
                {
                    DeleteTask();
                }
                else
                {
                    BadCommandFormat();
                }
            }
            
            else if (_list[0] == "/load")
            {
                if (_list.Count == 2)
                {
                    LoadTaskFromFile();
                }
                else
                {
                    BadCommandFormat();
                }
            }
            else if (_list[0] == "/save")
            {
                if (_list.Count == 2)
                {
                    SaveTaskToFile();
                }
                else
                {
                    BadCommandFormat();
                }
            }
            
            else if (_list[0] == "/complete")
            {
                if (_list.Count == 2)
                {
                    Complete();
                }
                else
                {
                    BadCommandFormat();
                }
            }
            
            else if (_list[0] == "/time")
            {
                if (_list.Count == 3)
                {
                    AddTimeLimitToTask();
                }
                else
                {
                    BadCommandFormat();
                }
            }
            
            else if (_list[0] == "/today")
            {
                if (_list.Count == 3)
                {
                    TasksForToday();
                }
                else
                {
                    BadCommandFormat();
                }
            }

            else if (_list[0] == "/create-group")
            {
                if (_list.Count == 2)
                {
                    CreateGroup();
                }
                else
                {
                    BadCommandFormat();
                }
            }
            
            else if (_list[0] == "/delete-group")
            {
                if (_list.Count == 2)
                {
                    DeleteGroup();
                }
                else
                {
                    BadCommandFormat();
                }
            }
            
            else if (_list[0] == "/add-to-group")
            {
                if (_list.Count == 3)
                {
                    AddItemToGroup();
                }
                else
                {
                    BadCommandFormat();
                }
            }
            
            else if (_list[0] == "/delete-from-group")
            {
                if (_list.Count == 3)
                {
                    DeleteItemFromGroup();
                }
                else
                {
                    BadCommandFormat();
                }
            }
            
            else if (_list[0] == "/completed")
            {
                if (_list.Count == 2)
                {
                    GroupCompleted();
                }
                else if (_list.Count == 1)
                {
                    WriteAllCompleted();
                }
                else
                {
                    BadCommandFormat();
                }
            }

            else
            {
                BadCommandFormat();
            }
        }

        private void AddTask()
        {
            _tasks.Add(_list[1]);
        }

        private void WriteAllTasks()
        {
            Console.WriteLine("Name of the task, id");
            for (int i = 0; i < _tasks.Count; i++)
            {
                Console.WriteLine(_tasks[i] + ", " + Convert.ToString(i));
            }
            foreach (var group in _groups)
            {
                group.Tasks();
            }
            Console.WriteLine("Completed task:");
            foreach (var task in _completed)
            {
                Console.WriteLine(task);
            }
        }
        
        private void DeleteTask()
        {
            int ID = -1;
            if (StringParser.Parseid(_list[1], out ID))
            {
                _tasks.RemoveAt(ID);
            }
            else
            {
                Console.WriteLine("Could note be parsed");
            }
        }

        private void LoadTaskFromFile()
        {
            if (File.Exists(_list[1]))
            {
                string[] fileTasks = File.ReadAllLines(_list[1]);
                foreach (var task in fileTasks)
                {
                    _tasks.Add(task);
                }
            }
            else
            {
                Console.WriteLine("Could not find this file");
            }
        }

        private void SaveTaskToFile()
        {
            File.WriteAllLines(_list[1], _tasks);
        }

        private void Complete()
        {
            int ID = -1;
            if (StringParser.Parseid(_list[1], out ID))
            {
                _completed.Add(_list[ID]);
                _tasks.RemoveAt(ID);
            }
            else
            {
                Console.WriteLine("Could note be parsed by id");
            }
        }

        private void WriteAllCompleted()
        {
            foreach (var task in _completed)
            {
                Console.WriteLine(task);
            }
        }

        private void AddTimeLimitToTask()
        {
            Console.WriteLine("Please, enter the time in date format: MM.dd.yyyy");
            int ID = -1;
            if (StringParser.Parseid(_list[1], out ID))
            {
                _tasks[ID] += " " + _list[2];
            }
            else
            {
                Console.WriteLine("Could note be parsed by id");
            }
        }

        private void TasksForToday()
        {
            foreach (var task in _tasks)
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

        private void CreateGroup()
        {
            _groups.Add(new Group(_list[1]));
        }

        private void DeleteGroup()
        {
            for (int i = 0; i < _groups.Count; i++)
            {
                if (_groups[i].Name() == _list[1])
                {
                    _groups.RemoveAt(i);
                }
            }
        }

        private void AddItemToGroup()
        {
            foreach (var @group in _groups)
            {
                if (group.Name() == _list[2])
                {
                    int ID = -1;
                    if (StringParser.Parseid(_list[1], out ID))
                    {
                        group.AddItem(_tasks[ID]);
                    }
                    else
                    {
                        Console.WriteLine("Could note be parsed by id");
                    }
                }
            }
        }

        private void DeleteItemFromGroup()
        {
            foreach (var group in _groups)
            {
                if (group.Name() == _list[2])
                {
                    int ID = -1;
                    if (StringParser.Parseid(_list[1], out ID))
                    {
                        group.DeleteItem(ID);
                    }
                    else
                    {
                        Console.WriteLine("Could note be parsed by id");
                    }
                }
            }
        }

        private void GroupCompleted()
        {
            foreach (var group in _groups)
            {
                if (group.Name() == _list[1])
                {
                    group.Completed();
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