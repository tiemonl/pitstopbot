namespace PitStopBot {
	class Program {
		static void Main(string[] args) => new PitStopBot().MainAsync().GetAwaiter().GetResult();
	}
}
