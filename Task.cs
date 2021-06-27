using System;
using System.Collections.Generic;
using System.Linq;

namespace MyToDoList1
{
    internal class Task
    {
        public string Name { get; }
        public int Id { get; }
        public int CountCompleted { get; }
        public DateTime Date { get; private set; }
        public bool IsCompleted { get; private set; }
        public List<Task> SubTasks { get; }
        public Task(string name, int id, int countCompleted, bool isCompleted)
        {
            Name = name;
            Id = id;
            CountCompleted = countCompleted;
            IsCompleted = isCompleted;
            SubTasks = new List<Task>();
        }

        public void SetDate(string dateString)
        {
            try{
                Date = DateTime.Parse(dateString,
                    System.Globalization.CultureInfo.InvariantCulture);
            }
            catch(Exception e){
                Console.WriteLine($"Invalid date format:\n{e}");
            }
        }

        private bool IsDuplicate(Task otherTask) => SubTasks.Any(task => task.Name == otherTask.Name);
        
        public void AddSubTask(Task task)
        {
            if (!IsDuplicate(task)) SubTasks.Add(task);
        }

        public static int TaskIndex(List<Task> tasks, int id)
        {
            var index = -1; 
            for (var i = 0; i < tasks.Count; i++)
            {
                if (tasks[i].SubTasks.Count != 0)
                {
                    TaskIndex(tasks[i].SubTasks, id);
                } 
                else if (id == tasks[i].Id)
                {
                    index = i;
                }
            }

            return index;
        }

        public override string ToString()
        {
            var date = Date.ToString("d");
            var data = $"  {Name} {Id}";
            if (Date != new DateTime()) data += $" {date}";
            if (SubTasks.Count != 0) data += $" {CountCompleted}/{SubTasks.Count}";
            return data;
        }

        public void Complete()
        {
            IsCompleted = true;
        }

        public static void BadIndex(int id)
        {
            Console.WriteLine($"Could not find element by id {id}");
        }
    }
}