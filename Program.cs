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
    
    internal class Task
    {
        public string Name {
            get;
            set;
        }
        public string Date {
            get;
            set;
        }
        public int ID {
            get;
            set;
        }

        public static int GetindexTaskByid(List<Task> _tasks, int ID)
        {
            int index = -1; 
            for (int i = 0; i < _tasks.Count; i++)
            {
                if (ID == _tasks[i].ID)
                {
                    index = i;
                }
            }

            return index;
        } 
    }
    
    
    
    internal class Group
    {
        private List<Task> _tasks = new List<Task>();
        private List<string> _completed = new List<string>();
        public string Name { get; set; }

        public List<Task> GetTasks()
        {
            return _tasks;
        }

        public void AddItem(Task task)
        {
            _tasks.Add(task);
        }

        public void DeleteItem(int ID)
        {
            int index = Task.GetindexTaskByid(_tasks, ID);
            if (index != -1)
            {
                _tasks.RemoveAt(index);
            }
            else
            {
                Console.WriteLine("Could not find element by ID:" + ID);
            }
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
            Console.WriteLine(Name + ":");
            foreach (var task in _tasks)
            {
                Console.WriteLine("  " + task.Name + " " + Convert.ToString(task.ID) + " " + task.Date);
            }
        }

        public static int FindIndexByName(List<Group> _groups, string name)
        {
            int index = -1;
            for (int i = 0; i < _groups.Count; i++)
            {
                if (_groups[i].Name == name)
                {
                    index = i;
                }
            }

            return index;
        }
        
    }
    
    internal class CommandExecutor
    {
        private List<Task> _tasks = new List<Task>();
        private List<string> _list = new List<string>();
        private List<string> _completed = new List<string>();

        private List<Group> _groups = new List<Group>();

        private static int _counter;

        public CommandExecutor()
        {
            var group = new Group();
            group.Name = "Tasks (without group)";
            _groups.Add(group);
        }
        
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
            var task = new Task
            {
                Name = _list[1],
                ID = _counter++,
                Date = "",
            };
            int index = Group.FindIndexByName(_groups, "Tasks (without group)");
            if (index != -1)
            {
                _groups[index].AddItem(task);
            }
        }

        private void WriteAllTasks()
        {
            Console.WriteLine("Format: Name of the task, id, date");
            foreach (var group in _groups)
            {
                group.Tasks();
            }

            if (_completed.Count != 0)
            {
                Console.WriteLine("Completed task:");
            }
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
                int index = Group.FindIndexByName(_groups, "Tasks (without group)");
                if (index != -1)
                {
                    _groups[index].DeleteItem(ID);
                }
            }
            else
            {
                Console.WriteLine("Could not be parsed");
            }
        }

        private void LoadTaskFromFile()
        {
            if (File.Exists(_list[1]))
            {
                string[] fileTasks = File.ReadAllLines(_list[1]);
                foreach (var line in fileTasks)
                {
                    List<string> words = new StringParser(line).GetList();
                    Task task = new Task();
                    task.Name = words[0];
                    int ID = -1;
                    if (StringParser.Parseid(words[1], out ID))
                    {
                        task.ID = ID;
                    }
                    else
                    {
                        Console.WriteLine("Could not be parsed by id");
                    }
                    if (words.Count == 3)
                    {
                        task.Date = words[2];
                    }
                    else
                    {
                        task.Date = "";
                    }
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
            List<string> lines = new List<string>();
            foreach (var task in _tasks)
            {
                lines.Add(task.Name + " " + Convert.ToString(task.ID) + " " + task.Date);
            }
            File.WriteAllLines(_list[1], lines);
        }

        private void Complete()
        {
            int ID = -1;
            if (StringParser.Parseid(_list[1], out ID))
            {
                int index = -1;
                foreach (var group in _groups)
                {
                    index = Task.GetindexTaskByid(group.GetTasks(), ID);
                    if (index != -1)
                    {
                        _completed.Add(group.GetTasks()[index].Name);
                        group.DeleteItem(index);
                    }
                }
                
                if (index == -1)
                {
                    Console.WriteLine("Could not find element by ID:" + ID);
                }
            }
            else
            {
                Console.WriteLine("Could not be parsed by id");
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
                int index = -1;
                foreach (var group in _groups)
                {
                    index = Task.GetindexTaskByid(group.GetTasks(), ID);
                }
                _tasks[index].Date = _list[2];
            }
            else
            {
                Console.WriteLine("Could not be parsed by id");
            }
        }

        private void TasksForToday()
        {
            string dateToday = DateTime.Now.ToString("MM.dd.yyyy");
            string TaskName = "";
            foreach (var group in _groups)
            {
                foreach (var task in group.GetTasks())
                {
                    if (task.Date == dateToday)
                    {
                        TaskName = task.Name;
                    }
                }
            }

            if (TaskName != "")
            {
                Console.WriteLine(TaskName);
            }
            else
            {
                Console.WriteLine("Today you're chilling");
            }
        }

        private void CreateGroup()
        {
            var group = new Group();
            group.Name = _list[1];
            _groups.Add(group);
        }

        private void DeleteGroup()
        {
            for (int i = 0; i < _groups.Count; i++)
            {
                if (_groups[i].Name == _list[1])
                {
                    _groups.RemoveAt(i);
                }
            }
        }

        private void AddItemToGroup()
        {
            foreach (var group in _groups)
            {
                if (group.Name == _list[2])
                {
                    int ID = -1;
                    if (StringParser.Parseid(_list[1], out ID))
                    {
                        //Add support to find tasks in default(not in _tasks)
                        int IndexGroupDefault = Group.FindIndexByName(_groups, "Tasks (without group)");
                        Group GroupDefault = _groups[IndexGroupDefault];
                        int index = Task.GetindexTaskByid(GroupDefault.GetTasks(), ID);
                        if (index != -1)
                        {
                            group.AddItem(GroupDefault.GetTasks()[index]);
                            GroupDefault.GetTasks().RemoveAt(index);
                        }
                        else
                        {
                            Console.WriteLine("Could not find element by ID:" + ID);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Could not be parsed by id");
                    }
                }
            }
        }

        private void DeleteItemFromGroup()
        {
            foreach (var group in _groups)
            {
                if (group.Name == _list[2])
                {
                    int ID = -1;
                    if (StringParser.Parseid(_list[1], out ID))
                    {
                        group.DeleteItem(ID);
                    }
                    else
                    {
                        Console.WriteLine("Could not be parsed by id");
                    }
                }
            }
        }

        private void GroupCompleted()
        {
            foreach (var group in _groups)
            {
                if (group.Name == _list[1])
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