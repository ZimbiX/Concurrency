using System;
using ConcurrencyUtilities;

namespace ZorkServer
{
	public class TakeItemProcessor: ActiveObjectInputOutput<string[], string>
	{
		public static bool HaveStone { get; private set; } /* It's not really worth making a player / game state class
			just to store this, or even worry about concurrency protection in this demo */

		public TakeItemProcessor(Channel<string[]> takeItemCommand, Channel<string> commandResult):
			base(takeItemCommand, commandResult) {}

		protected override string Process(string[] items) {
			if (items[0] == "stone") {
				string message;
				if (HaveStone)
					message = "You swap your stone for a new one.";
				else
					message = "You take a stone.";
				HaveStone = true;
				message += " It seems incredibly useful.";
				return message;
			}
			return "I can't find a " + items[0] + " to take.";
		}
	}
}