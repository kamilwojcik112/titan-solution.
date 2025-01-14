using Fronius.Onboarding.Controls;

namespace Fronius.Onboarding.Filtering;

public interface IControlFilter
{
    IList<IControl> Filter(IList<IControl> unfilteredControls);
}