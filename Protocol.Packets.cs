using MineLib.Core;

using ProtocolTrueCraft.Enum;
using ProtocolTrueCraft.Packets;

namespace ProtocolTrueCraft
{
    public sealed partial class Protocol
    {
        private void OnPacketHandled(int id, IPacket packet, ConnectionState state)
        {
            if(!Connected)
                return;

            switch (state)
            {
                case ConnectionState.Joining:

                    #region JoiningServer

                    switch ((PacketsServer) id)
                    {
                        case PacketsServer.LoginResponse:
                            State = ConnectionState.Joined;
                            break;

                        case PacketsServer.Disconnect:
                            Disconnect();
                            break;
                    }

                    #endregion Login

                    break;

                case ConnectionState.Joined:

                    #region JoinedServer

                    switch ((PacketsServer) id)
                    {
                        case PacketsServer.KeepAlive:
                            var keepAlivePacket = (KeepAlivePacket) packet;
                            KeepAliveAsync(new KeepAliveAsyncArgs(0)).Wait();
                            break;

                        case PacketsServer.Disconnect:
                            Disconnect();
                            break;
                    }

                    #endregion

                    break;

                default:
                    throw new ProtocolException("Connection error: Incorrect data.");
            }
        }
    }
}