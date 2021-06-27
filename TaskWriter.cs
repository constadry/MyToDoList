using System;
using System.Collections.Generic;

namespace MyToDoList1
{
    internal static class TaskWriter
    {
        private static void PrintTasks(Group @group, bool completed)
        {
            Console.WriteLine(group.Name + ":");
            foreach (var task in group.Tasks)
            {
                if (task.IsCompleted && !completed || !task.IsCompleted && completed) continue;
                Console.WriteLine(task.ToString());
                foreach (var subTask in task.SubTasks)
                {
                    if (subTask.IsCompleted && !completed || subTask.IsCompleted && completed) continue;
                    Console.WriteLine($"  {subTask}");
                }
            }
        }
        
        public static void WriteAllTasks(List<Group> groups)
        {
            Console.WriteLine("Format: Name of the task, id, date");
            const bool completed = false;
            foreach (var group in groups) PrintTasks(group, completed);
            Console.WriteLine("Completed:");
            foreach (var group in groups) PrintTasks(group, !completed);
        }

        public static void WriteCompletedTasks(IEnumerable<Group> groups)
        {
            const bool completed = false;
            foreach (var group in groups) PrintTasks(group, !completed);
        }
    }
}