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

namespace Assembly.Metro.Controls.PageTemplates.Games.Components
{
    /// <summary>
    /// Interaction logic for NetworkControl.xaml
    /// </summary>
    public partial class NetworkControl : UserControl, IPokeCommandHandler
    {
        private NetworkPokeClient _client;
        private NetworkPokeServer _server;
        private IRTEProvider _rteProvider;
        private HaloMap _map;

        public NetworkControl(HaloMap map)
        {
            _map = map;
            InitializeComponent();
        }

        private void StartServer_click(object sender, RoutedEventArgs e)
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

        private void StartClient_click(object sender, RoutedEventArgs e)
        {
            try
            {
                _client = new NetworkPokeClient(IPAddress.Parse(TextBlock1.Text));
                var thread = new Thread(new ThreadStart(delegate
                {
                    while (true)
                    {
                        _client.ReceiveCommand(this);
                    }
                }));
                thread.Start();
                _rteProvider = new SocketRTEProvider(_client, RTEConnectionType.ConsoleX360);
                _map.StartNetworkPoke(_rteProvider);
                
            }
            catch (FormatException formatException)
            {
                MetroMessageBox.Show("Invalid IP", "The specified IP address was not valid.");
            }
        }

        private void SendCommand_click(object sender, RoutedEventArgs e)
        {
            _client.SendCommand(new TestCommand(TextBlock1.Text));
        }

        public void HandleTestCommand(TestCommand test)
        {
            Dispatcher.Invoke(new Action(delegate { TextBlockOutput.Text = test.Message; }));
        }

        public void HandleFreezeCommand(FreezeCommand freeze)
        {
			var xbdm = App.AssemblyStorage.AssemblySettings.Xbdm;
			if (freeze.Freeze)
				xbdm.Freeze();
			else
				xbdm.Unfreeze();

            if (freeze.Freeze)
                Debug.WriteLine("Console Froze");
            else
                Debug.WriteLine("Console Unfroze");
        }

        public void HandleMemoryCommand(MemoryCommand memory)
        {
			var xbdm = App.AssemblyStorage.AssemblySettings.Xbdm;
			if (xbdm != null)
			{
				xbdm.MemoryStream.Seek(memory.Offset, SeekOrigin.Begin);
				xbdm.MemoryStream.Write(memory.Data, 0, memory.Data.Length);
			}

            Debug.WriteLine("Poked '{0}' to offset 0x{1:X8}", memory.Data, memory.Offset);
        }

        private void Unfreeze_click(object sender, RoutedEventArgs e)
        {
            _client.SendCommand(new FreezeCommand(false));
        }

        private void Freeze_click(object sender, RoutedEventArgs e)
        {
            _client.SendCommand(new FreezeCommand(true));
        }

        private void SendPoke_click(object sender, RoutedEventArgs e)
        {
            var offset = TextBoxOffset.Text.StartsWith("0x")
                ? UInt32.Parse(TextBoxOffset.Text.Replace("0x", ""), System.Globalization.NumberStyles.HexNumber)
                : UInt32.Parse(TextBoxOffset.Text);
            var data = VariousFunctions.StringToByteArray(TextBoxData.Text);
            _client.SendCommand(new MemoryCommand(offset, data));
        }
    }
}

