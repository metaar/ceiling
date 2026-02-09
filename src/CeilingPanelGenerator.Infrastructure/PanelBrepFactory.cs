using CeilingPanelGenerator.Domain;

namespace CeilingPanelGenerator.Infrastructure;

public sealed class PanelBrepFactory
{
    public IReadOnlyList<(double X, double Y)[]> CreatePanelLoops(IEnumerable<PanelCell> cells)
    {
        return cells
            .Select(cell => new[]
            {
                (cell.MinX, cell.MinY),
                (cell.MaxX, cell.MinY),
                (cell.MaxX, cell.MaxY),
                (cell.MinX, cell.MaxY),
                (cell.MinX, cell.MinY)
            })
            .ToList();
    }
}
