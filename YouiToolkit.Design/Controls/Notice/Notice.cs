namespace YouiToolkit.Design
{
    public static class Notice
    {
        public static void Info(string message, string title, double durationSeconds = 3)
            => ShowCore(message, title, MessageBoxIcon.Info, durationSeconds);

        public static void Success(string message, string title, double durationSeconds = 3)
            => ShowCore(message, title, MessageBoxIcon.Success, durationSeconds);

        public static void Error(string message, string title, double durationSeconds = 3)
            => ShowCore(message, title, MessageBoxIcon.Error, durationSeconds);

        public static void Warning(string message, string title, double durationSeconds = 3)
            => ShowCore(message, title, MessageBoxIcon.Warning, durationSeconds);

        public static void Question(string message, string title, double durationSeconds = 3)
            => ShowCore(message, title, MessageBoxIcon.Question, durationSeconds);

        public static void Show(string message, string title, MessageBoxIcon noticeIcon = MessageBoxIcon.None, double durationSeconds = 3)
            => ShowCore(message, title, noticeIcon, durationSeconds);

        private static void ShowCore(string message, string title, MessageBoxIcon noticeIcon, double? durationSeconds)
        {
            if (NoticeWindow.Instance == null)
            {
                var window = new NoticeWindow();
                window.Show();
            }
            NoticeWindow.Instance.AddNotice(message, title, durationSeconds, noticeIcon);
        }
    }
}
