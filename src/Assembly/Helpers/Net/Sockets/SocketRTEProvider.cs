using Blamite.Blam;
using Blamite.IO;
using Blamite.RTE;

namespace Assembly.Helpers.Net.Sockets
{
    public class SocketRTEProvider : IRTEProvider
    {
        private NetworkPokeClient _client;

        public SocketRTEProvider(NetworkPokeClient client, RTEConnectionType type)
        {
            _client = client;
            ConnectionType = type;
        }

        public RTEConnectionType ConnectionType
        {
            get;
            private set;
        }
        public IStream GetMetaStream(ICacheFile cacheFile)
        {
            return new EndianStream(new SocketStream(_client), Endian.BigEndian);
        }
    }
}
