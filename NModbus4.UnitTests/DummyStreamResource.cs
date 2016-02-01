namespace Modbus.UnitTests
{
    public class DummyStreamResource : global::Modbus.IO.IStreamResource
    {
        public int InfiniteTimeout => default(int);

        public int ReadTimeout { get; set; }

        public int WriteTimeout { get; set; }

        public void DiscardInBuffer()
        {
        }

        public int Read(byte[] buffer, int offset, int count) => default(int);


        public void Write(byte[] buffer, int offset, int count)
        {
        }

        public void Dispose()
        {
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
