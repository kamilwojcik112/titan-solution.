using Fronius.Onboarding.Controls;
using Fronius.Onboarding.DTO;
using Fronius.Onboarding.Repositories;

namespace Fronius.Onboarding.Services
{
    internal interface IControlService
    {
        Task<IList<IControl>> GetAll();

        Task<IControlRepository> AddControl(ControlDetailsDto control);
    }
}