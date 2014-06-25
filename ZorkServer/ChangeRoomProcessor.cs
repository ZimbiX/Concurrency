using System;
using ConcurrencyUtilities;

namespace ZorkServer
{
	public class ChangeRoomProcessor: ActiveObjectInputOutput<string[], string>
	{
		public ChangeRoomProcessor(Channel<string[]> changeRoomCommand, Channel<string> commandResult): base(changeRoomCommand, commandResult) {

		}

		protected override string Process(string[] command) {
			string direction = command[0];
			if (direction == "north" || direction == "up")
				return "The door won't budge. The cliff face crumbles as you instead try to climb.";
			return "Moved " + direction + ", but it was not very effective; everything looks the same!";
		}
	}
}