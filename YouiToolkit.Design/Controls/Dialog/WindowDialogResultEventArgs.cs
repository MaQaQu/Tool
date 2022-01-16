using System;

namespace YouiToolkit.Design
{
    public class WindowDialogResultEventArgs : HandlabledEventArgs
    {
        public object? Result { get; set; }

        public WindowDialogResult DialogResult => Convert(Result);

        public WindowDialogResultEventArgs(object? result)
        {
            Result = result;
        }

        public WindowDialogResult Convert(object? result)
        {
            _ = TryConvert(result, out WindowDialogResult dialogResult);
            return dialogResult;
        }

        public bool TryConvert(object? result, out WindowDialogResult dialogResult)
        {
            if (result == null)
            {
                dialogResult = WindowDialogResult.None;
                return false;
            }
            else if (result is WindowDialogResult @enum)
            {
                dialogResult = @enum;
                return true;
            }
            else if (result is string @string)
            {
                return Enum.TryParse(@string, out dialogResult);
            }
            else if (result is int @int)
            {
                if (Enum.IsDefined(typeof(WindowDialogResult), (WindowDialogResult)@int))
                {
                    dialogResult = (WindowDialogResult)@int;
                    return true;
                }
                else
                {
                    dialogResult = WindowDialogResult.None;
                    return false;
                }
            }
            else
            {
                dialogResult = WindowDialogResult.None;
                return false;
            }
        }
    }
}
