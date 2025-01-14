namespace Fronius.Onboarding.Controls;

/// <summary>
/// A Control is a Operation/Command, that can be performed by an inverter.
/// It is sent to the inverter and the inverter acts accordingly.
/// With Controls we can set/modify/update existing behavior on the inverter.
/// A Control has a dedicated StartTime - this is when the effect of the control becomes active
/// A Control has a dedidcated EndTime - this is where the effect of the control ends
///
/// Some Examples:
/// - if we send a Disconnect control, the inverter will Disconnect from the Grid.
/// - if we send a Standby Control, to Inverter will change to Standby.
/// - if we send a Generation-Limitation Control, the inverter will reduce it's power generation.
/// - ...
/// </summary>
public interface IControl
{
    /// <summary>
    /// Who sent the Control, who was the originator of the Control
    /// </summary>
    public string Origin { get; set; }

    public DateTime Begin { get; set; }
    public DateTime End { get; set; }
}