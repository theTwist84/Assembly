using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Assembly.Helpers.Net.Sockets
{
    public class SocketStream : Stream
    {
        private IPokeCommandHandler _handler;

        public SocketStream(IPokeCommandHandler handler)
        {
            _handler = handler;
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
			switch (origin)
			{
				case SeekOrigin.Begin:
					Position = offset;
					break;

				case SeekOrigin.Current:
					Position += offset;
					break;

				case SeekOrigin.End:
					Position = 0x100000000 - offset;
					break;
			}
			return Position;
        }

        public override void SetLength(long value)
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
			return 0;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
			// Create a new byte array to write
			var writeBuffer = new byte[count];
			Buffer.BlockCopy(buffer, offset, writeBuffer, 0, count);

			// Send a MemoryCommand to the server
            var memory = new MemoryCommand((uint)Position, writeBuffer);
            _handler.StartMemoryCommand(memory);
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override long Length
        {
            get { return 0x100000000; }
        }

        public override long Position { get; set; }
    }
}
