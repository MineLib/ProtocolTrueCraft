using MineLib.Core;
using MineLib.Core.IO;

namespace ProtocolTrueCraft.Packets.Client.Login
{
    /// <summary>
    /// Sent from clients to begin a new connection.
    /// </summary>
    public struct HandshakePacket : IPacket
    {
        public string Username;
        
        public byte ID { get { return 0x02; } }

        public IPacket ReadPacket(IProtocolDataReader reader)
        {
            Username = reader.ReadString();

            return this;
        }

        public IPacket WritePacket(IProtocolStream stream)
        {
            stream.WriteString(Username);

            return this;
        }
    }
}