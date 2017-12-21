namespace Modbus.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Net;

    using Utility;

    /// <summary>
    ///     Collection of 16 bit registers.
    /// </summary>
    public class StringData : IModbusMessageDataCollection
    {
        private String mData;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StringData" /> class.
        /// </summary>
        public StringData()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StringData" /> class.
        /// </summary>
        /// <param name="bytes">Array for register collection.</param>
        public StringData(byte[] bytes)
        {
            mData = System.Text.Encoding.ASCII.GetString(bytes);
        }

        ///// <summary>
        /////     Initializes a new instance of the <see cref="RegisterCollection" /> class.
        ///// </summary>
        ///// <param name="registers">Array for register collection.</param>
        //public RegisterCollection(params ushort[] registers)
        //    : this((IList<ushort>)registers)
        //{
        //}

        ///// <summary>
        /////     Initializes a new instance of the <see cref="RegisterCollection" /> class.
        ///// </summary>
        ///// <param name="registers">List for register collection.</param>
        //public RegisterCollection(IList<ushort> registers)
        //    : base(registers.IsReadOnly ? new List<ushort>(registers) : registers)
        //{
        //}

        public byte[] NetworkBytes
        {
            get
            {
                return System.Text.Encoding.ASCII.GetBytes(mData);
            }
        }

        /// <summary>
        ///     Gets the byte count.
        /// </summary>
        public byte ByteCount
        {
            get { return (byte)mData.Length; }
        }

        /// <summary>
        ///     Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </returns>
        public override string ToString()
        {
            return mData;
        }
    }
}
