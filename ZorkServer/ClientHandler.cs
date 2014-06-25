using System;
using ConcurrencyUtilities;
using System.Net.Sockets;

namespace ZorkServer
{
	// Input: messages to send to the client through the network stream
	// Output: commands from the client to send to the command parser
	public class ClientHandler: ActiveObject
	{
		NetworkStream _networkStream;
		Channel<string> _command;
		Channel<string> _commandResultMessage;

		public ClientHandler(NetworkStream networkStream, Channel<string> command, Channel<string> commandResultMessage): base() {
			_networkStream = networkStream;
			_command = command;
			_commandResultMessage = commandResultMessage;
			SendStringToClient("Welcome to the Zork server\n");
		}

		string ReadStringFromClientWithPrompt() {
			SendStringToClient("> ");
			return ReadStringFromClient();
		}

		string ReadStringFromClient() {
			byte[] bytesRead = new byte[100];
			int numBytesRead = _networkStream.Read(bytesRead, 0, 100);
			string stringRead = System.Text.ASCIIEncoding.ASCII.GetString(bytesRead, 0, numBytesRead);
			return stringRead.TrimEnd('\r', '\n');
		}

		void SendStringToClient(string sendString) {
			byte[] toSend = System.Text.ASCIIEncoding.ASCII.GetBytes(sendString);
			_networkStream.Write(toSend, 0, toSend.Length);
		}

		protected override void Execute() { // Loops continuously
			// 1. Wait for a command from the client
			string commandRead = ReadStringFromClientWithPrompt();
			Console.WriteLine("New command from client: " + '"' + commandRead + '"');

			// 2. Send the command to the command parser
			_command.Put(commandRead);

			// 3. Wait for a result message from the command execution
//			string resultMessage = "Done\n";
			string resultMessage = _commandResultMessage.Take();

			// 4. Send the result message to the client
			SendStringToClient(resultMessage);
		}
	}
}