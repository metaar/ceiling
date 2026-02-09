namespace CeilingPanelGenerator.Domain;

public sealed record CeilingPanelLayoutSettings(
    double PanelWidth,
    double PanelLength,
    double JointGap,
    double BorderOffset,
    double PanelThickness)
{
    public void Validate()
    {
        if (PanelWidth <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(PanelWidth), "Panel width must be greater than zero.");
        }

        if (PanelLength <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(PanelLength), "Panel length must be greater than zero.");
        }

        if (JointGap < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(JointGap), "Joint gap cannot be negative.");
        }

        if (BorderOffset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(BorderOffset), "Border offset cannot be negative.");
        }

        if (PanelThickness <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(PanelThickness), "Panel thickness must be greater than zero.");
        }
    }
}
