using System;
using System.Collections.Generic;
using System.Linq;

namespace MyToDoList1
{
    internal class Group
    {
        private readonly List<string> _completed = new List<string>();
        public List<Task> Tasks { get; }
        public string Name { get; }

        public Group(string name)
        {
            Name = name;
            Tasks = new List<Task>();
        }
        
        private bool IsDuplicate(Task otherTask) => Tasks.Any(task => task.Name == otherTask.Name);
        
        public void AddItem(Task task)
        {
            if (!IsDuplicate(task)) Tasks.Add(task);
        }
        
        public void DeleteItem(int index)
        { 
            Tasks.RemoveAt(index);
        }

        public void AddToCompleted(int index)
        {
            _completed.Add(Tasks[index].Name);
        }
        
        public void Completed()
        {
            foreach (var task in _completed)
            {
                Console.WriteLine(task);
            }
        }
        public void PrintTasks()
        {
            Console.WriteLine(Name + ":");
            foreach (var task in Tasks)
            {
                Console.WriteLine(task.ToString());
                foreach (var subTask in task.SubTasks)
                {
                    Console.WriteLine($"  {subTask.ToString()}");
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
            Console.WriteLine($"Could not find group by name: {name}");
        }
    }
}