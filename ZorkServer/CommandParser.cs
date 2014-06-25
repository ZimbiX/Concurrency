using System;
using ConcurrencyUtilities;

namespace ZorkServer
{
	public class CommandParser: ActiveObjectInput<string>
	{
		Channel<string[]> _changeRoomCommand;
		Channel<string[]> _useItemCommand;
		Channel<string[]> _takeItemCommand;
		Channel<string[]> _invalidCommand;

		// Input channel: command
		// Manual output channels: changeRoomCommand, useItemCommand, takeItemCommand, invalidCommand
		public CommandParser(Channel<string> command, Channel<string[]> changeRoomCommand, Channel<string[]> useItemCommand, Channel<string[]> takeItemCommand, Channel<string[]> invalidCommand): base(command) {
			_changeRoomCommand = changeRoomCommand;
			_useItemCommand = useItemCommand;
			_takeItemCommand = takeItemCommand;
			_invalidCommand = invalidCommand;
		}

		static bool WordsContain(string[] words, string searchString) {
			return Array.IndexOf(words, searchString) >= 0;
		}

		static bool IsATravelVerb(string word) {
			return WordsContain(new string[] { "go", "run", "walk", "travel", "move" }, word);
		}

		static bool IsADirection(string word) {
			return WordsContain(new string[] { "up", "down", "left", "right", "north", "south", "east", "west" }, word);
		}

		static bool IsDirectionCommand(string[] words) {
			return ( words.Length == 1 && IsADirection(words[0]) ||
			         words.Length == 2 && IsADirection(words[1]) && IsATravelVerb(words[0]) );
		}

		public static string DirectionFromDirectionCommand(string[] words) {
			foreach (string word in words) {
				if (IsADirection(word))
					return word;
			}
			throw new ArgumentException("No direction found in words");
		}

		static bool IsUseItemCommand(string[] words) {
			return ( words.Length == 2 && words[0] == "use" ||
			         words.Length == 4 && words[0] == "use" && WordsContain(new string[] { "in", "with" }, words[2]) );
		}

		public static string[] ItemsFromUseItemCommand(string[] words) {
			if (words.Length == 4)
				return new string[] { words[1], words[3] };
			return new string[] { words[1] };
		}

		static bool IsTakeItemCommand(string[] words) {
			return (words.Length == 2 && words[0] == "take");
		}

		public static string ItemFromTakeItemCommand(string[] words) {
			return words[1];
		}

		// Automatically process commands that appear in the command channel
		protected override void Process(string command) {
			string[] words = command.ToLower().Split(' ');
			if (IsDirectionCommand(words))
				_changeRoomCommand.Put(new string[] { DirectionFromDirectionCommand(words) });
			else if (IsUseItemCommand(words))
				_useItemCommand.Put(ItemsFromUseItemCommand(words));
			else if (IsTakeItemCommand(words))
				_takeItemCommand.Put(new string[] { ItemFromTakeItemCommand(words) });
			else
				_invalidCommand.Put(words);
		}
	}
}