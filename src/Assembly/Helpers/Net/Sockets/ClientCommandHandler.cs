using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Diagnostics;

using Assembly.Metro.Dialogs;
using System.Windows.Threading;

namespace Assembly.Helpers.Net.Sockets
{
    public class ClientCommandHandler: IPokeCommandHandler
    {
        private NetworkPokeClient _client;
		private List<string> _clientList = new List<string>();
        public ClientCommandHandler(string IpAddress)
        {
            _client = new NetworkPokeClient(IPAddress.Parse(IpAddress));
             var thread = new Thread(new ThreadStart(delegate
             {
                 while (true)
                 {
					 bool succeeded = _client.ReceiveCommand(this);
					 if (!succeeded)
					 {
						 App.AssemblyStorage.AssemblySettings.HomeWindow.Dispatcher.Invoke(
							 new Action(delegate { MetroMessageBox.Show("Server lost", "Connection to the server has been lost"); }));
						 return;
					 }
                 }
             }));
             thread.Start();         
        }

        public void HandleFreezeCommand(FreezeCommand freeze)
        {
            HandlerFunctions.FreezeCommand(freeze);
        }

        public void HandleMemoryCommand(MemoryCommand memory)
        {
            HandlerFunctions.MemoryCommand(memory);
        }

        public void StartFreezeCommand(FreezeCommand freeze)
        {
            var succeeded = _client.SendCommand(freeze);
            if (!succeeded)
            {
                App.AssemblyStorage.AssemblySettings.HomeWindow.Dispatcher.Invoke(
                    new Action(delegate { MetroMessageBox.Show("Command Failed", "A freeze command has failed to be sent."); }));
            }
        }

        public void StartMemoryCommand(MemoryCommand memory)
        {
            var succeeded = _client.SendCommand(memory);
			if (!succeeded)
			{
				App.AssemblyStorage.AssemblySettings.HomeWindow.Dispatcher.Invoke(
					new Action(delegate { MetroMessageBox.Show("Command Failed", "A poke has failed to be sent to the server."); }));
			}
        }

		public List<string> GetClientIpList()
		{
			return _clientList;
		}


		public void HandleClientListCommand(ClientListCommand clientListCommand)
		{
			_clientList = clientListCommand.Clients;
		}
	}
}
