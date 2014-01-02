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
        public List<SocketStream.PokeBuffer> Models { get; set; }

        public MemoryCommand()
            : base(PokeCommandType.Memory)
        {
            Models = new List<SocketStream.PokeBuffer>();
        }

        public MemoryCommand(List<SocketStream.PokeBuffer> models) 
            : base(PokeCommandType.Memory)
        {
            Models = models;
        }

        public MemoryCommand(uint offset, byte[] buffer)
            : base(PokeCommandType.Memory)
        {
            Models = new List<SocketStream.PokeBuffer>();
            Models.Add(new SocketStream.PokeBuffer(offset, buffer));
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
                    Models.Add(new SocketStream.PokeBuffer(offset, buffer));
                }
            }
        }

        public override void Serialize(Stream stream)
        {
			using (var writer = new EndianWriter(stream, Endian.BigEndian, false))
			{
			    var count = Models.Count;
                writer.WriteInt32(count);
			    foreach (var model in Models)
			    {
			        writer.WriteUInt32(model.Position);
                    writer.WriteInt32(model.Buffer.Length);
                    writer.WriteBlock(model.Buffer);
			    }
			}
        }

        public override void Handle(IPokeCommandHandler handle)
        {
            handle.HandleMemoryCommand(this);
        }
    }
}