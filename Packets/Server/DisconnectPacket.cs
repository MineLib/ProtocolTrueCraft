using MineLib.Core;
using MineLib.Core.IO;

namespace ProtocolTrueCraft.Packets.Server
{
    /// <summary>
    /// Disconnects from a server or kicks a player. This is the last packet sent.
    /// </summary>
    public struct DisconnectPacket : IPacket
    {
        public string Reason;

        public byte ID { get { return 0xFF; } }

        public IPacket ReadPacket(IProtocolDataReader reader)
        {
            Reason = reader.ReadString();

            return this;
        }

        public IPacket WritePacket(IProtocolStream stream)
        {
            stream.WriteString(Reason);

            return this;
        }
    }
}