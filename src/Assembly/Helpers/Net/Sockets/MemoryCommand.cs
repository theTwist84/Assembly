using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;


namespace Assembly.Helpers.Net.Sockets
{
    public class MemoryCommand : PokeCommand
    {
        public Model DataModel { get; private set; }

        public MemoryCommand()
            : base(PokeCommandType.Memory)
        {
            
        }

        public MemoryCommand(uint offset, byte[] data) 
            : base(PokeCommandType.Memory)
        {
            DataModel = new Model {Data = data, Offset = offset};
        }

        public override void Deserialize(Stream stream)
        {
            var lengthData = new byte[4];
            stream.Read(lengthData, 0, 4);
            var length = BitConverter.ToInt32(lengthData, 0);
            var buffer = new byte[length];
            while (length > 0)
            {
                var read = stream.Read(buffer, 0, length);
                length -= read;
            }
            var jsonString = Encoding.ASCII.GetString(buffer);
            DataModel = JsonConvert.DeserializeObject<Model>(jsonString);
        }

        public override void Serialize(Stream stream)
        {
            var jsonData = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(DataModel));
            var dataLength = jsonData.Length;
            stream.Write(BitConverter.GetBytes(dataLength), 0, 4);
            stream.Write(jsonData, 0, jsonData.Length);
        }

        public override void Handle(IPokeCommandHandler handle)
        {
            var xbdm = App.AssemblyStorage.AssemblySettings.Xbdm;
            if (xbdm != null)
            {
                xbdm.MemoryStream.Seek(DataModel.Offset, SeekOrigin.Begin);
                xbdm.MemoryStream.Write(DataModel.Data, 0, DataModel.Data.Length);
            }
            handle.HandleMemoryCommand(this);
        }

        public class Model
        {
            public uint Offset { get; set; }
            public byte[] Data { get; set; }
        }
    }
}