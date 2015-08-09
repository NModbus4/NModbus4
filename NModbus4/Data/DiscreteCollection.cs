namespace Modbus.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    ///     Collection of discrete values.
    /// </summary>
    public class DiscreteCollection : Collection<bool>, IModbusMessageDataCollection
    {
        private const int BitsPerByte = 8;
        private readonly List<bool> _discretes;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DiscreteCollection" /> class.
        /// </summary>
        public DiscreteCollection()
            : this(new List<bool>())
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DiscreteCollection" /> class.
        /// </summary>
        public DiscreteCollection(params bool[] bits)
            : this((IList<bool>)bits)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DiscreteCollection" /> class.
        /// </summary>
        public DiscreteCollection(params byte[] bytes)
            : this()
        {
            if (bytes == null)
                throw new ArgumentNullException("bytes");

            _discretes.Capacity = bytes.Length * BitsPerByte;
            foreach (byte b in bytes)
            {
                _discretes.Add((b & 1) == 1);
                _discretes.Add((b & 2) == 2);
                _discretes.Add((b & 4) == 4);
                _discretes.Add((b & 8) == 8);
                _discretes.Add((b & 16) == 16);
                _discretes.Add((b & 32) == 32);
                _discretes.Add((b & 64) == 64);
                _discretes.Add((b & 128) == 128);
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DiscreteCollection" /> class.
        /// </summary>
        public DiscreteCollection(IList<bool> bits)
            : this(new List<bool>(bits))
        {
        }

        internal DiscreteCollection(List<bool> bits)
            : base(bits)
        {
            Debug.Assert(bits != null);
            _discretes = bits;
        }

        /// <summary>
        ///     Gets the network bytes.
        /// </summary>
        public byte[] NetworkBytes
        {
            get
            {
                byte[] bytes = new byte[ByteCount];

                for (int index = 0; index < _discretes.Count; index++)
                {
                    if (_discretes[index])
                    {
                        bytes[index / BitsPerByte] |= (byte)(1 << (index % BitsPerByte));
                    }
                }

                return bytes;
            }
        }

        /// <summary>
        ///     Gets the byte count.
        /// </summary>
        public byte ByteCount
        {
            get { return (byte) ((Count + 7)/8); }
        }

        /// <summary>
        ///     Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </returns>
        public override string ToString()
        {
            return String.Concat("{", String.Join(", ", this.Select(discrete => discrete ? "1" : "0").ToArray()), "}");
        }
    }
}
