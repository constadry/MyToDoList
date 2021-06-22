using System;
using System.Collections.Generic;
using System.IO;

namespace MyToDoList1
{
    internal class StringParser
    {
        private readonly List<string> _words;
        public StringParser(string input)
        {
            _words = new List<string>(input.Split());
        }

        public List<string> GetList()
        {
            return _words;
        }

        public static bool ParseId(string str, out int id)
        {
            var isParsable = int.TryParse(str, out id);
            if (!isParsable)
            {
                id = -1;
            }

            return isParsable;
        }
    }
    
    internal class Task
    {
        public string Name { get; set; }
        public string Date { get; set; }
        public int Id { get; set; }

        public static int GetIndexTaskById(List<Task> tasks, int id)
        {
            var index = -1; 
            for (var i = 0; i < tasks.Count; i++)
            {
                if (id == tasks[i].Id)
                {
                    index = i;
                }
            }

            return index;
        }

        public static void BadIndex(int id)
        {
            Console.WriteLine("Could not find element by id {0}", id);
        }
    }
    
    internal class Group
    {
        private readonly List<Task> _tasks = new List<Task>();
        private readonly List<string> _completed = new List<string>();
        public string Name { get; set; }

        public List<Task> GetTasks()
        {
            return _tasks;
        }

        public void AddItem(Task task)
        {
            _tasks.Add(task);
        }

        public void DeleteItem(int id)
        {
            var index = Task.GetIndexTaskById(_tasks, id);
            if (index != -1)
            {
                _tasks.RemoveAt(index);
            }
        }

        public void AddToCompleted(int index)
        {
            _completed.Add(_tasks[index].Name);
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
                Console.WriteLine("  {0} {1} {2}", task.Name, Convert.ToString(task.Id), task.Date);
            }
        }

        public static int FindIndexByName(List<Group> groups, string name)
        {
            var index = -1;
            for (var i = 0; i < groups.Count; i++)
            {
                if (groups[i].Name == name)
                {
                    index = i;
                }
            }

            return index;
        }

        public static void BadName(string name)
        {
            Console.WriteLine("Could not find group by name: {0}", name);
        }
    }
    
    internal class CommandExecutor
    {
        private List<string> _list = new List<string>();
        private readonly List<string> _completed = new List<string>();
        private readonly List<Group> _groups = new List<Group>();

        public CommandExecutor()
        {
            var group = new Group();
            group.Name = "Tasks";
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
                if (_list.Count == 1)
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
                    CreateGroup(_list[1]);
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
                switch (_list.Count)
                {
                    case 2:
                        GroupCompleted();
                        break;
                    case 1:
                        WriteAllCompleted();
                        break;
                    default:
                        BadCommandFormat();
                        break;
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
                Id = _list[1].GetHashCode(),
                Date = "",
            };
            var index = Group.FindIndexByName(_groups, "Tasks");
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
            if (StringParser.ParseId(_list[1], out var id))
            {
                var index = Group.FindIndexByName(_groups, "Tasks");
                if (index != -1)
                {
                    _groups[index].DeleteItem(id);
                }
            }
            else
            {
                BadId(id);
            }
        }

        private int LoadGroup(string nameGroup)
        {
            var indexGroup = Group.FindIndexByName(_groups, nameGroup);
            if (indexGroup == -1)
            {
                CreateGroup(nameGroup);
                indexGroup = _groups.Count - 1;
            }

            return indexGroup;
        }

        private void LoadTaskFromFile()
        {
            if (File.Exists(_list[1]))
            {
                var indexGroup = -1;
                string[] fileTasks = File.ReadAllLines(_list[1]);
                foreach (var line in fileTasks)
                {
                    var words = new StringParser(line).GetList();
                    if (words.Count == 1)
                    {
                        indexGroup = LoadGroup(words[0]);
                    }
                    else
                    {
                        var task = new Task {Name = words[0]};
                        if (StringParser.ParseId(words[1], out var id))
                        {
                            task.Id = id;
                        }
                        else
                        {
                            BadId(id);
                        }
                        task.Date = words.Count == 3 ? words[2] : "";
                        _groups[indexGroup].AddItem(task);
                    }
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
            foreach (var group in _groups)
            {
                lines.Add(group.Name);
                foreach (var task in group.GetTasks())
                {
                    lines.Add(task.Name + " " + Convert.ToString(task.Id) + " " + task.Date);
                }
                
            }
            File.WriteAllLines(_list[1], lines);
        }
        
        private void Complete()
        {
            if (StringParser.ParseId(_list[1], out var id))
            {
                foreach (var group in _groups)
                {
                    var index = Task.GetIndexTaskById(@group.GetTasks(), id);
                    if (index == -1) continue;
                    _completed.Add(@group.GetTasks()[index].Name);
                    @group.AddToCompleted(index);
                    @group.DeleteItem(id);
                }
            }
            else
            {
                BadId(id);
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
            if (StringParser.ParseId(_list[1], out var id))
            {
                foreach (var group in _groups)
                {
                    var index = Task.GetIndexTaskById(group.GetTasks(), id);
                    if (index == -1) continue;
                    group.GetTasks()[index].Date = _list[2];
                }
            }
            else
            {
                BadId(id);
            }
        }

        private void TasksForToday()
        {
            var dateToday = DateTime.Now.ToString("MM.dd.yyyy");
            var taskName = "";
            foreach (var group in _groups)
            {
                foreach (var task in group.GetTasks())
                {
                    if (task.Date == dateToday)
                    {
                        taskName = task.Name;
                        Console.WriteLine(taskName);
                    }
                }
            }
            if (taskName == "") Console.WriteLine("Today you're chilling");
        }

        private void CreateGroup(string name)
        {
            var group = new Group();
            group.Name = name;
            _groups.Add(group);
        }

        private void DeleteGroup()
        {
            var index = Group.FindIndexByName(_groups, _list[1]);
            if (index != -1)
            {
                _groups.RemoveAt(index);
            }
            else
            {
                Group.BadName(_list[1]);
            }
        }

        private void AddItemToGroup()
        {
            var indexGroup = Group.FindIndexByName(_groups, _list[2]);
            if (indexGroup != -1)
            {
                if (StringParser.ParseId(_list[1], out var id))
                {
                    Console.WriteLine(id);
                    var indexGroupDefault = Group.FindIndexByName(_groups, "Tasks");
                    Console.WriteLine(indexGroupDefault);
                    var groupDefault = _groups[indexGroupDefault];
                    var index = Task.GetIndexTaskById(groupDefault.GetTasks(), id);
                    if (index != -1)
                    {
                        _groups[indexGroup].AddItem(groupDefault.GetTasks()[index]);
                        groupDefault.GetTasks().RemoveAt(index);    
                    }
                    else
                    {
                        Task.BadIndex(id);
                    }
                }
                else
                {
                    BadId(id);
                }
            }
            else
            {
                Group.BadName(_list[1]);
            }
        }

        private void DeleteItemFromGroup()
        {
            foreach (var group in _groups)
            {
                if (group.Name == _list[2])
                {
                    if (StringParser.ParseId(_list[1], out var id))
                    {
                        group.DeleteItem(id);
                    }
                    else
                    {
                        BadId(id);
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
        
        private static void BadCommandFormat()
        {
            Console.WriteLine("Please, enter correct command name");
        }

        private static void BadId(int id)
        {
            Console.WriteLine("Could not be parsed by id: {0}", id);
        } 
    }
    
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
            StringParser stringParser = new StringParser(input);
            CommandExecutor commandExecutor = new CommandExecutor();
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