using System;
using System.Collections.Generic;
using System.Linq;

namespace MyToDoList1
{
    internal class Task
    {
        public string Name { get; set; }
        public string Date { get; set; }
        public int Id { get; set; }
        
        public int CountCompleted { get; set; }

        private readonly List<Task> _subTasks = new List<Task>();

        private bool Duplicate(Task otherTask)
        {
            return _subTasks.Any(task => task.Name == otherTask.Name);
        } 
        
        public void AddSubTask(Task task)
        {
            if (!Duplicate(task)) _subTasks.Add(task);
        }

        public List<Task> GetSubTasks()
        {
            return _subTasks;
        }

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
}