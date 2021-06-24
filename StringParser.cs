using System.Collections.Generic;

namespace MyToDoList1
{
    internal class StringParser
    {
        private readonly List<string> _words;
        public StringParser(string input)
        {
            _words = new List<string>(input.Split());
        }

        public List<string> GetList() => _words;

        public static bool ParseId(string str, out int id)
        {
            var isParsable = int.TryParse(str, out id);
            if (!isParsable)
            {
                id = -1;
            }

            return isParsable;
        }
    }
}