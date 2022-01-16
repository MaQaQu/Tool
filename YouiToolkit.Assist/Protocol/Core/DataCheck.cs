using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouiToolkit.Assist
{
    internal static class DataCheck
    {
        public static byte[] GetCRC(byte[] data, int start, int end)
        {
            byte[] output = null;

            if (end < start) return output;
            if (start < 0) return output;
            if (end >= data.Length) return output;

            byte num1 = 0xFF;
            byte num2 = 0xFF;
            for (int i = start; i <= end; i++)
            {
                num1 = (byte)(num1 ^ data[i]);
                for (int j = 0; j < 8; j++)
                {
                    byte num3 = (byte)(num2 & 1);
                    byte num4 = (byte)(num1 & 1);
                    num2 = (byte)(num2 >> 1);
                    num2 = (byte)(num2 & 0x7f);
                    num1 = (byte)(num1 >> 1);
                    num1 = (byte)(num1 & 0x7f);
                    if (num3 == 1)
                    {
                        num1 = (byte)(num1 | 0x80);
                    }
                    if (num4 == 1)
                    {
                        num2 = (byte)(num2 ^ 160);
                        num1 = (byte)(num1 ^ 1);
                    }
                }
            }
            output = new byte[] { num2, num1 };

            return output;
        }

        public static byte[] GetSUM(byte[] data, int start, int end)
        {
            byte[] output = null;

            if (end < start) return output;
            if (start < 0) return output;
            if (end >= data.Length) return output;

            uint sum = 0;

            for (int i = start; i <= end; i++)
            {
                sum += data[i];
            }
            output = BitConverter.GetBytes(sum);

            return output;
        }

        public static byte[] GetXOR(byte[] data, int start, int end)
        {
            byte[] output = null;

            if (end < start) return output;
            if (start < 0) return output;
            if (end >= data.Length) return output;

            byte xor = 0;

            for (int i = start; i <= end; i++)
            {
                xor ^= data[i];
            }
            output = new byte[] { xor };

            return output;
        }
    }
}
