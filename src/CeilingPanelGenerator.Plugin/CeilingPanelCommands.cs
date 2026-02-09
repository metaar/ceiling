using CeilingPanelGenerator.Application;
using CeilingPanelGenerator.Domain;
using CeilingPanelGenerator.Infrastructure;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;

namespace CeilingPanelGenerator.Plugin;

public sealed class GenerateCeilingPanelsCommand : Command
{
    public override string EnglishName => "GenerateCeilingPanels";

    protected override Result RunCommand(RhinoDoc doc, RunMode mode)
    {
        var getBoundary = new GetObject();
        getBoundary.SetCommandPrompt("Select closed planar ceiling boundary");
        getBoundary.GeometryFilter = ObjectType.Curve;
        getBoundary.SubObjectSelect = false;
        getBoundary.Get();

        if (getBoundary.CommandResult() != Result.Success)
        {
            return getBoundary.CommandResult();
        }

        var boundary = getBoundary.Object(0).Curve();
        if (boundary is null || !boundary.IsClosed)
        {
            RhinoApp.WriteLine("Boundary must be a closed curve.");
            return Result.Failure;
        }

        if (!boundary.TryGetPlane(out var plane, doc.ModelAbsoluteTolerance))
        {
            RhinoApp.WriteLine("Boundary must be planar.");
            return Result.Failure;
        }

        var boundaryBounds = boundary.GetBoundingBox(plane);
        var spanX = boundaryBounds.Max.X - boundaryBounds.Min.X;
        var spanY = boundaryBounds.Max.Y - boundaryBounds.Min.Y;

        var width = AskForNumber("Panel width", 600.0);
        if (!width.HasValue)
        {
            return Result.Cancel;
        }

        var length = AskForNumber("Panel length", 1200.0);
        if (!length.HasValue)
        {
            return Result.Cancel;
        }

        var gap = AskForNumber("Joint gap", 5.0);
        if (!gap.HasValue)
        {
            return Result.Cancel;
        }

        var border = AskForNumber("Border offset", 0.0);
        if (!border.HasValue)
        {
            return Result.Cancel;
        }

        var thickness = AskForNumber("Panel thickness", 12.0);
        if (!thickness.HasValue)
        {
            return Result.Cancel;
        }

        CeilingPanelLayoutSettings settings;
        try
        {
            settings = new CeilingPanelLayoutSettings(width.Value, length.Value, gap.Value, border.Value, thickness.Value);
            settings.Validate();
        }
        catch (ArgumentOutOfRangeException ex)
        {
            RhinoApp.WriteLine(ex.Message);
            return Result.Failure;
        }

        var generator = new PanelGridGenerator();
        var layout = generator.Generate(spanX, spanY, settings);

        var loopFactory = new PanelBrepFactory();
        var loops = loopFactory.CreatePanelLoops(layout.Panels);

        var attributes = new ObjectAttributes { Name = "CeilingPanel" };

        var createdCount = 0;
        foreach (var loop in loops)
        {
            var corners = loop
                .Select(point => plane.PointAt(boundaryBounds.Min.X + point.X, boundaryBounds.Min.Y + point.Y))
                .ToList();

            var polyline = new Polyline(corners);
            var panelCurve = polyline.ToNurbsCurve();
            var clippedCurves = Curve.CreateBooleanIntersection(panelCurve, boundary, doc.ModelAbsoluteTolerance);
            if (clippedCurves is null)
            {
                continue;
            }

            foreach (var clippedCurve in clippedCurves.Where(curve => curve.IsClosed))
            {
                var breps = Brep.CreatePlanarBreps(clippedCurve, doc.ModelAbsoluteTolerance);
                if (breps is null)
                {
                    continue;
                }

                foreach (var brep in breps)
                {
                    var extrusionPath = new LineCurve(
                        plane.Origin,
                        plane.Origin - (plane.ZAxis * settings.PanelThickness));
                    var extrusion = brep.Faces[0].CreateExtrusion(extrusionPath, true);
                    if (extrusion is null)
                    {
                        continue;
                    }

                    doc.Objects.AddBrep(extrusion, attributes);
                    createdCount += 1;
                }
            }
        }

        doc.Views.Redraw();
        RhinoApp.WriteLine($"Generated {createdCount} panel(s). Coverage: {layout.CoverageRatio:P1}.");
        return Result.Success;
    }

    private static double? AskForNumber(string prompt, double defaultValue)
    {
        var result = RhinoGet.GetNumber(prompt, false, ref defaultValue);
        return result == Result.Success ? defaultValue : null;
    }
}
