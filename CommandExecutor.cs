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
            _list = stringParser.Words;
        }

        public void Execute()
        {
            switch (_list[0])
            {
                case "/add" when _list.Count == 2:
                    AddTask(_list[1]);
                    break;
                case "/add":
                    BadCommandFormat();
                    break;
                case "/all" when _list.Count == 1:
                    TaskWriter.WriteAllTasks(_groups, _completed);
                    break;
                case "/all":
                    BadCommandFormat();
                    break;
                case "/delete" when _list.Count == 2:
                    DeleteTask(_list[1]);
                    break;
                case "/delete":
                    BadCommandFormat();
                    break;
                case "/load" when _list.Count == 2:
                    LoadTaskFromFile(_list[1]);
                    break;
                case "/load":
                    BadCommandFormat();
                    break;
                case "/save" when _list.Count == 2:
                    SaveTaskToFile(_list[1]);
                    break;
                case "/save":
                    BadCommandFormat();
                    break;
                case "/complete" when _list.Count == 2:
                    Complete(_list[1]);
                    break;
                case "/complete":
                    BadCommandFormat();
                    break;
                case "/time" when _list.Count == 3:
                    AddTimeLimitToTask(_list[1], _list[2]);
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
                    DeleteGroup(_list[1]);
                    break;
                case "/delete-group":
                    BadCommandFormat();
                    break;
                case "/add-to-group" when _list.Count == 3:
                    AddItemToGroup(_list[1], _list[2]);
                    break;
                case "/add-to-group":
                    BadCommandFormat();
                    break;
                case "/delete-from-group" when _list.Count == 3:
                    DeleteItemFromGroup(_list[1], _list[2]);
                    break;
                case "/delete-from-group":
                    BadCommandFormat();
                    break;
                case "/completed":
                    switch (_list.Count)
                    {
                        case 2:
                            GroupCompleted(_list[1]);
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
                    AddSubTaskToTask(_list[1], _list[2]);
                    break;
                case "/add-subtask":
                    BadCommandFormat();
                    break;
                default:
                    BadCommandFormat();
                    break;
            }
        }

        private void AddTask(string taskName)
        {
            var task = new Task(taskName, taskName.GetHashCode(), 0);
            var index = Group.GroupIndex(_groups, "Tasks");
            if (index != -1)
            {
                _groups[index].AddItem(task);
            }
        }
        
        private void DeleteTask(string taskName)
        {
            if (StringParser.TryParseId(taskName, out var id))
            {
                var index = Group.GroupIndex(_groups, "Tasks");
                if (index == -1) return;
                var indexTask = Task.TaskIndex(_groups[index].Tasks, id);
                if (indexTask != -1) _groups[index].DeleteItem(indexTask);
            }
            else
            {
                BadId(id);
            }
        }

        private int LoadGroup(string nameGroup)
        {
            var indexGroup = Group.GroupIndex(_groups, nameGroup);
            if (indexGroup != -1) return indexGroup;
            CreateGroup(nameGroup);
            indexGroup = _groups.Count - 1;

            return indexGroup;
        }

        private void LoadTaskFromFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                var indexGroup = -1;
                var fileTasks = File.ReadAllLines(fileName);
                for (var j = 0; j < fileTasks.Length; ++j)
                {
                    var words = new StringParser(fileTasks[j]).Words;
                    if (words.Count == 1)
                    {
                        indexGroup = LoadGroup(words[0]);
                    }
                    else
                    {
                        if (!StringParser.TryParseId(words[1], out var id)) BadId(id);
                        var countCompleted = Convert.ToInt32(words[2]);
                        var task = new Task(words[0], id, countCompleted);
                        if(words.Count == 5) task.SetDate(words[4]);
                        for (int i = 0; i < Convert.ToInt32(words[3]); i++)
                        {
                            ++j;
                            var subWords = new StringParser(fileTasks[j]).Words;
                            if (StringParser.TryParseId(subWords[1], out var subId)) BadId(subId);
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

        private void SaveTaskToFile(string fileName)
        {
            var lines = new List<string>();
            foreach (var group in _groups)
            {
                lines.Add(group.Name);
                foreach (var task in group.Tasks)
                {
                    var subTasks= task.SubTasks;
                    var taskString =
                        $"{task.Name} {task.Id} {task.CountCompleted} {subTasks.Count} {task.Date:MM.dd.yyyy}"; 
                    lines.Add(taskString);
                    if (subTasks.Count == 0 || task.CountCompleted == task.SubTasks.Count) continue;
                    lines.AddRange(subTasks.Select(SubTaskString));
                }
                
            }
            File.WriteAllLines(fileName, lines);
        }

        private void CompleteTask(Group group, Task task, int i)
        {
            _completed.Add(task.Name);
            @group.AddToCompleted(i);
            @group.DeleteItem(i);
        }
        
        private void CompleteSubTask(Group group, int id)
        {
            for(var i = 0; i < group.Tasks.Count; ++i)
            {
                var task = group.Tasks[i];
                var subTasks = task.SubTasks;
                if (subTasks.Count != 0)
                {
                    if (task.CountCompleted == subTasks.Count) continue;
                    var index = Task.TaskIndex(@task.SubTasks, id);
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
        
        private void Complete(string taskId)
        {
            if (StringParser.TryParseId(taskId, out var id))
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

        private void AddTimeLimitToTask(string taskId, string date)
        {
            if (StringParser.TryParseId(taskId, out var id))
            {
                foreach (var group in _groups)
                {
                    var index = Task.TaskIndex(group.Tasks, id);
                    if (index == -1) continue;
                    var tasks = group.Tasks; 
                    tasks[index].SetDate(date);
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
                foreach (var task in group.Tasks)
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

        private void DeleteGroup(string name)
        {
            var index = Group.GroupIndex(_groups, name);
            if (index != -1)
            {
                _groups.RemoveAt(index);
            }
            else
            {
                Group.BadName(name);
            }
        }

        private void AddItemToGroup(string taskId, string groupName)
        {
            var indexGroup = Group.GroupIndex(_groups, groupName);
            if (indexGroup != -1)
            {
                if (StringParser.TryParseId(taskId, out var id))
                {
                    var indexGroupDefault = Group.GroupIndex(_groups, "Tasks");
                    var groupDefault = _groups[indexGroupDefault];
                    var index = Task.TaskIndex(groupDefault.Tasks, id);
                    if (index != -1)
                    {
                        _groups[indexGroup].AddItem(groupDefault.Tasks[index]);
                        groupDefault.Tasks.RemoveAt(index);    
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
                Group.BadName(groupName);
            }
        }

        private void DeleteItemFromGroup(string taskId, string groupName)
        {
            foreach (var group in _groups)
            {
                if (@group.Name != groupName) continue;
                if (StringParser.TryParseId(taskId, out var id))
                {
                    var indexTask = Task.TaskIndex(@group.Tasks, id);
                    if (indexTask != -1)
                    {
                        @group.DeleteItem(indexTask);
                        // AddTask(group.Tasks[indexTask].Name);
                    }
                }
                else
                {
                    BadId(id);
                }
            }
        }

        private void GroupCompleted(string groupName)
        {
            foreach (var group in _groups)
            {
                if (group.Name == groupName)
                {
                    group.Completed();
                }
            }
        }

        private void AddSubTaskToTask(string taskId, string subTaskName)
        {
            if (StringParser.TryParseId(taskId, out var id))
            {
                foreach (var group in _groups)
                {
                    var tasks = group.Tasks;
                    var index = Task.TaskIndex(tasks, id);
                    if (index == -1) continue;
                    var subId = subTaskName.GetHashCode();
                    var subTask = new Task(subTaskName, subId, 0);
                    tasks[index].AddSubTask(subTask);
                }
            }
            else
            {
                BadId(id);
            }
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