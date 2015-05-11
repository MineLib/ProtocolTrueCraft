using System;
using System.IO;

namespace ProtocolTrueCraft.IO
{
    public sealed partial class TrueCraftStream : Stream
    {
        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return Receive(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Send(buffer, offset, count);
        }

        public override bool CanRead
        {
            get { throw new NotImplementedException(); }
        }

        public override bool CanSeek
        {
            get { throw new NotImplementedException(); }
        }

        public override bool CanWrite
        {
            get { throw new NotImplementedException(); }
        }

        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        public override long Position { get; set; }
    }
}