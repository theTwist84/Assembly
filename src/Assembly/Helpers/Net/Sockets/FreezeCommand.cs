using System;
using System.IO;

namespace Assembly.Helpers.Net.Sockets
{
    public class FreezeCommand : PokeCommand
    {
        public bool Freeze = true;

        public FreezeCommand() :
            base(PokeCommandType.Freeze)
        {
            
        }

        public FreezeCommand(bool freeze) : 
            base(PokeCommandType.Freeze)
        {
            Freeze = freeze;
        }

        public override void Deserialize(Stream stream)
        {
            Freeze = Convert.ToBoolean(stream.ReadByte());
        }

        public override void Serialize(Stream stream)
        {
            stream.WriteByte(Convert.ToByte(Freeze));
        }

        public override void Handle(IPokeCommandHandler handle)
        {
	        var xbdm = App.AssemblyStorage.AssemblySettings.Xbdm;
            if(Freeze)
                xbdm.Freeze();
            if(!Freeze)
                xbdm.Unfreeze();
            handle.HandleFreezeCommand(this);
        }
    }
}

