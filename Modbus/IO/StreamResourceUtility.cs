using System.Linq;
using System.Text;

namespace Modbus.IO
{
	internal static class StreamResourceUtility
	{
		internal static string ReadLine(IStreamResource stream)
		{
			var result = new StringBuilder();
			var singleByteBuffer = new byte[1];

			do
			{
				stream.Read(singleByteBuffer, 0, 1);
				result.Append(Encoding.ASCII.GetChars(singleByteBuffer).First());
			} while (!result.ToString().EndsWith(Modbus.NewLine));

			return result.ToString().Substring(0, result.Length - Modbus.NewLine.Length);
		}		
	}
}
