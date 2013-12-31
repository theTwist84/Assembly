using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Assembly.Helpers.Net.Sockets
{
    public interface IPokeCommandHandler
    {
        void HandleFreezeCommand(FreezeCommand freeze);
        void HandleMemoryCommand(MemoryCommand memory);
		void HandleClientListCommand(ClientListCommand clientListCommand);

        void StartFreezeCommand(FreezeCommand freeze);
        void StartMemoryCommand(MemoryCommand memory);
		List<string> GetClientIpList();

		void HandleChangeNameCommand(ChangeNameCommand changeNameCommand);
	}
}
