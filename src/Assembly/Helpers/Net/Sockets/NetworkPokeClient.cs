using Assembly.Metro.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows.Documents;

namespace Assembly.Helpers.Net.Sockets
{
	/// <summary>
	/// Network poking client.
	/// </summary>
    public class NetworkPokeClient
    {
        private Socket _socket;

		// TODO: Should we make it possible to set the port number somehow?
		private static int Port = 19002;

		/// <summary>
		/// Initializes a new instance of the <see cref="NetworkPokeClient"/> class.
		/// The client will connect to a server located at a given IP address.
		/// </summary>
		/// <param name="address">The IP address of the server to connect to.</param>
		public NetworkPokeClient(IPAddress address)
		{
			var endpoint = new IPEndPoint(address, Port);
			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			try
			{
				_socket.Connect(endpoint);
			}
			catch (SocketException)
			{
				MetroMessageBox.Show("Connection Error", "The server was not found.");
			}
		}
		/// <summary>
		/// Sends a command to the server.
		/// </summary>
		/// <param name="command">The command to send.</param>
        public bool SendCommand(PokeCommand command)
        {
			try
			{
				using (var stream = new NetworkStream(_socket, false))
					CommandSerialization.SerializeCommand(command, stream);
				return true;
			}
			catch (IOException)
			{
				return false;
			}
        }

		/// <summary>
		/// Waits for a command to be sent by the server and sends it to a handler.
		/// </summary>
		/// <param name="handler">The <see cref="IPokeCommandHandler"/> to handle the command with.</param>
        public bool ReceiveCommand(IPokeCommandHandler handler)
        {
			try
			{
				using (var stream = new NetworkStream(_socket, false))
				{
					var command = CommandSerialization.DeserializeCommand(stream);
					command.Handle(handler);
				}
			}
			catch (IOException)
			{
				_socket.Close();
				return false;
			}
			return true;
        }
    }
}
