using System;
using System.Collections.Generic;
using System.IO;

namespace MyToDoList1
{
    internal class CommandExecutor
    {
        private List<string> _list = new List<string>();
        private readonly List<Group> _groups = new List<Group>();

        public CommandExecutor()
        {
            CreateGroup("Tasks");
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
                    TaskWriter.WriteAllTasks(_groups);
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
                    FileTask.SaveTaskToFile(_list[1], _groups);
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
                            TaskWriter.WriteCompletedTasks(_groups);
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
            var task = new Task(taskName, taskName.GetHashCode(), 0, false);
            var index = Group.GroupIndex(_groups, "Tasks");
            if (index.HasValue)
            {
                _groups[index.Value].AddItem(task);
            }
        }
        
        private void DeleteTask(string taskName)
        {
            if (StringParser.TryParseId(taskName, out var id))
            {
                var index = Group.GroupIndex(_groups, "Tasks");
                if (!index.HasValue) return;
                var indexTask = Task.TaskIndex(_groups[index.Value].Tasks, id);
                if (indexTask.HasValue) _groups[index.Value].DeleteItem(indexTask.Value);
            }
            else
            {
                BadId(id);
            }
        }
        
        private void LoadTaskFromFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                var fileTasks = File.ReadAllLines(fileName);
                var group = new Group(null);
                for (var j = 0; j < fileTasks.Length; ++j)
                {
                    var words = new StringParser(fileTasks[j]).Words;
                    if (words.Count == 1)
                    {
                        group = FileTask.LoadGroup(words[0], _groups);
                    }
                    else
                    {
                        var task = FileTask.LoadTask(words);
                        for (var i = 0; i < Convert.ToInt32(words[2]); ++i, ++j)
                        {
                            var subWords = new StringParser(fileTasks[j]).Words;
                            var subTask = FileTask.LoadSubTask(subWords);
                            task.AddSubTask(subTask);
                        }
                        group.AddItem(task);
                    }
                }
            }
            else
            {
                Console.WriteLine("Could not find this file");
            }
        }

        private void Complete(string taskId)
        {
            if (StringParser.TryParseId(taskId, out var id))
            {
                foreach (var group in _groups)
                {
                    var index = Task.TaskIndex(group.Tasks, id);
                    if (!index.HasValue) continue;
                    var task = group.Tasks[index.Value];
                    if (task.Id != id) task = task.SubTasks[index.Value];
                    task.Complete();
                }
            }
            else
            {
                BadId(id);
            }
        }

        private void AddTimeLimitToTask(string taskId, string date)
        {
            if (StringParser.TryParseId(taskId, out var id))
            {
                foreach (var group in _groups)
                {
                    var index = Task.TaskIndex(group.Tasks, id);
                    if (!index.HasValue) continue;
                    var tasks = group.Tasks; 
                    tasks[index.Value].SetDate(date);
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
                    if (task.Date.ToShortDateString() != dateToday.ToShortDateString() 
                        || task.IsCompleted) continue;
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
            if (index.HasValue)
            {
                _groups.RemoveAt(index.Value);
            }
            else
            {
                Group.BadName(name);
            }
        }

        private void AddItemToGroup(string taskId, string groupName)
        {
            var indexGroup = Group.GroupIndex(_groups, groupName);
            if (indexGroup.HasValue)
            {
                if (StringParser.TryParseId(taskId, out var id))
                {
                    var indexGroupDefault = Group.GroupIndex(_groups, "Tasks");
                    if (!indexGroupDefault.HasValue) return;
                    var groupDefault = _groups[indexGroupDefault.Value];
                    var index = Task.TaskIndex(groupDefault.Tasks, id);
                    if (index.HasValue)
                    {
                        _groups[indexGroup.Value].AddItem(groupDefault.Tasks[index.Value]);
                        groupDefault.Tasks.RemoveAt(index.Value);    
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
                    if (indexTask.HasValue)
                    {
                        @group.DeleteItem(indexTask.Value);
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
            var groupIndex = Group.GroupIndex(_groups, groupName);
            if (!groupIndex.HasValue) BadCommandFormat();
            else
            {
                var group = _groups[groupIndex.Value];
                TaskWriter.PrintTasks(group, true);
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
                    if (!index.HasValue) continue;
                    var subId = subTaskName.GetHashCode();
                    var subTask = new Task(subTaskName, subId, 0, false);
                    tasks[index.Value].AddSubTask(subTask);
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