using CeilingPanelGenerator.Domain;

namespace CeilingPanelGenerator.Application;

public sealed class PanelGridGenerator
{
    public CeilingPanelLayoutResult Generate(
        double spanX,
        double spanY,
        CeilingPanelLayoutSettings settings)
    {
        settings.Validate();

        if (spanX <= 0 || spanY <= 0)
        {
            return new CeilingPanelLayoutResult(Array.Empty<PanelCell>(), 0, 0);
        }

        var innerWidth = Math.Max(0, spanX - (2 * settings.BorderOffset));
        var innerLength = Math.Max(0, spanY - (2 * settings.BorderOffset));

        if (innerWidth <= 0 || innerLength <= 0)
        {
            return new CeilingPanelLayoutResult(Array.Empty<PanelCell>(), 0, 0);
        }

        var cells = new List<PanelCell>();
        var y = settings.BorderOffset;
        var yLimit = settings.BorderOffset + innerLength;
        var xLimit = settings.BorderOffset + innerWidth;

        while (y < yLimit)
        {
            var nextY = Math.Min(y + settings.PanelLength, yLimit);
            var x = settings.BorderOffset;

            while (x < xLimit)
            {
                var nextX = Math.Min(x + settings.PanelWidth, xLimit);
                cells.Add(new PanelCell(x, y, nextX, nextY));
                x = nextX + settings.JointGap;
            }

            y = nextY + settings.JointGap;
        }

        var panelizedArea = cells.Sum(cell => (cell.MaxX - cell.MinX) * (cell.MaxY - cell.MinY));
        var coverage = panelizedArea / (innerWidth * innerLength);
        return new CeilingPanelLayoutResult(cells, panelizedArea, coverage);
    }
}
