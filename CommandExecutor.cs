using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MyToDoList1
{
    internal class CommandExecutor
    {
        private List<string> _list = new List<string>();
        private readonly List<string> _completed = new List<string>();
        private readonly List<Group> _groups = new List<Group>();

        public CommandExecutor()
        {
            var group = new Group("Tasks");
            _groups.Add(group);
        }
        
        public void SetList(StringParser stringParser)
        {
            _list = stringParser.GetList();
        }

        public void Execute()
        {
            switch (_list[0])
            {
                case "/add" when _list.Count == 2:
                    AddTask();
                    break;
                case "/add":
                    BadCommandFormat();
                    break;
                case "/all" when _list.Count == 1:
                    WriteAllTasks();
                    break;
                case "/all":
                    BadCommandFormat();
                    break;
                case "/delete" when _list.Count == 2:
                    DeleteTask();
                    break;
                case "/delete":
                    BadCommandFormat();
                    break;
                case "/load" when _list.Count == 2:
                    LoadTaskFromFile();
                    break;
                case "/load":
                    BadCommandFormat();
                    break;
                case "/save" when _list.Count == 2:
                    SaveTaskToFile();
                    break;
                case "/save":
                    BadCommandFormat();
                    break;
                case "/complete" when _list.Count == 2:
                    Complete();
                    break;
                case "/complete":
                    BadCommandFormat();
                    break;
                case "/time" when _list.Count == 3:
                    AddTimeLimitToTask();
                    break;
                case "/time":
                    BadCommandFormat();
                    break;
                case "/today" when _list.Count == 1:
                    TasksForToday();
                    break;
                case "/today":
                    BadCommandFormat();
                    break;
                case "/create-group" when _list.Count == 2:
                    CreateGroup(_list[1]);
                    break;
                case "/create-group":
                    BadCommandFormat();
                    break;
                case "/delete-group" when _list.Count == 2:
                    DeleteGroup();
                    break;
                case "/delete-group":
                    BadCommandFormat();
                    break;
                case "/add-to-group" when _list.Count == 3:
                    AddItemToGroup();
                    break;
                case "/add-to-group":
                    BadCommandFormat();
                    break;
                case "/delete-from-group" when _list.Count == 3:
                    DeleteItemFromGroup();
                    break;
                case "/delete-from-group":
                    BadCommandFormat();
                    break;
                case "/completed":
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

                    break;
                case "/add-subtask" when _list.Count == 3:
                    AddSubTaskToTask();
                    break;
                case "/add-subtask":
                    BadCommandFormat();
                    break;
                default:
                    BadCommandFormat();
                    break;
            }
        }

        private void AddTask()
        {
            var task = new Task(_list[1], _list[1].GetHashCode(), 0);
            var index = Group.FindIndexByName(_groups, "Tasks");
            if (index != -1)
            {
                _groups[index].AddItem(task);
            }
        }

        private void WriteAllTasks()
        {
            Console.WriteLine("Format: Name of the task, id, date");
            foreach (var group in _groups) group.Tasks();
            if (_completed.Count != 0) Console.WriteLine("Completed task:");
            foreach (var task in _completed) Console.WriteLine(task);
        }
        
        private void DeleteTask()
        {
            if (StringParser.ParseId(_list[1], out var id))
            {
                var index = Group.FindIndexByName(_groups, "Tasks");
                if (index == -1) return;
                var indexTask = Task.GetIndexTaskById(_groups[index].GetTasks(), id);
                if (indexTask != -1) _groups[index].DeleteItem(indexTask);
            }
            else
            {
                BadId(id);
            }
        }

        private int LoadGroup(string nameGroup)
        {
            var indexGroup = Group.FindIndexByName(_groups, nameGroup);
            if (indexGroup != -1) return indexGroup;
            CreateGroup(nameGroup);
            indexGroup = _groups.Count - 1;

            return indexGroup;
        }

        private void LoadTaskFromFile()
        {
            if (File.Exists(_list[1]))
            {
                var indexGroup = -1;
                var fileTasks = File.ReadAllLines(_list[1]);
                for (var j = 0; j < fileTasks.Length; ++j)
                {
                    var words = new StringParser(fileTasks[j]).GetList();
                    if (words.Count == 1)
                    {
                        indexGroup = LoadGroup(words[0]);
                    }
                    else
                    {
                        if (!StringParser.ParseId(words[1], out var id)) BadId(id);
                        var countCompleted = Convert.ToInt32(words[2]);
                        var task = new Task(words[0], id, countCompleted);
                        if(words.Count == 5) task.SetDate(words[4]);
                        for (int i = 0; i < Convert.ToInt32(words[3]); i++)
                        {
                            ++j;
                            var subWords = new StringParser(fileTasks[j]).GetList();
                            if (StringParser.ParseId(subWords[1], out var subId)) BadId(subId);
                            var subTask = new Task(subWords[0], subId, 0);
                            if(subWords.Count == 3) task.SetDate(subWords[2]);
                            task.AddSubTask(subTask);
                        }
                        _groups[indexGroup].AddItem(task);
                    }
                }
            }
            else
            {
                Console.WriteLine("Could not find this file");
            }
        }

        private static string SubTaskString(Task subTask) => $"{subTask.Name} {subTask.Id} {subTask.Date:MM.dd.yyyy}";

        private void SaveTaskToFile()
        {
            var lines = new List<string>();
            foreach (var group in _groups)
            {
                lines.Add(group.Name);
                foreach (var task in group.GetTasks())
                {
                    var subTasks= task.GetSubTasks();
                    var taskString =
                        $"{task.Name} {task.Id} {task.CountCompleted} {subTasks.Count} {task.Date:MM.dd.yyyy}"; 
                    lines.Add(taskString);
                    if (subTasks.Count == 0 || task.CountCompleted == task.GetSubTasks().Count) continue;
                    lines.AddRange(subTasks.Select(SubTaskString));
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
                    var tasks = group.GetTasks(); 
                    tasks[index].SetDate(_list[2]);
                }
            }
            else
            {
                BadId(id);
            }
        }

        private void TasksForToday()
        {
            var dateToday = DateTime.Now;
            var taskName = "";
            foreach (var group in _groups)
            {
                foreach (var task in group.GetTasks())
                {
                    if (task.Date.ToShortDateString() != dateToday.ToShortDateString()) continue;
                    taskName = task.Name;
                    Console.WriteLine(taskName);
                }
            }
            if (taskName == "") Console.WriteLine("Today you're chilling");
        }

        private void CreateGroup(string name)
        {
            var group = new Group(name);
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
                    var indexGroupDefault = Group.FindIndexByName(_groups, "Tasks");
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
                if (@group.Name != _list[2]) continue;
                if (StringParser.ParseId(_list[1], out var id))
                {
                    var indexTask = Task.GetIndexTaskById(@group.GetTasks(), id);
                    if (indexTask != -1) @group.DeleteItem(indexTask);
                }
                else
                {
                    BadId(id);
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
                    var subId = _list[2].GetHashCode();
                    var subTask = new Task(_list[0], subId, 0);
                    tasks[index].AddSubTask(subTask);
                }
            }
            //Bad id
        }
        
        private static void BadCommandFormat()
        {
            Console.WriteLine("Please, enter correct command name");
        }

        private static void BadId(int id)
        {
            Console.WriteLine($"Could not be parsed by id: {id}");
        } 
    }
}