using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Assembly.Helpers.Net.Sockets
{
	public class ClientModel
    {
        

        public ClientModel()
        {       
        }

        public ClientModel(string name, Socket socket)
        {
            Name = name;
	        ClientSocket = socket;
        }

		public Socket ClientSocket { get; set; }

		public string Name { get; set; }
	}
}
