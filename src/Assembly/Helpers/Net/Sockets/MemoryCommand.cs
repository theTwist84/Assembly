using System;
using System.IO;
using System.Text;
using Blamite.IO;

namespace Assembly.Helpers.Net.Sockets
{
    public class MemoryCommand : PokeCommand
    {
		public uint Offset { get; set; }
		public byte[] Data { get; set; }

        public MemoryCommand()
            : base(PokeCommandType.Memory)
        {
            
        }

        public MemoryCommand(uint offset, byte[] data) 
            : base(PokeCommandType.Memory)
        {
			Offset = offset;
			Data = data;
        }

        public override void Deserialize(Stream stream)
        {
			using (var reader = new EndianReader(stream, Endian.BigEndian, false))
			{
				Offset = reader.ReadUInt32();
				var size = reader.ReadInt32();
				Data = reader.ReadBlock(size);
			}
        }

        public override void Serialize(Stream stream)
        {
			using (var writer = new EndianWriter(stream, Endian.BigEndian, false))
			{
				writer.WriteUInt32(Offset);
				writer.WriteInt32(Data.Length);
				writer.WriteBlock(Data);
			}
        }

        public override void Handle(IPokeCommandHandler handle)
        {
            handle.HandleMemoryCommand(this);
        }
    }
}