using System.Globalization;
using System.Text.RegularExpressions;

namespace YouiToolkit.Design
{
	public static class IconFontHelper
	{
		/// <summary>
		/// 格式化字符
		/// </summary>
		/// <param name="symbol">原字符</param>
		/// <returns>输出字符</returns>
		public static string GetSymbol(string symbol)
		{
			if (string.IsNullOrEmpty(symbol))
			{
				return string.Empty;
			}
			if (symbol.Length == 1)
			{
				return symbol;
			}
			if (Regex.IsMatch(symbol, @"^&?#?\\?[x|u]"))
			{
				Regex reUnicode = new Regex(@"&?#?\\?[x|u]([0-9a-fA-F]{4});", RegexOptions.Compiled);

				symbol = reUnicode.Replace(symbol, m =>
				{
					if (short.TryParse(m.Groups[1].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out short c))
					{
						return $"{(char)c}";
					}
					return m.Value;
				});
				return symbol;
			}
			if (int.TryParse(symbol, out int utf))
			{
				return char.ConvertFromUtf32(utf);
			}
			return symbol;
		}
	}
}
