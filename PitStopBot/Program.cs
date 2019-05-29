namespace PitStopBot {
    class Program {
        private static void Main(string[] args) => new PitStopBot().MainAsync().GetAwaiter().GetResult();
    }
}
