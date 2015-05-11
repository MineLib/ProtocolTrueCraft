using MineLib.Core;
using MineLib.Core.IO;

namespace ProtocolTrueCraft.Packets.Client.Login
{
    /// <summary>
    /// Sent by clients after the handshake to request logging into the world.
    /// </summary>
    public struct LoginRequestPacket : IPacket
    {
        public int ProtocolVersion;
        public string Username;

        public byte ID { get { return 0x01; } }

        public IPacket ReadPacket(IProtocolDataReader reader)
        {
            ProtocolVersion = reader.ReadInt();
            Username = reader.ReadString();
            reader.ReadLong(); // Unused
            reader.ReadByte();  // Unused

            return this;
        }

        public IPacket WritePacket(IProtocolStream stream)
        {
            stream.WriteInt(ProtocolVersion);
            stream.WriteString(Username);
            stream.WriteLong(0); // Unused
            stream.WriteByte(0);  // Unused

            return this;
        }
    }
}