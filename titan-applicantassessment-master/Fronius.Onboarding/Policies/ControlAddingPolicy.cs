using Fronius.Onboarding.Controls;
using Fronius.Onboarding.Repositories;

namespace Fronius.Onboarding.Policies
{
    internal class ControlAddingPolicy : IControlAddingPolicy
    {
        public bool CanControlBeAdded(IControl control, IControlRepository controlRepository)
        {
            if (controlRepository is null)
            {
                throw new ArgumentNullException(nameof(controlRepository));
            }

            IList<IControl> controls = controlRepository.GetAll();

            var result = controls.Any(c =>
                control.Begin == c.Begin &&
                control.End == c.End &&
                control.Origin == c.Origin);

            return !result;
        }
    }
}