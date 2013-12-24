using Blamite.Blam;
using Blamite.IO;
using Blamite.RTE;

namespace Assembly.Helpers.Net.Sockets
{
    public class SocketRTEProvider : IRTEProvider
    {
        private IPokeCommandHandler _handler;

        public SocketRTEProvider(IPokeCommandHandler handler, RTEConnectionType type)
        {
            _handler = handler;
            ConnectionType = type;
        }

		public RTEConnectionType ConnectionType { get; private set; }

        public IStream GetMetaStream(ICacheFile cacheFile)
        {
            return new EndianStream(new SocketStream(_handler), Endian.BigEndian);
        }
    }
}
