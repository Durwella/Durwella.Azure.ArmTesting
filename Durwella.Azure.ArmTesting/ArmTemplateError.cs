namespace Durwella.Azure.ArmTesting
{
    public class ArmTemplateError
    {
        public string Path { get; set; }
        public int LineNumber { get; set; }
        public string Message { get; set; }

        public ArmTemplateError()
        {
        }

        public ArmTemplateError(string path, int lineNumber, string message)
        {
            Path = path;
            LineNumber = lineNumber;
            Message = message;
        }

        public override string ToString()
        {
            // https://docs.microsoft.com/en-us/cpp/ide/formatting-the-output-of-a-custom-build-step-or-build-event?view=vs-2017
            return $"{Path}({LineNumber}) : error ARM0000: {Message}";
        }
    }
}
