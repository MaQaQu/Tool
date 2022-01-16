using System;
using System.Diagnostics;

namespace YouiToolkit.Utils
{
    [AttributeUsage(AttributeTargets.All)]
    public class CommentAttribute : Attribute
    {
        public string Comment { get; set; }

        [DebuggerStepThrough]
        public CommentAttribute(string comment)
        {
            Comment = comment;
        }
    }
}
