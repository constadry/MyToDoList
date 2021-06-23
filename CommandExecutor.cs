using System;
using System.Collections.Generic;
using System.IO;

namespace MyToDoList1
{
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
            
            else if (_list[0] == "/add-subtask")
            {
                if (_list.Count == 3)
                {
                    AddSubTaskToTask();
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
                Id = _list[1].GetHashCode(),
                Date = "",
                CountCompleted = 0
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
                    var indexTask = Task.GetIndexTaskById(_groups[index].GetTasks(), id);
                    if (indexTask != -1) _groups[index].DeleteItem(indexTask);
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
                        task.CountCompleted = 0;
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

        private void CompleteTask(Group group, Task task, int i)
        {
            _completed.Add(task.Name);
            @group.AddToCompleted(i);
            @group.DeleteItem(i);
        }
        
        private void CompleteSubTask(Group group, int id)
        {
            for(var i = 0; i < group.GetTasks().Count; ++i)
            {
                var task = group.GetTasks()[i];
                var subTasks = task.GetSubTasks();
                if (subTasks.Count != 0)
                {
                    if (task.CountCompleted == subTasks.Count) continue;
                    var index = Task.GetIndexTaskById(@task.GetSubTasks(), id);
                    if (index == -1) continue;
                    
                    // _completed.Add(@task.GetSubTasks()[index].Name);
                    ++task.CountCompleted;
                    if (task.CountCompleted != subTasks.Count) continue;
                    CompleteTask(group, task, i);
                }
                else
                {
                    if (task.Id == id)
                    {
                        CompleteTask(group, task, i);
                    }
                }
            }
        }
        
        private void Complete()
        {
            if (StringParser.ParseId(_list[1], out var id))
            {
                foreach (var group in _groups)
                {
                    CompleteSubTask(group, id);
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
                        var indexTask = Task.GetIndexTaskById(group.GetTasks(), id);
                        if (indexTask != -1) group.DeleteItem(indexTask);
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

        private void AddSubTaskToTask()
        {
            if (StringParser.ParseId(_list[1], out var id))
            {
                foreach (var group in _groups)
                {
                    var tasks = group.GetTasks();
                    var index = Task.GetIndexTaskById(tasks, id);
                    if (index == -1) continue;
                    var subTask = new Task();
                    subTask.Name = _list[2];
                    subTask.Id = _list[2].GetHashCode();
                    subTask.Date = "";
                    tasks[index].AddSubTask(subTask);
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
}