import net.wimpi.modbus.ModbusCoupler;
import net.wimpi.modbus.Modbus;
import net.wimpi.modbus.net.ModbusTCPListener;
import net.wimpi.modbus.procimg.SimpleDigitalIn;
import net.wimpi.modbus.procimg.SimpleDigitalOut;
import net.wimpi.modbus.procimg.SimpleInputRegister;
import net.wimpi.modbus.procimg.SimpleProcessImage;
import net.wimpi.modbus.procimg.SimpleRegister;


/**
 * Class implementing a simple Modbus/TCP slave.
 * A simple process image is available to test
 * functionality and behaviour of the implementation.
 *
 * @author Dieter Wimberger
 * @version 1.2rc1 (09/11/2004)
 */
public class TcpSlave {


  public static void main(String[] args) {

    ModbusTCPListener listener = null;
    SimpleProcessImage spi = null;
    int port = Modbus.DEFAULT_PORT;

    try {
      if(args != null && args.length ==1) {
        port = Integer.parseInt(args[0]);
      }
      System.out.println("jModbus Modbus Slave (Server)");

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


      //2. create the coupler holding the image
      ModbusCoupler.getReference().setProcessImage(spi);
      ModbusCoupler.getReference().setMaster(false);
      ModbusCoupler.getReference().setUnitID(1);

      //3. create a listener with 3 threads in pool
      if (Modbus.debug) System.out.println("Listening...");
      listener = new ModbusTCPListener(3);
      listener.setAddress(java.net.InetAddress.getByAddress(new byte[] {127, 0, 0, 1}));
      listener.setPort(port);
      listener.start();

    } catch (Exception ex) {
      ex.printStackTrace();
    }
  }//main


}//class TCPSlaveTest
