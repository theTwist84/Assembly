using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Mono.Nat;
using System.IO;

namespace Assembly.Helpers.Net.Sockets
{
	/// <summary>
	/// Network poking server.
	/// </summary>
	public class NetworkPokeServer
	{
		private Socket _listener;
		private readonly List<ClientModel> _clients = new List<ClientModel>();

		// TODO: Should we make it possible to set the port number somehow?
		private static int Port = 19002;

		private static string UpnpDescription = "Assembly Network Poking";

		/// <summary>
		/// Initializes a new instance of the <see cref="NetworkPokeServer"/> class.
		/// A server will be created on the local machine on port 19002.
		/// </summary>
		public NetworkPokeServer()
		{
			var hostIp = IPAddress.Any;
			var hostEndpoint = new IPEndPoint(hostIp, Port);
			_listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			// Bind to our local endpoint
			_listener.Bind(hostEndpoint);
			_listener.Listen(128); // Listen with a pending connection queue size of 128

			// Discover UPnP support
			NatUtility.DeviceFound += DeviceFound;
			NatUtility.StartDiscovery();
		}

		/// <summary>
		/// Updates the state of the server and waits for a command to become available.
		/// The first command that is available will be passed into a handler.
		/// </summary>
		/// <param name="handler">The <see cref="IPokeCommandHandler"/> to handle the command with.</param>
		public void ReceiveCommand(IPokeCommandHandler handler)
		{
			// Loop until a command is processed
			while (true)
			{
				// Duplicate our clients list for use with Socket.Select()
				List<ClientModel> readyClients;
				lock (_clients)
				{
					readyClients = new List<ClientModel>(_clients);
				}

				// The listener socket is "readable" when a client is ready to be accepted
				readyClients.Add(new ClientModel(null, _listener));

				// Wait for either a command to become available in a client,
				// or a client to be ready to connect
				List<Socket> sockets = readyClients.Select(client => client.ClientSocket).ToList();
				Socket.Select(sockets, null, null, -1);
				var failedClients = new List<ClientModel>();
				foreach (var client in readyClients)
				{

					if (client.ClientSocket != _listener)
					{
						try
						{
							// Command available
							using (var stream = new NetworkStream(client.ClientSocket, false))
							{
								var command = CommandSerialization.DeserializeCommand(stream);
								command.Source = client;
								command.Handle(handler);
							}
						}
						catch (IOException)
						{
							client.ClientSocket.Close();
							failedClients.Add(client);
						}
						break; // Only process one command at a time
					}
					else
					{
						// Client ready to connect
						var newClient = _listener.Accept();
						ConnectClient(newClient);
					}
				}
				foreach (var client in failedClients)
					_clients.Remove(client);
			}
		}

		/// <summary>
		/// Sends a command to all connected clients.
		/// </summary>
		/// <param name="command">The command to send.</param>
		public void SendCommandToAll(PokeCommand command)
		{
			lock (_clients)
			{
				var failedClients = new List<ClientModel>();
				foreach (var client in _clients)
				{
					try
					{
						using (var stream = new NetworkStream(client.ClientSocket, false))
							CommandSerialization.SerializeCommand(command, stream);
					}
					catch (IOException)
					{
						client.ClientSocket.Close();
						failedClients.Add(client);
					}
				}
				foreach (var client in failedClients)
					_clients.Remove(client);

			}
		}

		/// <summary>
		/// Connects a new client to the server.
		/// </summary>
		/// <param name="client">The client to connect.</param>
		private void ConnectClient(Socket client)
		{
			lock (_clients)
			{
				// wait for client to become connected
				// seems haxish
				while (!client.Connected) {}
				_clients.Add(new ClientModel(client.RemoteEndPoint.ToString(), client));
				
			}
			var clientStrings = _clients.Select(localClient => localClient.Name).ToList();
			SendCommandToAll(new ClientListCommand(clientStrings));
		}

		/// <summary>
		/// Callback for when a UPnP device is found.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DeviceFound(object sender, DeviceEventArgs e)
		{
			// Create a UPnP mapping for our port
			var device = e.Device;
			var map = new Mapping(Protocol.Tcp, Port, Port);
			map.Description = UpnpDescription;
			device.CreatePortMap(map);

#if DEBUG
			Debug.WriteLine("UPnP found device: " + device.GetExternalIP());
#endif
		}

		public List<ClientModel> GetClients()
		{
			return _clients;
		}
	}
}
