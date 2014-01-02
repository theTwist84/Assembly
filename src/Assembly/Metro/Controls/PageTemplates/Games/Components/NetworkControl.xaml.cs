using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Assembly.Helpers;
using Assembly.Helpers.Net.Sockets;
using Assembly.Metro.Dialogs;
using Blamite.Blam;
using Blamite.RTE;
using Blamite.RTE.H2Vista;
using XBDMCommunicator;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Assembly.Metro.Controls.PageTemplates.Games.Components
{
	/// <summary>
	/// Interaction logic for NetworkControl.xaml
	/// </summary>
	public partial class NetworkControl : UserControl
	{
		private IPokeCommandHandler _handler;
		private IRTEProvider _rteProvider;
		private HaloMap _map;
		private ViewModel _viewModel;

		public NetworkControl(HaloMap map)
		{
			_map = map;
			InitializeComponent();
			DataContext = _viewModel = new ViewModel();
		}

		private void StartServer_click(object sender, RoutedEventArgs e)
		{
			_handler = new ServerCommandHandler(_map, _viewModel);
			_rteProvider = SelectRteEngine(_map.CacheFile, _handler);
			_map.RteProvider = _rteProvider;
		}

		private void StartClient_click(object sender, RoutedEventArgs e)
		{
			try
			{
				_handler = new ClientCommandHandler(TextBlock1.Text, _map, _viewModel);
				_rteProvider = SelectRteEngine(_map.CacheFile, _handler);
				_map.RteProvider = _rteProvider;
			}
			catch (FormatException)
			{
				MetroMessageBox.Show("Invalid IP", "The specified IP address was not valid.");
			}
		}

		private IRTEProvider SelectRteEngine(ICacheFile cache, IPokeCommandHandler handler)
		{
			if (_map.CacheFile.Engine == EngineType.ThirdGeneration)
				return new SocketRTEProvider(_handler, RTEConnectionType.ConsoleX360);
			else
				throw new NotImplementedException("No Halo 2 Vista poking allowed yet.");
		}

		private void Unfreeze_click(object sender, RoutedEventArgs e)
		{
			_handler.StartFreezeCommand(new FreezeCommand(false));
		}

		private void Freeze_click(object sender, RoutedEventArgs e)
		{
			_handler.StartFreezeCommand(new FreezeCommand(true));
		}

		private void SendName_Click(object sender, RoutedEventArgs e)
		{
			_handler.StartNameChangeCommand(new ChangeNameCommand(TextBlock1.Text));
		}

		private void Disconnect_click(object sender, RoutedEventArgs e)
		{
			_map.RteProvider = new XBDMRTEProvider(App.AssemblyStorage.AssemblySettings.Xbdm);
			//TODO:  Make the sockets close
			_handler = null;
		}

		public class ViewModel : INotifyPropertyChanged
		{
			public ViewModel()
			{
				_clients.CollectionChanged += ClientsOnCollectionChanged;
			}

			#region Events

			void ClientsOnCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
			{
				Clients = (ObservableCollection<string>)sender;
			}

			#endregion

			public ObservableCollection<string> Clients
			{
				get { return _clients; }
				set { SetField(ref _clients, value, "Clients"); }
			}
			private ObservableCollection<string> _clients = new ObservableCollection<string>();

			#region Binding Stuff

			public event PropertyChangedEventHandler PropertyChanged;
			protected virtual void OnPropertyChanged(string propertyName)
			{
				if (PropertyChanged != null)
					PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
			protected bool SetField<T>(ref T field, T value, string propertyName)
			{
				if (EqualityComparer<T>.Default.Equals(field, value)) return false;
				field = value;
				OnPropertyChanged(propertyName);
				return true;
			}

			#endregion
		}
	}
}

