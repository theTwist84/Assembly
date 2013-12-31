using Blamite.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Assembly.Helpers.Net.Sockets
{
	public class ClientListCommand : PokeCommand
	{
		public List<string> Clients { get; private set; }

		public ClientListCommand()
			: base(PokeCommandType.ClientList)
		{
			Clients = new List<string>();
		}

		public ClientListCommand(List<ClientModel> clients)
			: base(PokeCommandType.ClientList)
		{
			Clients = new List<string>();
			Clients.Add("Server");
			foreach (var client in clients)
			{
				Clients.Add(client.Name);
			}
		}


		public override void Deserialize(System.IO.Stream stream)
		{
			using (var reader = new EndianReader(stream, Endian.BigEndian, false))
			{
				var numClients = reader.ReadInt32();
				for (int i = 0; i < numClients; i++)
				{
					var length = reader.ReadInt32();
					var bytes = reader.ReadBlock(length);
					Clients.Add(Encoding.ASCII.GetString(bytes));
				}
			}
		}

		public override void Serialize(System.IO.Stream stream)
		{
			using (var writer = new EndianWriter(stream, Endian.BigEndian, false))
			{
				writer.WriteInt32(Clients.Count);
				foreach (var client in Clients)
				{
					var clientBlock = Encoding.ASCII.GetBytes(client);
					writer.WriteInt32(client.Length);
					writer.WriteBlock(clientBlock);
				}
			}
		}

		public override void Handle(IPokeCommandHandler handle)
		{
			handle.HandleClientListCommand(this);
		}
	}
}
