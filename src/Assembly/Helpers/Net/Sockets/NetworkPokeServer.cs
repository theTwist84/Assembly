using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Mono.Nat;

namespace Assembly.Helpers.Net.Sockets
{
    public class NetworkPokeServer
    {
        private readonly List<Socket> _clients = new List<Socket>();

        public NetworkPokeServer()
        {
            var hostIp = IPAddress.Any;
            var hostEndpoint = new IPEndPoint(hostIp, 19002);
            var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            // Bind to our local endpoint
            listener.Bind(hostEndpoint);
            listener.Listen(128); // Listen with a pending connection queue size of 128

            NatUtility.DeviceFound += DeviceFound;
            NatUtility.StartDiscovery();


            // Begin accepting incoming connections
            listener.BeginAccept(ConnectClient, listener);

        }

        private void DeviceFound(object sender, DeviceEventArgs e)
        {
            var device = e.Device;
            Debug.WriteLine(device.GetExternalIP().ToString());
            var map = new Mapping(Protocol.Tcp, 19002, 19002);
            map.Description = "Assembly Network Poking";
            device.CreatePortMap(map);
        }

        private void ConnectClient(IAsyncResult result)
        {

            var listener = (Socket) result.AsyncState;
            var client = listener.EndAccept(result);
            listener.BeginAccept(ConnectClient, listener);

            lock (_clients)
            {
                _clients.Add(client);
            }
        }

        public void ReceiveCommand(IPokeCommandHandler handler)
        {
            List<Socket> readyClients;
            lock (_clients)
            {
                readyClients = new List<Socket>(_clients);
            }
            if (readyClients.Count == 0)
                return;

            //wait for a client to have a command
            Socket.Select(readyClients, null, null, -1);
            foreach (var socket in readyClients)
            {
                using (var stream = new NetworkStream(socket, false))
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
                            continue;
                    }
                    command.Deserialize(stream);
                    SendCommand(command);
                }
            }
        }

        public void SendCommand(PokeCommand command)
        {
            lock (_clients)
            {
                foreach (var socket in _clients)
                {
                    using (var stream = new NetworkStream(socket, false))
                    {
                        var type = (byte) command.Type;
                        stream.WriteByte(type);
                        command.Serialize(stream);
                    }
                }
            }
        }
    }
}
