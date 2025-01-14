using Fronius.Onboarding.Controls;
using Fronius.Onboarding.Logging;
using Fronius.Onboarding.Policies;

namespace Fronius.Onboarding.Repositories
{
    internal class InMemoryControlRepository : IControlRepository
    {
        private readonly List<IControl> _controls = new();
        private readonly object _lock = new object();
        private readonly IControlAddingPolicy _controlAddingPolicy;
        private readonly ILogger _logger;

        public InMemoryControlRepository(IControlAddingPolicy controlAddingPolicy, ILogger logger)
        {
            _controlAddingPolicy = controlAddingPolicy;
            _logger = logger;
        }

        public IControlRepository AddControl(IControl control)
        {
            lock (_lock)
            {
                if (_controlAddingPolicy.CanControlBeAdded(control, this) is false)
                {
                    _logger.LogDebug($"Exactly same Control was already added. Control Begin: {control.Begin}, Control: {control.End}, Control Origin: {control.Origin}");
                    return this;
                }

                _controls.Add(control);
            }

            return this;
        }

        public IList<IControl> GetAll()
        {
            lock (_lock)
            {
                return _controls.ToList();
            }
        }
    }
}