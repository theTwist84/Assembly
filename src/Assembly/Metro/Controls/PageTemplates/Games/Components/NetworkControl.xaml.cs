using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Assembly.Helpers;
using Assembly.Helpers.Net.Sockets;
using Assembly.Metro.Dialogs;
using Blamite.RTE;
using XBDMCommunicator;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Net.Sockets;

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

			new Thread(new ThreadStart(delegate
			{
				while (true)
				{
					if (_handler != null && _handler.GetClientIpList() != null)
						_viewModel.Clients = new ObservableCollection<string>(_handler.GetClientIpList());
					Thread.Sleep(5000);
				}
			})).Start();
		}

		private void StartServer_click(object sender, RoutedEventArgs e)
		{
			_handler = new ServerCommandHandler(_map);
			_rteProvider = new SocketRTEProvider(_handler, RTEConnectionType.ConsoleX360);
			_map.setRTEProvider(_rteProvider);
		}

		private void StartClient_click(object sender, RoutedEventArgs e)
		{
			try
			{
				_handler = new ClientCommandHandler(TextBlock1.Text, _map);
				_rteProvider = new SocketRTEProvider(_handler, RTEConnectionType.ConsoleX360);
				_map.setRTEProvider(_rteProvider);
			}
			catch (FormatException)
			{

				MetroMessageBox.Show("Invalid IP", "The specified IP address was not valid.");
			}
		}

		private void Unfreeze_click(object sender, RoutedEventArgs e)
		{
			_handler.StartFreezeCommand(new FreezeCommand(false));
		}

		private void Freeze_click(object sender, RoutedEventArgs e)
		{
			_handler.StartFreezeCommand(new FreezeCommand(true));
		}

		private void SendPoke_click(object sender, RoutedEventArgs e)
		{
			var offset = TextBoxOffset.Text.StartsWith("0x")
				? UInt32.Parse(TextBoxOffset.Text.Replace("0x", ""), System.Globalization.NumberStyles.HexNumber)
				: UInt32.Parse(TextBoxOffset.Text);
			var data = VariousFunctions.StringToByteArray(TextBoxData.Text);
			_handler.StartMemoryCommand(new MemoryCommand(offset, data));
		}

		private void Disconnect_click(object sender, RoutedEventArgs e)
		{
			_map.setRTEProvider(new XBDMRTEProvider(App.AssemblyStorage.AssemblySettings.Xbdm));
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

