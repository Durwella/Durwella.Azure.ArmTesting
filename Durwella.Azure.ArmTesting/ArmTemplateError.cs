using Newtonsoft.Json;

namespace Durwella.Azure.ArmTesting
{
    public class ArmTemplateError
    {
        public string Path { get; set; }
        public int LineNumber { get; set; }
        public int ColumnNumber { get; set; }
        public string Message { get; set; }

        public ArmTemplateError()
        {
        }

        public ArmTemplateError(int lineNumber, int columnNumber, string message)
        {
            LineNumber = lineNumber;
            ColumnNumber = columnNumber;
            Message = message;
        }

        internal ArmTemplateError(IJsonLineInfo lineInfo, string message)
            : this(lineInfo.LineNumber, lineInfo.LinePosition, message)
        {
        }

        public override string ToString()
        {
            // https://docs.microsoft.com/en-us/cpp/ide/formatting-the-output-of-a-custom-build-step-or-build-event?view=vs-2017
            return $"{Path}({LineNumber},{ColumnNumber}) : error ARM0000: {Message}";
        }
    }
}
