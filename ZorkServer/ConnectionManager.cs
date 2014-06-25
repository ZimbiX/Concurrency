using System;
using System.Net.Sockets;
using ConcurrencyUtilities;

namespace ZorkServer
{
	public class ConnectionManager: ActiveObject
	{
		Channel<string> _command;
		Channel<string> _commandResultMessage;
		int _portStart;

		public ConnectionManager(Channel<string> command, Channel<string> commandResultMessage): base() {
			_command = command;
			_commandResultMessage = commandResultMessage;
			_portStart = 5484;
		}

		protected override void Execute() { // Loops continuously
			// Create our tcpListener
			TcpListener tcpListener = new TcpListener(_portStart);
			// Start listening
			tcpListener.Start();
			// Let the user know we are waiting
			Console.WriteLine("Waiting for connection. (telnet localhost " + _portStart + ").");
			// Accept a client
			TcpClient client = tcpListener.AcceptTcpClient();
			// Get our stream
			NetworkStream networkStream = client.GetStream();

			ClientHandler clientHandler = new ClientHandler(networkStream, _command, _commandResultMessage);
			clientHandler.Start();

			// Wait for input
			Console.WriteLine("Client connected on port " + _portStart);
//			Console.ReadLine();

			_portStart++;
		}
	}
}