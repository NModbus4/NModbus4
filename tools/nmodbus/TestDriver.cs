using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Modbus.Device;

/// <summary>
/// Summary description for Class1
/// </summary>
public class TestDriver
{
	static void Main(string[] args)
	{
		try
		{
			using (TcpClient client = new TcpClient("127.0.0.1", 502))
			{
				client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
				ModbusTcpMaster master = ModbusTcpMaster.CreateTcp(client);

				// read five input values
				ushort startAddress = 100;
				ushort numInputs = 5;
				bool[] inputs = master.ReadInputs(startAddress, numInputs);

				for (int i = 0; i < numInputs; i++)
					Console.WriteLine("Input {0}={1}", startAddress + i, inputs[i] ? 1 : 0);

				while (true)
				{
				}
			}
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
		}
	}
}
