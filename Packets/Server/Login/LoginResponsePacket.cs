using MineLib.Core;
using MineLib.Core.IO;

using ProtocolTrueCraft.Enum;

namespace ProtocolTrueCraft.Packets.Server.Login
{
    /// <summary>
    /// Sent by the server to allow the player to spawn, with information about the world being spawned into.
    /// </summary>
    public struct LoginResponsePacket : IPacket
    {
        public int EntityID;
        public long Seed;
        public Dimension Dimension;

        public byte ID { get { return 0x01; } }

        public IPacket ReadPacket(IProtocolDataReader reader)
        {
            EntityID = reader.ReadInt();
            reader.ReadString(); // Unused
            Seed = reader.ReadLong();
            Dimension = (Dimension) reader.ReadSByte();

            return this;
        }

        public IPacket WritePacket(IProtocolStream stream)
        {
            stream.WriteInt(EntityID);
            stream.WriteString(""); // Unused
            stream.WriteLong(Seed);
            stream.WriteSByte((sbyte) Dimension);

            return this;
        }
    }
}