using Fronius.Onboarding.Controls;
using Fronius.Onboarding.Logging;

namespace Fronius.Onboarding.Filtering
{
    internal class ControlFilter : IControlFilter
    {
        private readonly ILogger _logger;

        public ControlFilter(ILogger logger)
        {
            _logger = logger;
        }

        public IList<IControl> Filter(IList<IControl> unfilteredControls)
        {
            DateTime now = DateTime.UtcNow;

            // started in past, finished in past
            var withoutControlsStartedAndFinishedInThePast = RemoveControlsThatStartedAndFinishedInThePast(unfilteredControls, now);

            // starting in past, ending in future
            var controlsStartedInThePastAndStillActive = FilterControlsStartedInThePastAndStillActive(withoutControlsStartedAndFinishedInThePast, now);

            // starting in future, ending in future
            var controlsStartInFutureAndEndInFuture = FilterControlsStartInFutureAndEndInFuture(controlsStartedInThePastAndStillActive, now);

            // error cases
            var erroredCases = FilterControlsThatHasInvalidStartAndEndData(controlsStartInFutureAndEndInFuture);

            var filteredControls = erroredCases.ToList();

            return filteredControls;
        }

        private IList<IControl> RemoveControlsThatStartedAndFinishedInThePast(IList<IControl> unfilteredControls, DateTime now)
        {
            return unfilteredControls.Where(control => !(control.Begin < now && control.End < now)).ToList();
        }

        private IList<IControl> FilterControlsStartedInThePastAndStillActive(IList<IControl> unfilteredControls, DateTime now)
        {
            var result = unfilteredControls.Where(control => control.Begin < now && control.End > now).ToList();

            if (result.Count > 0)
            {
                _logger.LogDebug($"Found {result.Count} controls that started in the past and are still active");
            }

            return unfilteredControls;
        }

        private IList<IControl> FilterControlsStartInFutureAndEndInFuture(IList<IControl> unfilteredControls, DateTime now)
        {
            var result = unfilteredControls.Where(control => control.Begin > now && control.End > now).ToList();

            if (result.Count > 0)
            {
                _logger.LogDebug($"Found {result.Count} controls that start in the future and end in the future");
            }

            return unfilteredControls;
        }

        private IList<IControl> FilterControlsThatHasInvalidStartAndEndData(IList<IControl> unfilteredControls)
        {
            return unfilteredControls.Where(control => !(control.Begin >= control.End)).ToList();
        }
    }
}