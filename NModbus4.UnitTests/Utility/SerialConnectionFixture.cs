using System;
using System.Collections.Generic;
using System.Text;
using Modbus.Net;
using Modbus.Util;
using NUnit.Framework;
using System.Threading;

namespace Modbus.UnitTests.Util
{
	[TestFixture]
	public class SerialConnectionFixture
	{
		SerialParameters _serialParameters;
		SerialConnection _serialConnection;

		[SetUp]
		public void SetUp()
		{
			_serialParameters = new SerialParameters();
			_serialParameters.PortName = "COM3";

			_serialConnection = new SerialConnection(_serialParameters);

			//TODO what I really want to do above is have a config file w/ test settings which will vary by developement environment
			// e.g. SerialParameters sp = new SerialParameters(props.TestPortName, props.TestBaudRate, ...
		}

		[TearDown]
		public void TearDown()
		{
			if (_serialConnection.IsOpen)
				_serialConnection.Close();
			
			Thread.Sleep(200);
		}

		[Test]
		public void CreateSerialConnectionDefaultParameters()
		{
			SerialConnection sc = new SerialConnection(new SerialParameters());
			Assert.IsNotNull(sc);
		}

		[Test]
		public void CheckOpenConnection()
		{
			_serialConnection.Open();
			Assert.IsTrue(_serialConnection.IsOpen);
		}

		[Test]
		public void CheckCloseConnection()
		{
			_serialConnection.Open();
			Assert.IsTrue(_serialConnection.IsOpen);
			_serialConnection.Close();
			Thread.Sleep(250);
			Assert.IsFalse(_serialConnection.IsOpen);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void CheckOpenConnectionBadPortName()
		{
			_serialParameters.PortName = "JUNK";
			SerialConnection sc = new SerialConnection(_serialParameters);
			sc.Open();
		}
	}
}
