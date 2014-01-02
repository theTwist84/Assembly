using System;
using System.IO;
using System.Text;
using System.Windows.Documents;
using Blamite.IO;

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Assembly.Helpers.Net.Sockets
{
    public class MemoryCommand : PokeCommand
    {
        public List<NetworkPokeStream.PokeBuffer> Buffers { get; set; }

        public MemoryCommand()
            : base(PokeCommandType.Memory)
        {
            Buffers = new List<NetworkPokeStream.PokeBuffer>();
        }

        public MemoryCommand(List<NetworkPokeStream.PokeBuffer> buffers) 
            : base(PokeCommandType.Memory)
        {
            Buffers = buffers;
        }

        public MemoryCommand(uint offset, byte[] buffer)
            : base(PokeCommandType.Memory)
        {
            Buffers = new List<NetworkPokeStream.PokeBuffer>();
            Buffers.Add(new NetworkPokeStream.PokeBuffer(offset, buffer));
        }

        public override void Deserialize(Stream stream)
        {
            using (var reader = new EndianReader(stream, Endian.BigEndian, false))
            {
                var count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    var offset = reader.ReadUInt32();
                    var length = reader.ReadInt32();
                    var buffer = reader.ReadBlock(length);
                    Buffers.Add(new NetworkPokeStream.PokeBuffer(offset, buffer));
                }
            }
        }

        public override void Serialize(Stream stream)
        {
			using (var writer = new EndianWriter(stream, Endian.BigEndian, false))
			{
			    var count = Buffers.Count;
                writer.WriteInt32(count);
			    foreach (var buffer in Buffers)
			    {
					writer.WriteUInt32(buffer.Position);
					writer.WriteInt32(buffer.Buffer.Length);
					writer.WriteBlock(buffer.Buffer);
			    }
			}
        }

        public override void Handle(IPokeCommandHandler handle)
        {
            handle.HandleMemoryCommand(this);
        }
    }
}