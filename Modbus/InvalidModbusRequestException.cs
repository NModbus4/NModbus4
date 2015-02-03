using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Modbus.Message;

namespace Modbus
{
    /// <summary>
    ///     An exception that provides the exception code that will be sent in response to an invalid Modbus request.
    /// </summary>
    public class InvalidModbusRequestException : Exception
    {
		readonly byte _exceptionCode;

        /// <summary>
        ///     Initializes a new instance of the <see cref="InvalidModbusRequestException" /> class.
        /// </summary>
		/// <param name="exceptionCode">The exception code to provide to the slave.</param>
        public InvalidModbusRequestException(byte exceptionCode)
        {
			_exceptionCode = exceptionCode;
        }

		/// <summary>
		/// Gets the exception code to provide to the slave.
		/// </summary>
		public byte ExceptionCode
		{
			get { return _exceptionCode; }
		}
    }
}