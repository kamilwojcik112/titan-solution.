namespace Fronius.Onboarding.Exceptions
{
    public abstract class ControlException : Exception
    {
        protected ControlException(string message) : base(message)
        {
        }
    }
}