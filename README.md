# Ceiling Panel Generator (Rhino 8)

Rhino 8 plugin starter for automatic ceiling modeling with a layered C# architecture mirroring a generator-style plugin setup.

## Solution layout

- `CeilingPanelGenerator.Domain`
  - Domain records + validation (`CeilingPanelLayoutSettings`, `PanelCell`).
- `CeilingPanelGenerator.Application`
  - Panelization service (`PanelGridGenerator`) that returns both panel cells and summary metrics.
- `CeilingPanelGenerator.Infrastructure`
  - Geometry-adapter helpers (`PanelBrepFactory`) for converting panel cells into closed loops.
- `CeilingPanelGenerator.Plugin`
  - Rhino plugin bootstrap + command implementation (`GenerateCeilingPanels`).

## What the command does

`GenerateCeilingPanels`

1. Prompts for a closed planar ceiling boundary curve.
2. Prompts for panel width, panel length, joint gap, border offset, and panel thickness.
3. Builds panel cells over the boundary extents, clips cells to the selected boundary, and creates downward-extruded panel solids.
4. Prints generated panel count and panelized area coverage in the command line.

## Build

```bash
dotnet build CeilingPanelGenerator.sln
```
