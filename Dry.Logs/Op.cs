using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using static System.Environment;
using static System.String;

namespace Dry.Logs
{
    public class Op : IDisposable
    {
        public static IObservable<string> Log => Subject;
        static Subject<string> Subject { get; } = new Subject<string>();
        static AsyncLocal<Op> Context { get; } = new AsyncLocal<Op>();

        public Op([CallerMemberName] string text = null)
        {
            Parent = Context.Value;
            Context.Value = this;
            Stopwatch = Stopwatch.StartNew();
            Level = Parent == null ? 0 : Parent.Level + 1;
            Frame = new List<(int, string, Func<string>)>();
            Frame.Add((Level, text, () => $"took {Stopwatch.ElapsedMilliseconds} ms"));
        }

        Op Parent { get; }
        Stopwatch Stopwatch { get; }
        int Level { get; }

        List<(int Level, string Text, Func<string> Time)> Frame { get; }

        public void Dispose()
        {
            Stopwatch.Stop();
            Context.Value = Parent;
            if (Parent != null)
                lock (Parent.Frame)
                    lock (Frame)
                        Parent.Frame.AddRange(Frame);
            else
                Subject.OnNext(ToString());
        }

        public void Trace(string text)
        {
            var ms = $"after {Stopwatch.ElapsedMilliseconds} ms";
            lock (Frame)
                Frame.Add((Level + 1, text, () => ms));
        }

        public override string ToString()
        {
            lock (Frame)
                return Join(NewLine,
                    from row in Frame
                    select $"{new string(' ', row.Level)}{row.Text} {row.Time()}");
        }

        public XElement ToXml()
        {
            lock (Frame)
                return Frame.ToXml();
        }
    }
}
