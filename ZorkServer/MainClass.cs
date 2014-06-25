using System;
using ConcurrencyUtilities;

//using System.Collections;
//using System.Linq;
//using System.Net.Sockets;
//using System.Text;

/// <summary>
/// A program to demonstrate active objects forming a multi-user (single in-game player) Zork server.
/// The ConnectionManager listens for connections, spawning a new ClientHandler for each one. The ClientHandler
/// takes a command from the connected user, sends it to the CommandParser as a string, waits for a
/// command result message, then sends that back to the connected user.
/// This example is limited in that if two users were to execute a command at the same time, the command outputs may be
/// received by the wrong user.
/// The CommandParser validates the command, then sends its keywords to the appropriate command processor, out of
/// ChangeRoomProcessor, UseItemProcessor, TakeItemProcessor, or InvalidCommandProcessor.
/// The command processor sends a message resulting from the command processing to the MessageCreator. This then
/// assembles the command result output message, adding information about the game state to the processing result
/// message. The message is sent to the ClientHandler, which finally sends it back to the connected user.
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