using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Assembly.Helpers.Net.Sockets
{
    public interface IPokeCommandHandler
    {
		//Backend methods that deal with incoming commands
        void HandleFreezeCommand(FreezeCommand freeze);
        void HandleMemoryCommand(MemoryCommand memory);
		void HandleClientListCommand(ClientListCommand clientListCommand);
		void HandleChangeNameCommand(ChangeNameCommand changeNameCommand);

		//these are available to the user
        void StartFreezeCommand(FreezeCommand freeze);
        void StartMemoryCommand(MemoryCommand memory);
		void StartNameChangeCommand(ChangeNameCommand changeNameCommand);

	}
}
