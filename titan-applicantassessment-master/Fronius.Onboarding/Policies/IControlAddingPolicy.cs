using Fronius.Onboarding.Repositories;

namespace Fronius.Onboarding.Policies
{
    internal interface IControlAddingPolicy
    {
        bool CanControlBeAdded(Controls.IControl control, IControlRepository controlRepository);
    }
}