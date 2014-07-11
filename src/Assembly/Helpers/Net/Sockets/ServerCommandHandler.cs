using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Assembly.Metro.Controls.PageTemplates.Games;
using Assembly.Metro.Controls.PageTemplates.Games.Components;
using Assembly.Metro.Controls.Sidebar;
using AvalonDock.Layout;
using Blamite.Blam;
using Blamite.RTE;
using Xceed.Wpf.DataGrid.Converters;

namespace Assembly.Helpers.Net.Sockets
{
	public class ServerCommandHandler : IPokeCommandHandler
	{
		private NetworkPokeServer _server;
		private List<string> _clientList;
		private string _name = "Server";
	    private HaloMap _map;
		private SidebarContext _viewModel;
		private Blamite.RTE.IRTEProvider _provider;
		private bool _shouldEnd = false;
		private SidebarContext _context;


		public ServerCommandHandler(HaloMap map, SidebarContext viewModel)
		{
		    _map = map;
		    _viewModel = viewModel;
			_provider = _map.RteProvider;
			_server = new NetworkPokeServer();
			HandleClientListCommand(new ClientListCommand(GetClientNameList()));
			var thread = new Thread(new ThreadStart(delegate
			{
				while (!_shouldEnd)
				{
				    _server.ReceiveCommand(this);
				}
			}));
			thread.Start();
		}

		public ServerCommandHandler(SidebarContext context)
		{
			_server = new NetworkPokeServer();
			_context = context;

			ObservableCollection<LayoutContent> tabs = App.AssemblyStorage.AssemblySettings.HomeWindow.documentManager.Children;
			HandleClientListCommand(new ClientListCommand(GetClientNameList()));

			HaloMap map = (from tab in tabs where tab.Content is HaloMap select (HaloMap) tab.Content).FirstOrDefault();
			if (map != null)
			{
				map.RteProvider = SelectRteEngine(map.CacheFile, map.Handler);
			}

			var thread = new Thread(new ThreadStart(delegate
			{
				while (!_shouldEnd)
				{
					_server.ReceiveCommand(this);
				}
			}));
			thread.Start();


		}

		private IRTEProvider SelectRteEngine(ICacheFile cache, IPokeCommandHandler handler)
		{
			if (cache.Engine == EngineType.ThirdGeneration)
				return new SocketRTEProvider(this, RTEConnectionType.ConsoleX360);
			else
				return new SocketRTEProvider(this, RTEConnectionType.LocalProcess);
		}

		public void HandleFreezeCommand(FreezeCommand freeze)
		{
			HandlerFunctions.FreezeCommand(freeze);
			_server.SendCommandToAll(freeze);
		}

		public void HandleMemoryCommand(MemoryCommand memory)
		{
			ObservableCollection<LayoutContent> tabs = App.AssemblyStorage.AssemblySettings.HomeWindow.documentManager.Children;
			HaloMap map = (from tab in tabs where tab.Content is HaloMap select (HaloMap)tab.Content).FirstOrDefault();

			//if (map.CacheFile.Engine == EngineType.ThirdGeneration)
				//HandleFreezeCommand(new FreezeCommand(true));
			_server.SendCommandToAll(memory);
			// Everything gets passed here
			//using (var stream = map.RteProvider.GetMetaStream(map.CacheFile))
				//HandlerFunctions.MemoryCommand(memory, map, stream);
			//if (_map.CacheFile.Engine == EngineType.ThirdGeneration)
				//HandleFreezeCommand(new FreezeCommand(false));
		}

		public void StartFreezeCommand(FreezeCommand freeze)
		{
			HandleFreezeCommand(freeze);
		}

		public void StartMemoryCommand(MemoryCommand memory)
		{
			HandleMemoryCommand(memory);
			return;
		}

		public void StartNameChangeCommand(ChangeNameCommand changeNameCommand)
		{
			_name = changeNameCommand.Name;
			HandleClientListCommand(new ClientListCommand(GetClientNameList()));
		}

		public void SetSidebarContext(SidebarContext context)
		{
			_context = context;
		}

		public List<ClientName> GetClientList()
		{
			return _server.GetClients();
		} 

		public List<string> GetClientNameList()
		{
			_clientList = new List<string>();
			_clientList.Add(_name);
			foreach (var client in GetClientList())
			{
				_clientList.Add(client.Name);
			}
			_context.Clients = new ObservableCollection<string>(_clientList);
			return _clientList;
		}

		public void HandleChangeNameCommand(ChangeNameCommand changeNameCommand)
		{
			changeNameCommand.Source.Name = changeNameCommand.Name;
			HandleClientListCommand(new ClientListCommand(GetClientNameList()));
		}

		public void HandleClientListCommand(ClientListCommand clientListCommand)
		{
			_server.SendCommandToAll(new ClientListCommand(GetClientNameList()));
		}

		public void StartTermination()
		{
			_shouldEnd = true;
			_server.ResetCallBack();
		}

	}
}
