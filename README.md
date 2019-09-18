# drylogs

Operation logger

[GitHub]( https://github.com/dmitrynogin/drylogs), [NuGet](https://www.nuget.org/packages/Dry.Logs/):
This component traces execution in a logical order instead of chronological as everybody else :)

For example, the following code:

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

Will generate:

    MainAsync took 116 ms
      Alpha function took 109 ms
        BetaAsync took 108 ms
          Waiting... after 0 ms
          Continue after 108 ms
      Alpha function took 109 ms
        BetaAsync took 109 ms
          Waiting... after 0 ms
          Continue after 109 ms
      Alpha function took 109 ms
        BetaAsync took 109 ms
          Waiting... after 0 ms
          Continue after 109 ms
      Alpha function took 111 ms
        BetaAsync took 111 ms
          Waiting... after 0 ms
          Continue after 111 ms

Which is a way more readable…




