using System;
using System.Collections.Generic;
using System.Linq;

namespace MyToDoList1
{
    internal class Group
    {
        private readonly List<Task> _tasks = new List<Task>();
        private readonly List<string> _completed = new List<string>();
        public string Name { get; set; }

        public List<Task> GetTasks()
        {
            return _tasks;
        }

        private bool Duplicate(Task otherTask)
        {
            return _tasks.Any(task => task.Name == otherTask.Name);
        } 
        
        public void AddItem(Task task)
        {
            if (!Duplicate(task)) _tasks.Add(task);
        }
        
        public void DeleteItem(int index)
        { 
            _tasks.RemoveAt(index);
        }

        public void AddToCompleted(int index)
        {
            _completed.Add(_tasks[index].Name);
        }
        
        public void Completed()
        {
            foreach (var task in _completed)
            {
                Console.WriteLine(task);
            }
        }

        public void Tasks()
        {
            Console.WriteLine(Name + ":");
            foreach (var task in _tasks)
            {
                Console.WriteLine
                (
                    "  {0} {1} {2} {3}/{4}",
                    task.Name,
                    Convert.ToString(task.Id),
                    task.Date,
                    task.CountCompleted,
                    task.GetSubTasks().Count
                    );
                foreach (var subTask in task.GetSubTasks())
                {
                    Console.WriteLine
                    (
                        "    {0} {1} {2}",
                        subTask.Name,
                        Convert.ToString(subTask.Id),
                        subTask.Date
                    );
                }
            }
        }

        public static int FindIndexByName(List<Group> groups, string name)
        {
            var index = -1;
            for (var i = 0; i < groups.Count; i++)
            {
                if (groups[i].Name == name)
                {
                    index = i;
                }
            }

            return index;
        }

        public static void BadName(string name)
        {
            Console.WriteLine("Could not find group by name: {0}", name);
        }
    }
}