import net.wimpi.modbus.*;
import net.wimpi.modbus.net.*;
import net.wimpi.modbus.procimg.*;
import net.wimpi.modbus.util.*;

// class implementing a simple Modbus slave
public class SerialSlave
{

	public static void main(String[] args)
	{

		ModbusSerialListener listener = null;
		SimpleProcessImage spi = new SimpleProcessImage();
		String portname = args[0];

		if (Modbus.debug) System.out.println("jModbus ModbusSerial Slave");

		try
		{
			//1. Prepare a process image
			spi = new SimpleProcessImage();

			for (int i = 0; i < 3000; i++)
				spi.addDigitalOut(new SimpleDigitalOut(false));


			for (int i = 0; i < 3000; i++)
				spi.addDigitalIn(new SimpleDigitalIn(false));

			for (int i = 0; i < 3000; i++)
				spi.addRegister(new SimpleRegister(0));

			for (int i = 0; i < 3000; i++)
				spi.addInputRegister(new SimpleInputRegister(0));

			//2. Create the coupler and set the slave identity
			ModbusCoupler.getReference().setProcessImage(spi);
			ModbusCoupler.getReference().setMaster(false);
			ModbusCoupler.getReference().setUnitID(1);

			//3. Set up serial parameters
			SerialParameters params = new SerialParameters();
			params.setPortName(portname);
			params.setBaudRate(9600);
			params.setDatabits(8);
			params.setParity("None");
			params.setStopbits(1);
			params.setEncoding(args[1]);
			params.setEcho(false);
			if (Modbus.debug) System.out.println("Encoding [" + params.getEncoding() + "]");

			//4. Set up serial listener
			listener = new ModbusSerialListener(params);
			listener.setListening(true);

		}
		catch (Exception ex)
		{
			ex.printStackTrace();
		}
	}
}

