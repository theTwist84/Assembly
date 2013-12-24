using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assembly.Helpers.Net.Sockets
{
    public interface IPokeCommandHandler
    {
        void HandleTestCommand(TestCommand test);
        void HandleFreezeCommand(FreezeCommand freeze);
        void HandleMemoryCommand(MemoryCommand memory);

        void StartTestCommand(TestCommand test);
        void StartFreezeCommand(FreezeCommand freeze);
        void StartMemoryCommand(MemoryCommand memory);
    }
}
