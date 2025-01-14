using Fronius.Onboarding.Controls;
using Fronius.Onboarding.DTO;
using Fronius.Onboarding.Exceptions;
using Fronius.Onboarding.Logging;
using Fronius.Onboarding.Policies;
using Fronius.Onboarding.Repositories;

namespace Fronius.Onboarding.Services
{
    internal class ControlService : IControlService
    {
        private readonly IControlAddingPolicy _controlAddingPolicy;
        private readonly IControlRepository _controlRepository;
        private readonly ILogger _logger;

        public ControlService(IControlRepository controlRepository, IControlAddingPolicy controlAddingPolicy, ILogger logger)
        {
            _controlAddingPolicy = controlAddingPolicy;
            _controlRepository = controlRepository;
            _logger = logger;
        }

        public Task<IControlRepository> AddControl(ControlDetailsDto controlDto)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(controlDto);

                var control = new Control
                {
                    Origin = controlDto.Origin,
                    Begin = controlDto.Begin,
                    End = controlDto.End
                };

                if (!_controlAddingPolicy.CanControlBeAdded(control, _controlRepository))
                {
                    throw new CannotAddControlException(control.Origin);
                }

                return Task.Run(() => _controlRepository.AddControl(control));
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError("Received null value: ", ex);
            }
            catch (CannotAddControlException ex)
            {
                _logger.LogError("Error occured durring adding new control: ", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured durring adding new control: ", ex);
            }

            return _controlRepository;
        }

        public Task<IList<IControl>> GetAll()
        {
            return Task.Run(() => _controlRepository.GetAll());
        }
    }
}