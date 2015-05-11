using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using MineLib.Core;
using MineLib.Core.Data;
using MineLib.Core.IO;

using Org.BouncyCastle.Math;

namespace ProtocolTrueCraft.IO
{
    // -- Credits to umby24 for encryption support, as taken from CWrapped.
    // -- Credits to SirCmpwn for encryption support, as taken from SMProxy.
    // -- All Write methods doesn't write to any stream. It writes to _buffer. Purge method decide where to put data, depends on _writeToNetwork
    public sealed partial class TrueCraftStream : IProtocolStream
    {
        #region Properties

        public bool Available { get { return _tcp.Available; } }
        public bool Connected { get { return _tcp != null && _tcp.Connected; } }

        #endregion

        private readonly INetworkTCP _tcp;

        private byte[] _buffer;
        private Encoding _encoding = Encoding.BigEndianUnicode;

        public TrueCraftStream(INetworkTCP tcp)
        {
            _tcp = tcp;
        }


        public void Connect(string ip, ushort port)
        {
            _tcp.Connect(ip, port);
        }
        public void Disconnect(bool reuse)
        {
            _tcp.Disconnect(reuse);
        }

        public Task ConnectAsync(string ip, ushort port)
        {
            return _tcp.ConnectAsync(ip, port);
        }

        public bool DisconnectAsync(bool reuse)
        {
            return _tcp.DisconnectAsync(reuse);
        }


        #region Vars

        // -- String

        public void WriteString(string value, int length = 0)
        {
            WriteShort((short) value.Length);
            WriteByteArray(_encoding.GetBytes(value));
        }

        // -- VarInt

        public void WriteVarInt(VarInt value)
        {
            WriteByteArray(GetVarIntBytes(value));
        }

        // BUG: Is broken?
        public static byte[] GetVarIntBytes(int _value)
        {
            uint value = (uint)_value;

            var bytes = new List<byte>();
            while (true)
            {
                if ((value & 0xFFFFFF80u) == 0)
                {
                    bytes.Add((byte)value);
                    break;
                }
                bytes.Add((byte)(value & 0x7F | 0x80));
                value >>= 7;
            }

            return bytes.ToArray();
        }

        // -- Boolean

        public void WriteBoolean(bool value)
        {
            WriteByte(Convert.ToByte(value));
        }

        // -- SByte & Byte

        public void WriteSByte(sbyte value)
        {
            WriteByte(unchecked((byte)value));
        }

        public void WriteByte(byte value)
        {
            if (_buffer != null)
            {
                var tempBuff = new byte[_buffer.Length + 1];

                Buffer.BlockCopy(_buffer, 0, tempBuff, 0, _buffer.Length);
                tempBuff[_buffer.Length] = value;

                _buffer = tempBuff;
            }
            else
                _buffer = new byte[] { value };
        }

        // -- Short & UShort

        public void WriteShort(short value)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);

            WriteByteArray(bytes);
        }

        public void WriteUShort(ushort value)
        {
            WriteByteArray(new byte[]
            {
                (byte) ((value & 0xFF00) >> 8),
                (byte) (value & 0xFF)
            });
        }

        // -- Int & UInt

        public void WriteInt(int value)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);

            WriteByteArray(bytes);
        }

        public void WriteUInt(uint value)
        {
            WriteByteArray(new[]
            {
                (byte)((value & 0xFF000000) >> 24),
                (byte)((value & 0xFF0000) >> 16),
                (byte)((value & 0xFF00) >> 8),
                (byte)(value & 0xFF)
            });
        }

        // -- Long & ULong

        public void WriteLong(long value)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);

            WriteByteArray(bytes);
        }

        public void WriteULong(ulong value)
        {
            WriteByteArray(new[]
            {
                (byte)((value & 0xFF00000000000000) >> 56),
                (byte)((value & 0xFF000000000000) >> 48),
                (byte)((value & 0xFF0000000000) >> 40),
                (byte)((value & 0xFF00000000) >> 32),
                (byte)((value & 0xFF000000) >> 24),
                (byte)((value & 0xFF0000) >> 16),
                (byte)((value & 0xFF00) >> 8),
                (byte)(value & 0xFF)
            });
        }

        // -- BigInt & UBigInt

        public void WriteBigInteger(BigInteger value)
        {
            var bytes = value.ToByteArray();
            Array.Reverse(bytes);

            WriteByteArray(bytes);
        }

        public void WriteUBigInteger(BigInteger value)
        {
            throw new NotImplementedException();
        }

        // -- Float

        public void WriteFloat(float value)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);

            WriteByteArray(bytes);
        }

        // -- Double

        public void WriteDouble(double value)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);

            WriteByteArray(bytes);
        }


        // -- StringArray

        public void WriteStringArray(string[] value)
        {
            var length = value.Length;

            for (var i = 0; i < length; i++)
                WriteString(value[i]);
        }

        // -- VarIntArray

        public void WriteVarIntArray(int[] value)
        {
            var length = value.Length;

            for (var i = 0; i < length; i++)
                WriteVarInt(value[i]);
        }

        // -- IntArray

        public void WriteIntArray(int[] value)
        {
            var length = value.Length;

            for (var i = 0; i < length; i++)
                WriteInt(value[i]);
        }

        // -- ByteArray

        public void WriteByteArray(byte[] value)
        {
            if (_buffer != null)
            {
                var tempLength = _buffer.Length + value.Length;
                var tempBuff = new byte[tempLength];

                Buffer.BlockCopy(_buffer, 0, tempBuff, 0, _buffer.Length);
                Buffer.BlockCopy(value, 0, tempBuff, _buffer.Length, value.Length);

                _buffer = tempBuff;
            }
            else
                _buffer = value;
        }

        #endregion Vars


        // -- Read methods

        public byte ReadByte()
        {
            var buffer = new byte[1];

            Receive(buffer, 0, buffer.Length);

            return buffer[0]; 
        }

        public VarInt ReadVarInt()
        {
            var result = 0;
            var length = 0;

            while (true)
            {
                var current = ReadByte();
                result |= (current & 0x7F) << length++ * 7;

                if (length > 6)
                    throw new InvalidDataException("Invalid varint: Too long.");

                if ((current & 0x80) != 0x80)
                    break;
            }

            return result;
        }

        public byte[] ReadByteArray(int value)
        {
            var result = new byte[value];
            if (value == 0) return result;
            int n = value;
            while (true)
            {
                n -= Receive(result, value - n, n);
                if (n == 0)
                    break;
            }
            return result;
        }

        // -- Read methods


        public async Task SendPacketAsync(IPacket packet)
        {
            WriteByte(packet.ID);
            packet.WritePacket(this);
            Purge(true);
        }


        public void SendPacket(ref IPacket packet)
        {
            WriteByte(packet.ID);
            packet.WritePacket(this);
            Purge(false);
        }
        

        private void Send(byte[] buffer, int offset, int count)
        {
                _tcp.Send(buffer, offset, count);
        }
        private int Receive(byte[] buffer, int offset, int count)
        {
                return _tcp.Receive(buffer, offset, count);
        }

        public Task SendAsync(byte[] buffer, int offset, int count)
        {
                return _tcp.SendAsync(buffer, offset, count);
        }
        public Task<int> ReadAsync(byte[] buffer, int offset, int count)
        {
                return _tcp.ReceiveAsync(buffer, offset, count);
        }
        

        #region Purge

        private void Purge(bool async = false)
        {
            if (async)
                SendAsync(_buffer, 0, _buffer.Length);
            else
                Send(_buffer, 0, _buffer.Length);

            _buffer = null;
        }

        private void PurgeModernWithoutCompression(bool async = false)
        {
            var lenBytes = GetVarIntBytes(_buffer.Length);

            var tempBuff = new byte[_buffer.Length + lenBytes.Length];

            Buffer.BlockCopy(lenBytes, 0, tempBuff, 0, lenBytes.Length);
            Buffer.BlockCopy(_buffer, 0, tempBuff, lenBytes.Length, _buffer.Length);

            if (async)
                SendAsync(tempBuff, 0, tempBuff.Length);
            else
                Send(tempBuff, 0, tempBuff.Length);

            _buffer = null;
        }


        #endregion


        public void Dispose()
        {
            if (_tcp != null)
                _tcp.Dispose();

            _buffer = null;
        }
    }
}