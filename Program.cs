using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace MyToDoList1
{
    internal class CommandValidator
    {
        private string command;

        private List<string> valid_commands = new List<string>()
        {
            "/add", "/all", "/delete",
            "/save", "/load", "/complete",
            "/completed", "/create-group", "/delete-group",
            "/add-to-group", "/delete-from-group", 
            "/add-subtask" 
        };
        public CommandValidator(string text)
        {
            if (valid_commands.Contains(text))
            {
                command = text;
            }
            else
            {
                command = "no command";
            }
        }
        string GetResult()
        {
            return command;
        }
    }

    internal class CommandExecutor
    {
        private List<string> tasks = new List<string>();
        private string command;
        private string argument_command;
        public CommandExecutor(string word1, string word2)
        {
            command = word1;
            argument_command = word2;
        }

        public void Execute()
        {
            if (command == "/add")
            {
                AddtTask();    
            }
        }

        private void AddtTask()
        {
            tasks.Add(argument_command);
        }
        
        private void BadCommandFormat()
        {
            Console.WriteLine("Please, enter correct command\n");
        }
    }
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            string str;
            str = Console.ReadLine();
            List<string> words = new List<string>();
            while (str != "exit")
            {
                words = new List<string>(str.Split());
                CommandValidator commandValidator = new CommandValidator(words[0]);
                // if (words.Count == 3)
                // {
                //     words[1] = words[1] + " " + words[2];
                // }
                CommandExecutor commandExecutor = new CommandExecutor(words[0], words[1]);
                commandExecutor.Execute();
                str = Console.ReadLine();
            }
        }
    }
}