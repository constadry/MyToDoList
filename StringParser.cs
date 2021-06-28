using System.Collections.Generic;

namespace MyToDoList1
{
    internal class StringParser
    {
        public List<string> Words { get; }
        public StringParser(string input)
        {
            Words = new List<string>(input.Split());
        }
        public static bool TryParseId(string str, out int id)
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