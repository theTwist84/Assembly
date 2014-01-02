using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Assembly.Metro.Controls.PageTemplates.Games;
using Blamite.Blam;
using Blamite.IO;
using Blamite.Native;

namespace Assembly.Helpers.Net.Sockets
{
    public static class HandlerFunctions
    {
        public static void FreezeCommand(FreezeCommand freeze)
        {
            var xbdm = App.AssemblyStorage.AssemblySettings.Xbdm;
            if (freeze.Freeze)
                xbdm.Freeze();
            else
                xbdm.Unfreeze();

            if (freeze.Freeze)
                Debug.WriteLine("Console Froze");
            else
                Debug.WriteLine("Console Unfroze");
        }

	    public static void MemoryCommand(MemoryCommand memory, HaloMap map, IStream stream)
	    {
			var buffers = memory.Buffers;
			foreach (var buffer in buffers)
			{
				if (map.CacheFile.MetaArea.ContainsBlockPointer(buffer.Position, buffer.Buffer.Length))
				{
					Debug.WriteLine("Memory command addr=0x{0:X8} len=0x{1:X}", buffer.Position, buffer.Buffer.Length);
					stream.SeekTo(buffer.Position);
					stream.WriteBlock(buffer.Buffer);
				}
			}  
        }
    }
}
