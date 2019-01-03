namespace Durwella.Azure.ArmTesting.Services
{
    public interface IArmFunctionEvaluation
    {
        string EvaluateAndReplaceFunctions(string armTemplate);
    }
}