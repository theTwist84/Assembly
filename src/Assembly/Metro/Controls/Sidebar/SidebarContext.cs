using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Assembly.Metro.Controls.Sidebar
{
	public class SidebarContext : INotifyPropertyChanged
	{

		public SidebarContext()
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
