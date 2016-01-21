using System.Text;

namespace DragonWar.Lib.Util
{
	public static class ByteUtils {
		public static string BytesToHex(byte[] pBuffer) {
			StringBuilder builder = new StringBuilder();
			int count = 0;
			foreach (byte b in pBuffer) {
				builder.AppendFormat("{0:X2} ", b);
				count ++;
				if (count == 4) {
					builder.AppendLine();
					count = 0;
				}
			}

			return builder.ToString();
		}
	}
}