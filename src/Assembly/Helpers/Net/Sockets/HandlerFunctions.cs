using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

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

        public static void MemoryCommand(MemoryCommand memory)
        {
            var xbdm = App.AssemblyStorage.AssemblySettings.Xbdm;
            if (xbdm != null)
            {
                xbdm.MemoryStream.Seek(memory.Offset, SeekOrigin.Begin);
                xbdm.MemoryStream.Write(memory.Data, 0, memory.Data.Length);
            }
        }
    }
}
