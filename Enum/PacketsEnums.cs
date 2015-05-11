namespace ProtocolTrueCraft.Enum
{
    public enum PacketsServer
    {
        KeepAlive = 0x00,

        Animation = 0x12,
        AttachEntity = 0x27,
        BlockAction = 0x36,
        BlockChange = 0x35,
        BulkBlockChange = 0x34,
        ChangeHeldItem = 0x10,
        ChatMessage = 0x03,
        ChunkData = 0x33,
        ChunkPreamble = 0x32,
        ClickWindow = 0x66,
        CloseWindow = 0x65,
        CollectItem = 0x16,
        DestroyEntity = 0x1D,
        Disconnect = 0xFF,
        EntityEquipment = 0x05,
        EntityLookAndRelativeMove = 0x21,

        Handshake = 0x02,
        HandshakeResponse = 0x02,
        LoginResponse = 0x01
    }

    public enum PacketsClient
    {
        KeepAlive = 0x00,

        Animation = 0x12,
        AttachEntity = 0x27,
        BlockAction = 0x36,
        BlockChange = 0x35,
        BulkBlockChange = 0x34,
        ChangeHeldItem = 0x10,
        ChatMessage = 0x03,
        ChunkData = 0x33,
        ChunkPreamble = 0x32,
        ClickWindow = 0x66,
        CloseWindow = 0x65,
        CollectItem = 0x16,
        DestroyEntity = 0x1D,
        Disconnect = 0xFF,
        EntityEquipment = 0x05,
        EntityLookAndRelativeMove = 0x21,

        Handshake = 0x02,
        HandshakeResponse = 0x02,
        LoginResponse = 0x01
    }
}

