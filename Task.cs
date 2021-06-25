using System;
using System.Collections.Generic;
using System.Linq;

namespace MyToDoList1
{
    internal class Task
    {
        public string Name { get; }
        public int Id { get; }
        public int CountCompleted { get; set; }
        public DateTime Date { get; private set; }
        
        private readonly List<Task> _subTasks = new List<Task>();

        public Task(string name, int id, int countCompleted, DateTime date = new DateTime())
        {
            Name = name;
            Id = id;
            CountCompleted = countCompleted;
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
        

        

        private bool Duplicate(Task otherTask) => _subTasks.Any(task => task.Name == otherTask.Name);
        
        public void AddSubTask(Task task)
        {
            if (!Duplicate(task)) _subTasks.Add(task);
        }

        public List<Task> GetSubTasks() => _subTasks;

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

        public new string ToString()
        {
            var date = Date.ToString("d");
            var data = $"  {Name} {Id}";
            if (Date != new DateTime()) data += $" {date}";
            if (_subTasks.Count != 0) data += $" {CountCompleted}/{_subTasks.Count}";
            return data;
        }

        public static void BadIndex(int id)
        {
            Console.WriteLine($"Could not find element by id {id}");
        }
    }
}