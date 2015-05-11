using MineLib.Core;
using MineLib.Core.IO;

namespace ProtocolTrueCraft.Packets.Server.Login
{
    /// <summary>
    /// Sent from servers to continue with a connection. A kick is sent instead if the connection is refused.
    /// </summary>
    public struct HandshakeResponsePacket : IPacket
    {
        /// <summary>
        /// Set to "-" for offline mode servers. Online mode beta servers are obsolete.
        /// </summary>
        public string ConnectionHash;

        public byte ID { get { return 0x02; } }

        public IPacket ReadPacket(IProtocolDataReader reader)
        {
            ConnectionHash = reader.ReadString();

            return this;
        }

        public IPacket WritePacket(IProtocolStream stream)
        {
            stream.WriteString(ConnectionHash);

            return this;
        }
    }
}