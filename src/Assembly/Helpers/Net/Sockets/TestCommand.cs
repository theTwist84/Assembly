using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Blamite.IO;

namespace Assembly.Helpers.Net.Sockets
{
    public class TestCommand : PokeCommand
    {
        public TestCommand() : base(PokeCommandType.Test)
        {
        }

        public TestCommand(string message) : base(PokeCommandType.Test)
        {
            Message = message;
        }


        public string Message { get; set; }

        public override void Deserialize(Stream stream)
        {
			using (var reader = new EndianReader(stream, Endian.BigEndian, false))
			{
				var length = reader.ReadInt32();
				Message = reader.ReadAscii(length + 1);
			}
        }

        public override void Serialize(Stream stream)
        {
			using (var writer = new EndianWriter(stream, Endian.BigEndian, false))
			{
				writer.WriteInt32(Message.Length);
				writer.WriteAscii(Message);
			}
        }

        public override void Handle(IPokeCommandHandler handle)
        {
            handle.HandleTestCommand(this);
        }
    }
}
