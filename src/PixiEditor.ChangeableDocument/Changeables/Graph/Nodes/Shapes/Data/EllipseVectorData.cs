﻿using PixiEditor.ChangeableDocument.Changeables.Graph.Interfaces.Shapes;
using Drawie.Backend.Core.ColorsImpl;
using Drawie.Backend.Core.Numerics;
using Drawie.Backend.Core.Surfaces;
using Drawie.Backend.Core.Surfaces.PaintImpl;
using Drawie.Numerics;

namespace PixiEditor.ChangeableDocument.Changeables.Graph.Nodes.Shapes.Data;

public class EllipseVectorData : ShapeVectorData, IReadOnlyEllipseData
{
    public VecD Radius { get; set; }
    
    public VecD Center { get; set; }

    public override RectD GeometryAABB =>
        new ShapeCorners(Center, Radius * 2).AABBBounds;

    public override ShapeCorners TransformationCorners =>
        new ShapeCorners(Center, Radius * 2).WithMatrix(TransformationMatrix);


    public EllipseVectorData(VecD center, VecD radius)
    {
        Center = center;
        Radius = radius;
    }

    public override void RasterizeGeometry(DrawingSurface drawingSurface)
    {
        Rasterize(drawingSurface, false);
    }

    public override void RasterizeTransformed(DrawingSurface drawingSurface)
    {
        Rasterize(drawingSurface, true);
    }

    private void Rasterize(DrawingSurface drawingSurface, bool applyTransform)
    {
        int saved = 0;
        if (applyTransform)
        {
            saved = drawingSurface.Canvas.Save();
            ApplyTransformTo(drawingSurface);
        }

        using Paint shapePaint = new Paint() { IsAntiAliased = true };

        if (Radius.ShortestAxis < StrokeWidth)
        {
            shapePaint.Color = StrokeColor;
            shapePaint.Style = PaintStyle.Fill;
            drawingSurface.Canvas.DrawOval(Center, Radius, shapePaint);
        }
        else
        {
            shapePaint.Color = FillColor;
            shapePaint.Style = PaintStyle.Fill;
            drawingSurface.Canvas.DrawOval(Center, Radius, shapePaint);

            shapePaint.Color = StrokeColor;
            shapePaint.Style = PaintStyle.Stroke;
            shapePaint.StrokeWidth = StrokeWidth;
            drawingSurface.Canvas.DrawOval(Center, Radius - new VecD(StrokeWidth / 2f), shapePaint);
        }

        if (applyTransform)
        {
            drawingSurface.Canvas.RestoreToCount(saved);
        }
    }
    
    public override bool IsValid()
    {
        return Radius is { X: > 0, Y: > 0 };
    }

    public override int CalculateHash()
    {
        return HashCode.Combine(Center, Radius, StrokeColor, FillColor, StrokeWidth, TransformationMatrix);
    }

    public override int GetCacheHash()
    {
        return CalculateHash();
    }

    public override object Clone()
    {
        return new EllipseVectorData(Center, Radius)
        {
            StrokeColor = StrokeColor,
            FillColor = FillColor,
            StrokeWidth = StrokeWidth,
            TransformationMatrix = TransformationMatrix
        };
    }
}
