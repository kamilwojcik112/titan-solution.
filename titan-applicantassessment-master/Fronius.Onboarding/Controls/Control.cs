using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fronius.Onboarding.Controls
{
    internal class Control : IControl
    {
        public string Origin { get; set; }
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
    }
}