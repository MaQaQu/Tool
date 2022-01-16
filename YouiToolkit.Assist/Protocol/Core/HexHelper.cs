using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouiToolkit.Assist
{
    internal static class HexHelper
	{
		public static string ConvertToHex(string stringdata, Encoding? encoding = null)
		{
			byte[] bytes = (encoding ?? Encoding.UTF8).GetBytes(stringdata);
			StringBuilder hexdata = new StringBuilder();
			foreach (byte b in bytes)
			{
				hexdata.Append(b.ToString("X2") + " ");
			}
			return hexdata.ToString();
		}

		public static string ConvertToHex(byte[] bytes)
		{
			StringBuilder hexdata = new StringBuilder();
			foreach (byte b in bytes)
			{
				hexdata.Append(b.ToString("X2") + " ");
			}
			return hexdata.ToString();
		}
	}
}
