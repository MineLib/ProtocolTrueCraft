using MineLib.Core;
using MineLib.Core.IO;

namespace ProtocolTrueCraft.Packets
{
    /// <summary>
    /// Sent periodically to confirm that the connection is still active. Send the same packet back
    /// to confirm it. Connection is dropped if no keep alive is received within one minute.
    /// </summary>
    public struct KeepAlivePacket : IPacket
    {
        public byte ID { get { return 0x00; } }

        public IPacket ReadPacket(IProtocolDataReader reader)
        {
            // This space intentionally left blank
            return this;
        }

        public IPacket WritePacket(IProtocolStream stream)
        {
            // This space intentionally left blank
            return this;
        }
    }
}