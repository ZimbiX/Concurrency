using System;
using ConcurrencyUtilities;

namespace ZorkServer
{
	public class InvalidCommandProcessor: ActiveObjectInputOutput<string[], string>
	{
		public InvalidCommandProcessor(Channel<string[]> invalidCommand, Channel<string> commandResult): base(invalidCommand, commandResult) {

		}

		protected override string Process(string[] command) {
			return "Sorry, I don't understand the instruction \"" + String.Join(" ", command) + "\"";
		}
	}
}