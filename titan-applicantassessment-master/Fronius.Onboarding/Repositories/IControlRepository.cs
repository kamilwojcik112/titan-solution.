using Fronius.Onboarding.Controls;

namespace Fronius.Onboarding.Repositories;

public interface IControlRepository
{
    public IList<IControl> GetAll();

    public IControlRepository AddControl(IControl control);
}