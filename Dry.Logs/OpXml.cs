using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Xml.Linq
{
    static class OpXml
    {
        public static XElement ToXml(this List<(int Level, string Text, Func<string> Time)> frame)
        {
            var stack = new Stack<(int Level, XElement Element)>();
            stack.Push((0, element(frame[0])));
            foreach (var row in frame.Skip(1))
            {
                (int Level, XElement Element) s1 = stack.Peek();
                (int Level, XElement Element) s2 = (row.Level, element(row));
                if (s1.Level < s2.Level)
                {
                    s1.Element.Add(s2.Element);
                    stack.Push(s2);
                }
                else
                {
                    while (s1.Level >= s2.Level && stack.Count >= 2)
                    {
                        stack.Pop();
                        s1 = stack.Peek();
                    }
                    s1.Element.Add(s2.Element);
                    stack.Push(s2);
                }
            }

            return stack.Select(s => s.Element).Last();

            XElement element((int Level, string Text, Func<string> Time) row) =>
                new XElement("Line",
                    new XAttribute("Level", row.Level),
                    new XAttribute("Text", row.Text),
                    new XAttribute("Time", row.Time()));
        }
    }
}
