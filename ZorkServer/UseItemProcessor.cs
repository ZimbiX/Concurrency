using System;
using ConcurrencyUtilities;

namespace ZorkServer
{
	public class UseItemProcessor: ActiveObjectInputOutput<string[], string>
	{
		public UseItemProcessor(Channel<string[]> useItemCommand, Channel<string> commandResult):
			base(useItemCommand, commandResult) {}

		protected override string Process(string[] items) {
			if (items[0] != "key")
				return "I can't find a " + items[0] + " to use.";
			if (items.Length == 2 && items[1] != "door")
				return "I can't find a " + items[1] + " to use the " + items[0] + " in";
			return "The key fits in the door, but it won't turn.";
		}
	}
}