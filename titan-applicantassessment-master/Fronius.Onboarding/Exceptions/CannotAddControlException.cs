namespace Fronius.Onboarding.Exceptions
{
    internal class CannotAddControlException : ControlException
    {
        public string Origin { get; }

        public CannotAddControlException(string origin) : base($"Control with Origin {origin} cannot be added")
        {
            Origin = origin;
        }
    }
}