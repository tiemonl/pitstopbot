namespace PitStopBot {
    class Program {
        private static void Main(string[] args) => new PitStopBot()
            .MainAsync(token: args[0], prefix: args[1])
            .GetAwaiter()
            .GetResult();
    }
}
