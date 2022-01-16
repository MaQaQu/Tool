using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace CodeBlock
{
    public partial class MainWindow : Window
    {
        private StringBuilder stringBuilder = new StringBuilder();

        private bool Filter(string file)
        {
            string ext = Path.GetExtension(file).ToLower();

            if (ext == ".cs" || ext == ".xaml")
            {
                if (file.Contains("\\obj\\Debug")
                 || file.Contains("\\obj\\Release")
                 || file.Contains("AssistProtocol.cs")
                 || file.StartsWith(".\\SuperPort")
                 || file.StartsWith(".\\TestConcole")
                 || file.StartsWith(".\\TestForm")
                 )
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        private void ReadCodes(string path)
        {
            foreach (string file in (Directory.GetFiles(path)))
            {
                if (Filter(file))
                {
                    string code = File.ReadAllText(file);
                    int length = code.Count((c) => c == '\n');
                    string ext = Path.GetExtension(file).ToLower();
                    string codeType = Path.GetExtension(file).ToLower() switch { ".xaml" => "xaml", _ => "c#", };

                    stringBuilder.AppendLine("---");
                    stringBuilder.AppendLine($"文件路劲：`{file}`");
                    stringBuilder.AppendLine($"文件行数：`{length}行`");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine($"```{codeType}");
                    stringBuilder.AppendLine(code);
                    stringBuilder.AppendLine("```");
                    stringBuilder.AppendLine();
                }
            }
            foreach (string dir in Directory.GetDirectories(path))
            {
                ReadCodes(dir);
            }
        }

        private int ReadLines(string path)
        {
            int sumline = 0;
            foreach (string file in (Directory.GetFiles(path)))
            {
                if (Filter(file))
                {
                    sumline += File.ReadAllLines(file).Length;
                }
            }
            foreach (string dir in Directory.GetDirectories(path))
            {
                sumline += ReadLines(dir);
            }
            return sumline;
        }

        public MainWindow()
        {
            InitializeComponent();

            stringBuilder.Clear();
            stringBuilder.AppendLine("# 源码");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"工程总源码量：`{ReadLines(".")}`行");
            stringBuilder.AppendLine();
            ReadCodes(".");

            textBox.Text = stringBuilder.ToString();
        }
    }
}
