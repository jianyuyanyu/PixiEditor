﻿using PixiEditor.ChangeableDocument.Changeables.Graph.Nodes.Shapes.Data;
using PixiEditor.ChangeableDocument.Rendering;
using Drawie.Backend.Core.ColorsImpl;
using Drawie.Numerics;

namespace PixiEditor.ChangeableDocument.Changeables.Graph.Nodes.Shapes;

[NodeInfo("Ellipse")]
public class EllipseNode : ShapeNode<EllipseVectorData>
{
    public InputProperty<VecD> Center { get; }
    public InputProperty<VecD> Radius { get; }
    public InputProperty<Color> StrokeColor { get; }
    public InputProperty<Color> FillColor { get; }
    public InputProperty<int> StrokeWidth { get; }

    public EllipseNode()
    {
        Center = CreateInput<VecD>("Position", "POSITION", VecI.Zero);
        Radius = CreateInput<VecD>("Radius", "RADIUS", new VecD(32, 32)).WithRules(
            v => v.Min(new VecD(1)));
        StrokeColor = CreateInput<Color>("StrokeColor", "STROKE_COLOR", new Color(0, 0, 0, 255));
        FillColor = CreateInput<Color>("FillColor", "FILL_COLOR", new Color(0, 0, 0, 255));
        StrokeWidth = CreateInput<int>("StrokeWidth", "STROKE_WIDTH", 1);
    }

    protected override EllipseVectorData? GetShapeData(RenderContext context)
    {
        return new EllipseVectorData(Center.Value, Radius.Value)
            { StrokeColor = StrokeColor.Value, FillColor = FillColor.Value, StrokeWidth = StrokeWidth.Value };
    }

    public override Node CreateCopy() => new EllipseNode();
}
