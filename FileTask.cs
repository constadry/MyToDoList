using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MyToDoList1
{
    internal static class FileTask
    {
        public static Group LoadGroup(string groupName, List<Group> groups)
        {
            var indexGroup = Group.GroupIndex(groups, groupName);
            if (indexGroup.HasValue) return groups[indexGroup.Value];
            var group = new Group(groupName);
            groups.Add(@group);
            indexGroup = groups.Count - 1;

            return groups[indexGroup.Value];
        }

        public static Task LoadTask(List<string> words)
        {
            var id = words[0].GetHashCode(); 
            var countCompleted = Convert.ToInt32(words[1]);
            var isCompleted = Convert.ToBoolean(words[3]);
            var task = new Task(words[0], id, countCompleted, isCompleted);
            if(words.Count == 5) task.SetDate(words[4]);
            return task;
        }

        public static Task LoadSubTask(List<string> subWords)
        {
            var subId = subWords[0].GetHashCode();
            var isCompleted = Convert.ToBoolean(subWords[1]);
            var subTask = new Task(subWords[0], subId, 0, isCompleted);
            if(subWords.Count == 3) subTask.SetDate(subWords[2]);
            return subTask;
        }
        
        private static string SubTaskToString(Task subTask) => $"{subTask.Name} {subTask.IsCompleted} {subTask.Date:MM.dd.yyyy}";
        
        public static void SaveTaskToFile(string fileName, IEnumerable<Group> groups)
        {
            var lines = new List<string>();
            foreach (var group in groups)
            {
                lines.Add(group.Name);
                foreach (var task in group.Tasks)
                {
                    var subTasks= task.SubTasks;
                    var taskString =
                        $"{task.Name} {task.CountCompleted} {subTasks.Count} {task.IsCompleted} {task.Date:MM.dd.yyyy}"; 
                    lines.Add(taskString);
                    if (subTasks.Count == 0 || task.CountCompleted == task.SubTasks.Count) continue;
                    lines.AddRange(subTasks.Select(SubTaskToString));
                }
                
            }
            File.WriteAllLines(fileName, lines);
        }
        
    }
}