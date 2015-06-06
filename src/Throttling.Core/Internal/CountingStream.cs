using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Throttling.Core.Internal
{

    public class CountingStream : Stream
    {
        private readonly Stream _inner;
        private long _length;

        public CountingStream(Stream inner)
        {
            _inner = inner;
        }

        public override bool CanRead
        {
            get
            {
                return _inner.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return _inner.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return _inner.CanWrite;
            }
        }

        public override long Length
        {
            get
            {
                return _length;
            }
        }

        public override long Position
        {
            get
            {
                return _inner.Position;
            }

            set
            {
                _inner.Position = value;
            }
        }

        public override bool CanTimeout
        {
            get
            {
                return _inner.CanTimeout;
            }
        }

        public override int ReadTimeout
        {
            get
            {
                return _inner.ReadTimeout;
            }

            set
            {
                _inner.ReadTimeout = value;
            }
        }

        public override int WriteTimeout
        {
            get
            {
                return _inner.WriteTimeout;
            }

            set
            {
                _inner.WriteTimeout = value;
            }
        }

        public override void Flush()
        {
            _inner.Flush();
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return _inner.FlushAsync(cancellationToken);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _inner.Read(buffer, offset, count);
        }

        public async override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return await _inner.ReadAsync(buffer, offset, count, cancellationToken);
        }
#if DNX451
        // We only anticipate using ReadAsync
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return _inner.BeginRead(buffer, offset, count, callback, state);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            Task<int> task = asyncResult as Task<int>;
            if (task != null)
            {
                return task.GetAwaiter().GetResult();
            }

            return _inner.EndRead(asyncResult);
        }
#endif

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _inner.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _inner.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _length += count;
            _inner.Write(buffer, offset, count);
        }
#if DNX451
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            _length += count;
            return _inner.BeginWrite(buffer, offset, count, callback, state);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            _inner.EndWrite(asyncResult);
        }
#endif
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            _length += count;
            return _inner.WriteAsync(buffer, offset, count, cancellationToken);
        }

        public override void WriteByte(byte value)
        {
            _length++;
            _inner.WriteByte(value);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _inner.Dispose();
            }
        }
    }
}
