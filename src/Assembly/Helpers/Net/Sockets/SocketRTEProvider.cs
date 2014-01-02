using Blamite.Blam;
using Blamite.IO;
using Blamite.RTE;
using Blamite.RTE.H2Vista;

namespace Assembly.Helpers.Net.Sockets
{
    public class SocketRTEProvider : IRTEProvider
    {
        private IPokeCommandHandler _handler;
		public string ExeName { get; set; }

        public SocketRTEProvider(IPokeCommandHandler handler, RTEConnectionType type)
        {
            _handler = handler;
            ConnectionType = type;
        }

		public RTEConnectionType ConnectionType { get; private set; }

        public IStream GetMetaStream(ICacheFile cacheFile)
        {
	        if (cacheFile.Engine == EngineType.ThirdGeneration)
	        {
		        return new EndianStream(new NetworkPokeStream(_handler), Endian.BigEndian);
	        }
	        else
	        {
				return new EndianStream(new NetworkPokeStream(_handler), Endian.LittleEndian);
	        }
        }
    }
}
