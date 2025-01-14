namespace Fronius.Onboarding.Logging
{
    internal interface ILogger
    {
        void LogDebug(string message);

        void LogInfo();

        void LogWarning();

        void LogError(string message, Exception ex);

        void LogFatal();
    }
}