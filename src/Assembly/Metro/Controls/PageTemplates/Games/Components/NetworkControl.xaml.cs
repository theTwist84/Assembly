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
    public partial class NetworkControl : UserControl
    {
        private IPokeCommandHandler _handler;
        private IRTEProvider _rteProvider;
        private HaloMap _map;

        public NetworkControl(HaloMap map)
        {
            _map = map;
            InitializeComponent();
        }

        private void StartServer_click(object sender, RoutedEventArgs e)
        {
			_handler = new ServerCommandHandler();
			_rteProvider = new SocketRTEProvider(_handler, RTEConnectionType.ConsoleX360);
			_map.StartNetworkPoke(_rteProvider);
        }

        private void StartClient_click(object sender, RoutedEventArgs e)
        {
            try
            {
                _handler = new ClientCommandHandler(TextBlock1.Text);
                _rteProvider = new SocketRTEProvider(_handler, RTEConnectionType.ConsoleX360);
                _map.StartNetworkPoke(_rteProvider);     
            }
            catch (FormatException formatException)
            {
                MetroMessageBox.Show("Invalid IP", "The specified IP address was not valid.");
            }
        }

        private void SendCommand_click(object sender, RoutedEventArgs e)
        {
            _handler.StartTestCommand(new TestCommand(TextBlock1.Text));
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
    }
}

