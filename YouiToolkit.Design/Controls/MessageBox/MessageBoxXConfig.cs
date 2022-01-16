using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace YouiToolkit.Design
{
    public class MessageBoxXConfig
    {
        public MessageBoxXConfig(MessageBoxIcon buttonIcon) : this()
        {
            MessageBoxIcon = buttonIcon;
        }

        public MessageBoxXConfig()
        {
            switch (Thread.CurrentThread.CurrentUICulture.IetfLanguageTag)
            {
                case "zh-CN":
                    YesButton = "是的";
                    NoButton = "不";
                    OKButton = "好的";
                    CancelButton = "取消";
                    InformationTtitle = "提示";
                    WarningTtitle = "警告";
                    SuccessTtitle = "完成";
                    ErrorTtitle = "错误";
                    QuestionTtitle = "询问";
                    break;
            }
        }

        public MessageBoxStyle MessageBoxStyle { get; set; } = MessageBoxStyle.Standard;
        public MessageBoxIcon MessageBoxIcon { get; set; } = MessageBoxIcon.Info;
        public DefaultButton DefaultButton { get; set; } = DefaultButton.YesOK;
        public Brush ButtonBrush { get; set; } = "#55CEF1".ToColor().ToBrush();
        public double MinHeight { get; set; } = 250;
        public double MinWidth { get; set; } = 500;
        public double FontSize { get; set; } = 16;
        public double MaxContentHeight { get; set; } = 100;
        public double MaxContentWidth { get; set; } = 300;
        public bool ShowInTaskbar { get; set; } = true;
        public bool Topmost { get; set; } = true;
        public WindowStartupLocation WindowStartupLocation { get; set; } = WindowStartupLocation.CenterOwner;
        public bool InteractOwnerMask { get; set; } = true;

        public string YesButton { get; set; } = "Yes";
        public string NoButton { get; set; } = "No";
        public string OKButton { get; set; } = "OK";
        public string CancelButton { get; set; } = "Cancel";
        public bool ReverseButtonSequence { get; set; }

        public string InformationTtitle { get; set; } = "Information";
        public string WarningTtitle { get; set; } = "Warning";
        public string SuccessTtitle { get; set; } = "Success";
        public string ErrorTtitle { get; set; } = "Error";
        public string QuestionTtitle { get; set; } = "Question";
        public string Title => MessageBoxIcon switch
        {
            MessageBoxIcon.Success => SuccessTtitle,
            MessageBoxIcon.Error => ErrorTtitle,
            MessageBoxIcon.Warning => WarningTtitle,
            MessageBoxIcon.Question => QuestionTtitle,
            _ => InformationTtitle,
        };
    }

    public enum MessageBoxStyle
    {
        Standard,
        Modern,
        Classic,
    }

    public enum MessageBoxIcon
    {
        None,
        Info,
        Success,
        Error,
        Warning,
        Question,
    }

    public enum DefaultButton
    {
        None,
        YesOK,
        NoOrCancel,
        CancelOrNo,
    }
}
