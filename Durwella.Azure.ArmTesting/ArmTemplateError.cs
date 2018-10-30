using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Durwella.Azure.ArmTesting
{
    public class TokenError
    {
        public int LineNumber { get; set; }
        public int ColumnNumber { get; set; }
        public string Message { get; set; }

        public TokenError(int lineNumber, int columnNumber, string message)
        {
            LineNumber = lineNumber;
            ColumnNumber = columnNumber;
            Message = message;
        }

        public TokenError(JToken jToken, string message)
            : this((IJsonLineInfo)jToken, message)
        {
        }

        public TokenError(IJsonLineInfo lineInfo, string message)
            : this(lineInfo.LineNumber, lineInfo.LinePosition, message)
        {
        }
    }

    public class ArmTemplateError
    {
        public string Path { get; }
        public int LineNumber { get; }
        public int ColumnNumber { get; }
        public string Message { get; }

        public ArmTemplateError()
        {
        }

        public ArmTemplateError(string path, int lineNumber, int columnNumber, string message)
        {
            Path = path;
            LineNumber = lineNumber;
            ColumnNumber = columnNumber;
            Message = message;
        }

        public ArmTemplateError(string path, TokenError tokenError)
            : this(path, tokenError.LineNumber, tokenError.ColumnNumber, tokenError.Message)
        {
        }

        public override string ToString()
        {
            // https://docs.microsoft.com/en-us/cpp/ide/formatting-the-output-of-a-custom-build-step-or-build-event?view=vs-2017
            return $"{Path}({LineNumber},{ColumnNumber}) : error ARM0000: {Message}";
        }
    }
}
