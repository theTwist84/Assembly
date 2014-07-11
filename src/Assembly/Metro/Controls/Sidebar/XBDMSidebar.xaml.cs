using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using Assembly.Helpers.Net.Sockets;
using Assembly.Metro.Controls.PageTemplates;
using Assembly.Metro.Controls.PageTemplates.Games;
using Assembly.Metro.Controls.PageTemplates.Games.Components;
using Assembly.Metro.Dialogs;
using AvalonDock.Layout;
using Blamite.Blam;
using Blamite.RTE;
using XBDMCommunicator;

namespace Assembly.Metro.Controls.Sidebar
{
	/// <summary>
	///     Interaction logic for XBDMSidebar.xaml
	/// </summary>
	public partial class XbdmSidebar
	{

		public XbdmSidebar()
		{
			InitializeComponent();
			if (App.AssemblyStorage.Handler != null)
				App.AssemblyStorage.Handler.SetSidebarContext((SidebarContext) DataContext);
		}

		private void btnScreenshot_Click(object sender, RoutedEventArgs e)
		{
			string screenshotFileName = Path.GetTempFileName();

			if (App.AssemblyStorage.AssemblySettings.Xbdm.GetScreenshot(screenshotFileName))
					App.AssemblyStorage.AssemblySettings.HomeWindow.AddScrenTabModule(screenshotFileName);
			else
				MetroMessageBox.Show("Not Connected", "You are not connected to a debug Xbox 360.");
		}

		private void btnFreeze_Click(object sender, RoutedEventArgs e)
		{
			App.AssemblyStorage.AssemblySettings.Xbdm.Freeze();
		}

		private void btnUnfreeze_Click(object sender, RoutedEventArgs e)
		{
			App.AssemblyStorage.AssemblySettings.Xbdm.Unfreeze();
		}

		private void btnRebootTitle_Click(object sender, RoutedEventArgs e)
		{
			App.AssemblyStorage.AssemblySettings.Xbdm.Reboot(Xbdm.RebootType.Title);
		}

		private void btnRebootCold_Click(object sender, RoutedEventArgs e)
		{
			App.AssemblyStorage.AssemblySettings.Xbdm.Reboot(Xbdm.RebootType.Cold);
		}

		private void StartServer_click(object sender, RoutedEventArgs e)
		{
			/*ObservableCollection<LayoutContent> tabs = App.AssemblyStorage.AssemblySettings.HomeWindow.documentManager.Children;
			_map = (from tab in tabs where tab.Content is HaloMap select (HaloMap) tab.Content).FirstOrDefault();
			if (_map != null)
			{
				_map.Handler = new ServerCommandHandler(_map, (SidebarContext) DataContext);
				App.AssemblyStorage.Handler = _map.Handler;
				
				_map.RteProvider = SelectRteEngine(_map.CacheFile, _map.Handler);
			}*/

			App.AssemblyStorage.Handler = new ServerCommandHandler((SidebarContext) DataContext);
		}



		private void SendName_Click(object sender, RoutedEventArgs e)
		{
			if (App.AssemblyStorage.Handler != null)
				App.AssemblyStorage.Handler.StartNameChangeCommand(new ChangeNameCommand(svrAddressBox.Text));
		}

	}
}