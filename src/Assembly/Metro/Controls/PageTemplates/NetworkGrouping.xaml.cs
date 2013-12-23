using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Assembly.Helpers;
using Assembly.Helpers.Net;
using Assembly.Helpers.Net.Sockets;
using Assembly.Helpers.UIX;
using Assembly.Metro.Dialogs;


namespace Assembly.Metro.Controls.PageTemplates
{
	/// <summary>
	///     Interaction logic for NetworkGrouping.xaml
	/// </summary>
	public partial class NetworkGrouping :  IPokeCommandHandler
	{
	    private NetworkPokeClient _client;
	    private NetworkPokeServer _server;

	    public NetworkGrouping()
		{
			InitializeComponent();

			gridNetworkType.Visibility = gridSignedIn.Visibility = Visibility.Collapsed;
			gridSignIn.Visibility = Visibility.Visible;
		}

		private void btnSignIn_Click(object sender, RoutedEventArgs e)
		{
			var credentials = new UserCredentials();
			credentials.Username = txtAccountUsername.Text;
			credentials.Password = txtAccountPassword.Password;

			var thrd = new Thread(SignInThread);
			thrd.SetApartmentState(ApartmentState.STA);
			thrd.Start(credentials);
		}

		private void SignInThread(object credentialsObj)
		{
			// Try to Sign In
			try
			{
				var credentials = (UserCredentials) credentialsObj;

				// Send Signin Package to Server        
				SignInResponse response = SignIn.AttemptSignIn(credentials.Username, credentials.Password);
				if (response == null || !response.Successful)
				{
					Dispatcher.Invoke(new Action(delegate { MetroMessageBox.Show("Unable to Sign In", response.ErrorMessage); }));
					return;
				}

				// Set the UI to show that we have signed in successfully
				Dispatcher.Invoke(new Action(delegate
				{
					gridNetworkType.Visibility = gridSignedIn.Visibility = Visibility.Visible;
					gridSignIn.Visibility = Visibility.Collapsed;
					lblSignedInWelcome.Text = response.DisplayName.ToLower();
					lblSignedInPosts.Text = string.Format("posts: {0:##,###}", response.PostCount);

					// Validate Avatar
					if (!string.IsNullOrEmpty(response.AvatarUrl) && response.AvatarUrl != "http://uploads.xbchaos.netdna-cdn.com/")
						ImageLoader.LoadImageAndFade(imgSignedInAvatar, new Uri(response.AvatarUrl), new AnimationHelper(this));

					MetroMessageBox.Show("welcome", "Welcome to network poking, " + response.DisplayName);
				}));
			}
			catch (Exception ex)
			{
				Dispatcher.Invoke(new Action(delegate { MetroException.Show(ex); }));
			}
		}

		public bool Close()
		{
			// Remove all session data, then close this mofo

			return true;
		}

		private class UserCredentials
		{
			public string Username { get; set; }
			public string Password { get; set; }
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
	    }

	    public void HandleMemoryCommand(MemoryCommand memory)
	    {
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