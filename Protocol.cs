using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using MineLib.Core;
using MineLib.Core.IO;

using ProtocolTrueCraft.IO;
using ProtocolTrueCraft.Packets;

namespace ProtocolTrueCraft
{
    public sealed partial class Protocol : IProtocol
    {
        #region Properties

        public string Name { get { return "TrueCraft"; } }
        public string Version { get { return "b1.7.3"; } }

        public ConnectionState State { get; set; }

        public bool Connected { get { return _stream != null && _stream.Connected; } }

        public bool UseLogin { get { return _minecraft.UseLogin; } }

        // -- Debugging
        public bool SavePackets { get; private set; }

        public List<IPacket> PacketsReceived { get; private set; }
        public List<IPacket> PacketsSended { get; private set; }

        public List<IPacket> LastPackets { get { return PacketsReceived != null ? PacketsReceived.GetRange(PacketsReceived.Count - 50, 50) : null; } }
        public IPacket LastPacket { get { return PacketsReceived[PacketsReceived.Count - 1]; } }
        // -- Debugging

        #endregion

        private Task _readTask;
        private IMinecraftClient _minecraft;

        private IProtocolStream _stream;


        public IProtocol Initialize(IMinecraftClient client, INetworkTCP tcp, bool debugPackets = false)
        {
            _minecraft = client;
            _stream = new TrueCraftStream(tcp);
            SavePackets = debugPackets;

            PacketsReceived = new List<IPacket>();
            PacketsSended = new List<IPacket>();

            SendingAsyncHandlers = new Dictionary<Type, Func<ISendingAsyncArgs, Task>>();
            RegisterSupportedSendings();

            return this;
        }

        private async void ReadCycle()
        {
            while (PacketReceiver())
                await Task.Delay(50);
        }

        private bool PacketReceiver()
        {
            if (!Connected)
                return false; // -- Terminate cycle

            if (_stream.Available)
            {
                var packetId = _stream.ReadByte();
                HandlePacket(packetId, _stream);
            }

            return true;
        }


        /// <summary>
        /// Packets are handled here. Compression and encryption are handled here too
        /// </summary>
        /// <param name="id">Packet ID</param>
        /// <param name="stream"></param>
        private void HandlePacket(int id, IProtocolStream stream)
        {
            using (var reader = new TrueCraftDataReader(stream as Stream))
            {
                IPacket packet = null;

                switch (State)
                {
                    #region Login

                    case ConnectionState.Joining:
                        if (ServerResponse.JoiningServer[id] == null)
                            throw new ProtocolException("Reading error: Wrong Login packet ID.");

                        packet = ServerResponse.JoiningServer[id]().ReadPacket(reader);

                        OnPacketHandled(id, packet, ConnectionState.Joining);
                        break;

                    #endregion Login

                    #region Play

                    case ConnectionState.Joined:
                        if (ServerResponse.JoinedServer[id] == null)
                            throw new ProtocolException("Reading error: Wrong Play packet ID.");

                        packet = ServerResponse.JoinedServer[id]().ReadPacket(reader);

                        OnPacketHandled(id, packet, ConnectionState.Joined);
                        break;

                    #endregion Play
                }

                if (SavePackets)
                    PacketsReceived.Add(packet);
            }
        }


        #region Network

        public void Connect(string ip, ushort port)
        {
            if (Connected)
                throw new ProtocolException("Connection error: Already connected to server.");

            // -- Connect to server.
            _stream.Connect(ip, port);

            // -- Begin data reading.
            if (_readTask != null && _readTask.Status == TaskStatus.Running)
                throw new ProtocolException("Connection error: Task already running.");
            else
                _readTask = Task.Factory.StartNew(ReadCycle);
        }

        public void Disconnect()
        {
            if (!Connected)
                throw new ProtocolException("Connection error: Not connected to server.");

            _stream.Disconnect(false);
        }

        public void SendPacket(IPacket packet)
        {
            if (!Connected)
                throw new ProtocolException("Connection error: Not connected to server.");

            _stream.SendPacket(ref packet);

            if (SavePackets)
                PacketsSended.Add(packet);
        }

        public void SendPacket(ref IPacket packet)
        {
            if (!Connected)
                throw new ProtocolException("Connection error: Not connected to server.");

            _stream.SendPacket(ref packet);

            if (SavePackets)
                PacketsSended.Add(packet);
        }


        public async Task ConnectAsync(string ip, ushort port)
        {
            if (Connected)
                throw new ProtocolException("Connection error: Already connected to server.");

            await _stream.ConnectAsync(ip, port);

            if (_readTask != null &&_readTask.Status == TaskStatus.Running)
                throw new ProtocolException("Connection error: Task already running.");
            else
                _readTask = Task.Factory.StartNew(ReadCycle);
        }

        public bool DisconnectAsync()
        {
            if (!Connected)
                throw new ProtocolException("Connection error: Not connected to server.");

            return _stream.DisconnectAsync(false);
        }

        public async Task SendPacketAsync(IPacket packet)
        {
            if (!Connected)
                throw new ProtocolException("Connection error: Not connected to server.");

            await _stream.SendPacketAsync(packet);

            if (SavePackets)
                PacketsSended.Add(packet);
        }

        #endregion


        public void Dispose()
        {
            if (_stream != null)
                _stream.Dispose();

            if (PacketsReceived != null)
                PacketsReceived.Clear();

            if (PacketsSended != null)
                PacketsSended.Clear();
        }
    }
}
