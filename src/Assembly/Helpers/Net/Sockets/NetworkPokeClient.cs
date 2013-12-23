using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Windows.Documents;

namespace Assembly.Helpers.Net.Sockets
{
    public class NetworkPokeClient
    {
        private Socket _socket;

        public NetworkPokeClient(IPAddress address)
        {
            var endpoint = new IPEndPoint(address, 19002);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(endpoint);
        }

        public void SendCommand(PokeCommand command)
        {
            using (var stream = new NetworkStream(_socket, false))
            {
                var type = (byte)command.Type;
                stream.WriteByte(type);
                command.Serialize(stream);
            }
        }

        public void ReceiveCommand(IPokeCommandHandler handler)
        {
            using (var stream = new NetworkStream(_socket, false))
            {
                var commandType = (PokeCommandType) stream.ReadByte();
                PokeCommand command;
                switch (commandType)
                {
                    case PokeCommandType.Test:
                        command = new TestCommand();
                        break;
                    case PokeCommandType.Freeze:
                        command = new FreezeCommand();
                        break;
                    case PokeCommandType.Memory:
                        command = new MemoryCommand();
                        break;
                    default:
                        throw new NotImplementedException("The suspected Command Type has not been implemented yet.");
                }
                command.Deserialize(stream);
                command.Handle(handler);
            }
        }
    }
}
