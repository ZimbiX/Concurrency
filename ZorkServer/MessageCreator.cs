using System;
using ConcurrencyUtilities;

namespace ZorkServer
{
	public class MessageCreator: ActiveObjectInputOutput<string, string>
	{
		public MessageCreator(Channel<string> commandResult, Channel<string> commandResultMessage):
			base(commandResult, commandResultMessage) {}

		protected override string Process(string commandResult) {
			return commandResult + "\n\n" + GameStatus();
		}

		string GameStatus() {
			string message = "It is dark. You are deep within a forest, surrounded by trees. There is a small" +
				"cliff face to the North, with an iron door inset, faintly marked \"EN513b\". There is a small" +
				"pile of stones beside you. You have a key";
			if (TakeItemProcessor.HaveStone)
				message += " and a small stone";
			message += ".\n\n";
			return message;
		}
	}
}