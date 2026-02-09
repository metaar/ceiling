using CeilingPanelGenerator.Domain;

namespace CeilingPanelGenerator.Application;

public sealed record CeilingPanelLayoutResult(
    IReadOnlyList<PanelCell> Panels,
    double PanelizedArea,
    double CoverageRatio);
