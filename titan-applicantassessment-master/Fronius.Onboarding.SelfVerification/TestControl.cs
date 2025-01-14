using System;
using Fronius.Onboarding.Controls;

namespace Fronius.Onboarding.SelfVerification;

public class TestControl : IControl
{
    public string Origin { get; set; } = "UnitTest";
    public DateTime Begin { get; set; }
    public DateTime End { get; set; }
}