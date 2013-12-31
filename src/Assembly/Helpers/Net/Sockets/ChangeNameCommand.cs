using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Blamite.IO;

namespace Assembly.Helpers.Net.Sockets
{
	public class ChangeNameCommand : PokeCommand
	{
		public ChangeNameCommand()
			: base(PokeCommandType.ChangeName)
		{
			
		}

		public ChangeNameCommand(string name)
			: base(PokeCommandType.ChangeName)
		{
			Name = name;
		}

		public override void Deserialize(Stream stream)
		{
			using (var reader = new EndianReader(stream, Endian.BigEndian, false))
			{
				var length = reader.ReadInt32();
				var bytes = reader.ReadBlock(length);
				Name = Encoding.ASCII.GetString(bytes);
			}
		}

		public override void Serialize(Stream stream)
		{
			using (var writer = new EndianWriter(stream, Endian.BigEndian, false))
			{
				writer.WriteInt32(Name.Length);
				var bytes = Encoding.ASCII.GetBytes(Name);
				writer.WriteBlock(bytes);
			}
		}

		public override void Handle(IPokeCommandHandler handle)
		{
			handle.HandleChangeNameCommand(this);
		}

		public string Name { get; set; }
	}
}
