using System;
using System.Collections.Generic;

namespace MyToDoList1
{
    internal static class TaskWriter
    {
        private static void PrintTasks(Group @group)
        {
            Console.WriteLine(group.Name + ":");
            foreach (var task in group.Tasks)
            {
                Console.WriteLine(task.ToString());
                foreach (var subTask in task.SubTasks)
                {
                    Console.WriteLine($"  {subTask}");
                }
            }
        }
        public static void WriteAllTasks(IEnumerable<Group> groups, List<string> completed)
        {
            Console.WriteLine("Format: Name of the task, id, date");
            foreach (var group in groups) PrintTasks(group);
            if (completed.Count != 0) Console.WriteLine("Completed task:");
            foreach (var task in completed) Console.WriteLine(task);
        }
    }
}