using Dry.Logs;
using Nito.AsyncEx;
using System;
using System.Threading.Tasks;
using static System.Console;
using static System.Linq.Enumerable;

namespace Demo
{
    class Program
    {
        static void Main() => AsyncContext.Run(MainAsync);

        static async Task MainAsync()
        {
            Op.Log.Subscribe(WriteLine);
            using (new Op())
                await Task.WhenAll(
                    from i in Range(0, 4)
                    select AlphaAsync());
        }

        private static async Task AlphaAsync()
        {
            using (new Op("Alpha function"))
                await BetaAsync();
        }

        private static async Task BetaAsync()
        {
            using (var op = new Op())
            {
                op.Trace("Waiting...");
                await Task.Delay(100);
                op.Trace("Continue");
            }
        }
    }
}
