using System;
using ConcurrencyUtilities;

//using System.Collections;
//using System.Linq;
//using System.Net.Sockets;
//using System.Text;

/// <summary>
/// A program to demonstrate active objects forming a multi-user (single in-game player) Zork server.
/// </summary>
namespace ZorkServer
{
	class MainClass
	{
		public static void Main(string[] args) {
			// Create channels:

			Channel<string> command = new Channel<string>();

			Channel<string[]> changeRoomCommand = new Channel<string[]>();
			Channel<string[]> useItemCommand = new Channel<string[]>();
			Channel<string[]> takeItemCommand = new Channel<string[]>();
			Channel<string[]> invalidCommand = new Channel<string[]>();

			Channel<string> commandResult = new Channel<string>();
			Channel<string> commandResultMessage = new Channel<string>();

			// Create active objects:

			ConnectionManager connectionManager = new ConnectionManager(command, commandResultMessage);

			CommandParser commandParser = new CommandParser(command, changeRoomCommand, useItemCommand, takeItemCommand, invalidCommand); // ActObj:In

			ChangeRoomProcessor changeRoomProcessor = new ChangeRoomProcessor(changeRoomCommand, commandResult);
			UseItemProcessor useItemProcessor = new UseItemProcessor(useItemCommand, commandResult);
			TakeItemProcessor takeItemProcessor = new TakeItemProcessor(takeItemCommand, commandResult);
			InvalidCommandProcessor invalidCommandProcessor = new InvalidCommandProcessor(invalidCommand, commandResult);

			MessageCreator messageCreator = new MessageCreator(commandResult, commandResultMessage);

			// Start active objects:

			connectionManager.Start();

			commandParser.Start();

			changeRoomProcessor.Start();
			useItemProcessor.Start();
			takeItemProcessor.Start();
			invalidCommandProcessor.Start();

			messageCreator.Start();
		}
	}
}