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
			TcpListener tcpListener = new TcpListener(_portStart);
			tcpListener.Start(); // Start listening
			Console.WriteLine("Waiting for connection. (telnet localhost " + _portStart + ").");
			TcpClient client = tcpListener.AcceptTcpClient(); // Wait for, and accept a client
			NetworkStream networkStream = client.GetStream(); // Get the stream

			ClientHandler clientHandler = new ClientHandler(networkStream, _command, _commandResultMessage);
			clientHandler.Start();
			Console.WriteLine("Client connected on port " + _portStart);

			_portStart++;
		}
	}
}