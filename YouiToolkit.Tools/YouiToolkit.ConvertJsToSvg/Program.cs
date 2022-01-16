using System.Diagnostics;
using System.Text;
using YouiToolkit.ConvertJsToSvg.Properties;

Dictionary<string, List<string>> map = new Dictionary<string, List<string>>();

Console.WriteLine("Convert `iconfont.js` to `./svg/*.svg`\r\n");

string iconfont;

if (File.Exists("iconfont.js"))
{
    using TextReader reader = new StreamReader("iconfont.js");
    iconfont = reader.ReadToEnd();
}
else
{
    iconfont = Resources.iconfont;
}

const string symbolTag = "symbol id=\"";
const string pathTag = "path d=\"";
const string endTag = "\"";

int index = 0;
int indexCurrent;

while ((indexCurrent = iconfont.IndexOf(symbolTag, index)) > 0)
{
    int indexNameEnd = iconfont.IndexOf(endTag, indexCurrent + symbolTag.Length);
    string name = iconfont.Substring(indexCurrent + symbolTag.Length, indexNameEnd - (indexCurrent + symbolTag.Length)).ToLower();

    map.Add(name, new List<string>());
    Console.WriteLine(name);
    Console.WriteLine();

    int indexPathStart = indexNameEnd;
    int indexPathEnd = indexNameEnd;

    while (iconfont.IndexOf(pathTag, indexPathStart) > 0
        && iconfont.IndexOf(pathTag, indexPathStart) < iconfont.IndexOf(symbolTag, indexPathStart))
    {
        indexPathStart = iconfont.IndexOf(pathTag, indexPathStart);
        indexPathEnd = iconfont.IndexOf(endTag, indexPathStart + pathTag.Length);

        string path = iconfont.Substring(indexPathStart + pathTag.Length, indexPathEnd - (indexPathStart + pathTag.Length));
        map[name].Add(path);
        Console.WriteLine(path);

        indexPathStart = indexPathEnd;
    }

    Console.WriteLine("---");

    // ---
    StringBuilder sb = new StringBuilder();
    const string s1 = "<?xml version=\"1.0\" standalone=\"no\"?><!DOCTYPE svg PUBLIC \" -//W3C//DTD SVG 1.1//EN\" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\"><svg t=\"1637636643032\" class=\"icon\" viewBox=\"0 0 1024 1024\" version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" p-id=\"2537\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" width=\"256\" height=\"256\"><defs><style type=\"text/css\"></style></defs>";
    const string s2 = "<path d=\"{0}\"></path>";
    const string s3 = "</svg>";
    sb.Append(s1);
    foreach (string path in map[name])
    {
        sb.Append(string.Format(s2, path));
    }
    sb.Append(s3);

    string svg = sb.ToString();
    string svgPath = $"svg/{name}.svg";
    Directory.CreateDirectory(Path.GetDirectoryName(svgPath));
    File.Delete(svgPath);
    TextWriter textWriter = new StreamWriter(svgPath);

    textWriter.Write(svg);
    textWriter.Flush();
    textWriter.Close();
    // ---

    index = indexPathEnd;
}

Console.WriteLine($"{map.Count} files saved.");
if (map.Count > 0)
{
    Process.Start("explorer.exe", "svg");
}
