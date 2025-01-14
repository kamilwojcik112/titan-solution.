namespace Fronius.Onboarding.Logging
{
    internal class Logger : ILogger
    {
        public void LogDebug(string message)
        {
            Console.WriteLine(message);
        }

        public void LogError(string message, Exception ex)
        {
            Console.WriteLine(message, ex);
        }

        public void LogFatal()
        {
            throw new NotImplementedException();
        }

        public void LogInfo()
        {
            throw new NotImplementedException();
        }

        public void LogWarning()
        {
            throw new NotImplementedException();
        }
    }
}