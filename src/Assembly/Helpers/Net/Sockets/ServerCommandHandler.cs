using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Assembly.Helpers.Net.Sockets
{
	public class ServerCommandHandler : IPokeCommandHandler
	{
		private NetworkPokeServer _server;
		private List<string> _clientList;
		public ServerCommandHandler()
		{
			_server = new NetworkPokeServer();
			var thread = new Thread(new ThreadStart(delegate
			{
				while (true)
				{
					_server.ReceiveCommand(this);	
				}
			}));
			thread.Start();
		}

		public void HandleFreezeCommand(FreezeCommand freeze)
		{
			HandlerFunctions.FreezeCommand(freeze);
			_server.SendCommandToAll(freeze);
		}

		public void HandleMemoryCommand(MemoryCommand memory)
		{
			HandlerFunctions.MemoryCommand(memory);
			_server.SendCommandToAll(memory);
		}

		public void StartFreezeCommand(FreezeCommand freeze)
		{
			HandleFreezeCommand(freeze);
		}

		public void StartMemoryCommand(MemoryCommand memory)
		{
			HandleMemoryCommand(memory);
		}

		public List<Socket> GetClientList()
		{
			return _server.GetClients();
		} 

		public List<string> GetClientIpList()
		{
			_server.SendCommandToAll(new ClientListCommand(GetClientList()));
			return _clientList;
		}

		public void HandleClientListCommand(ClientListCommand clientListCommand)
		{
			_clientList = clientListCommand.Clients;
		}
	}
}
