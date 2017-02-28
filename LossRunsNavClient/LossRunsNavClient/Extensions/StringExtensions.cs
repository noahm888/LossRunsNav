
using System.Collections.Generic;
using System.IO;

namespace LossRunsNavClient
{
    public static class StringExtensions
    {
        public static IEnumerable<string> ToLines(this string value)
        {
            if (string.IsNullOrEmpty(value)) { yield break; }
            using (StringReader reader = new StringReader(value))
            {
                string line;
                while (null != (line = reader.ReadLine()))
                {
                    yield return line;
                }
            }
        }
    }
}