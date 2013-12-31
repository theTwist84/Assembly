using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Assembly.Metro.Controls.PageTemplates.Games;

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

        public static void MemoryCommand(MemoryCommand memory, HaloMap map)
        {
            var xbdm = App.AssemblyStorage.AssemblySettings.Xbdm;
            if (xbdm != null)
            {
                var models = memory.Models;
                foreach (var model in models)
                {
                    if (map.CacheFile.MetaArea.ContainsBlockPointer(model.Position, model.Buffer.Length))
                    {
                        xbdm.MemoryStream.Seek(model.Position, SeekOrigin.Begin);
                        xbdm.MemoryStream.Write(model.Buffer, 0, model.Buffer.Length);
                    }
                }
            }
        }
    }
}
