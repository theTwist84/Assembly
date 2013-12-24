using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Diagnostics;

namespace Assembly.Helpers.Net.Sockets
{
    public class ClientCommandHandler: IPokeCommandHandler
    {
        private NetworkPokeClient _client;
        public ClientCommandHandler(string IpAddress)
        {
            _client = new NetworkPokeClient(IPAddress.Parse(IpAddress));
             var thread = new Thread(new ThreadStart(delegate
             {
                 while (true)
                 {
                     _client.ReceiveCommand(this);
                 }
             }));
             thread.Start();         
        }



        public void HandleTestCommand(TestCommand test)
        {
            Debug.WriteLine("As Client" + test.Message);
        }

        public void HandleFreezeCommand(FreezeCommand freeze)
        {
            HandlerFunctions.FreezeCommand(freeze);
        }

        public void HandleMemoryCommand(MemoryCommand memory)
        {
            HandlerFunctions.MemoryCommand(memory);
        }

        public void StartTestCommand(TestCommand test)
        {
            _client.SendCommand(test);
        }

        public void StartFreezeCommand(FreezeCommand freeze)
        {
            _client.SendCommand(freeze);
        }

        public void StartMemoryCommand(MemoryCommand memory)
        {
            _client.SendCommand(memory);
        }
    }
}
