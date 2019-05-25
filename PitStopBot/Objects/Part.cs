using System;
namespace PitStopBot.Objects {
	public class Details {
		public string internalId { get; set; }
		public bool isElite { get; set; }
		public string serialNumber { get; set; }
		public int durability { get; set; }
		public int weight { get; set; }
		public int steering { get; set; }
		public string model { get; set; }
		public int power { get; set; }
		public string type { get; set; }
		public string brand { get; set; }
		public int speed { get; set; }
		public string rarity { get; set; }
	}

	public class Part {
		public string external_url { get; set; }
		public Details details { get; set; }
		public string description { get; set; }
		public string id { get; set; }
		public string name { get; set; }
		public string image { get; set; }
	}
}
