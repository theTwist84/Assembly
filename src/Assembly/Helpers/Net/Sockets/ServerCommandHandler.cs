using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Assembly.Helpers.Net.Sockets
{
     public class ServerCommandHandler : IPokeCommandHandler
    {
         private NetworkPokeServer _server;
         public ServerCommandHandler()
         {
             _server = new NetworkPokeServer();
             var thread = new Thread(new ThreadStart(delegate
             {
                 while (true)
                 {
                     _server.ReceiveCommand(this);
                 }
             }));
             thread.Start();
         }

         public void HandleTestCommand(TestCommand test)
         {
             Debug.WriteLine("As server: " + test.Message);
             _server.SendCommandToAll(test);
         }

         public void HandleFreezeCommand(FreezeCommand freeze)
         {
             HandlerFunctions.FreezeCommand(freeze);
             _server.SendCommandToAll(freeze);
         }

         public void HandleMemoryCommand(MemoryCommand memory)
         {
             HandlerFunctions.MemoryCommand(memory);
             _server.SendCommandToAll(memory);
         }


         public void StartTestCommand(TestCommand test)
         {
             HandleTestCommand(test);
         }

         public void StartFreezeCommand(FreezeCommand freeze)
         {
             HandleFreezeCommand(freeze);
         }

         public void StartMemoryCommand(MemoryCommand memory)
         {
		    HandleMemoryCommand(memory);
         }
    }
}
