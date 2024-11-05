﻿using ChunkyImageLib.DataHolders;
using PixiEditor.Models.DocumentModels.UpdateableChangeExecutors.Features;
using PixiEditor.Models.Handlers;
using PixiEditor.Models.Tools;
using Drawie.Numerics;

namespace PixiEditor.Models.DocumentModels.UpdateableChangeExecutors;

/// <summary>
/// This class is responsible for handling the execution of a simple shape tool.
///  This executor handles: Shape tool state management and snapping
///  Drawing a shape can be either on raster layer or vector.
/// 
///  - Preview mode: a state when the tool is selected and editing of current shape is disabled or impossible. During this state,
///      snapping overlays are shown under mouse position, axes and snap point.
///  - Drawing mode: a state when the user clicked on the canvas and is dragging the mouse to draw a shape.
///        During this state, snapping axes are highlighted.
///  - Transform mode: a state when the user is transforming existing shape.
///     During this state, snapping axes are highlighted.
///
///     Possible state transitions:
///         - Preview -> Drawing (when user clicks on the canvas)
///         - Drawing -> Transform (when user releases the mouse after drawing)
///         - Transform -> Preview (when user applies the transform)
///         - Transform -> Drawing (when user clicks outside of shape transform bounds)
/// </summary>
internal abstract class SimpleShapeToolExecutor : UpdateableChangeExecutor, 
    ITransformableExecutor, IMidChangeUndoableExecutor
{
    private ShapeToolMode activeMode;

    protected ShapeToolMode ActiveMode
    {
        get => activeMode;
        set
        {
            StopMode(activeMode);
            activeMode = value;
            StartMode(activeMode);
        }
    }

    protected Guid memberId;
    protected VecD startDrawingPos;

    public override bool BlocksOtherActions => ActiveMode == ShapeToolMode.Drawing;

    public override ExecutionState Start()
    {
        IStructureMemberHandler? member = document?.SelectedStructureMember;

        if (member is null)
            return ExecutionState.Error;

        memberId = member.Id;

        if (controller.LeftMousePressed)
        {
            ActiveMode = ShapeToolMode.Drawing;
        }
        else
        {
            ActiveMode = ShapeToolMode.Preview;
        }

        document.SnappingHandler.Remove(memberId.ToString()); // This disables self-snapping

        return ExecutionState.Success;
    }

    protected virtual void StartMode(ShapeToolMode mode)
    {
        switch (mode)
        {
            case ShapeToolMode.Preview:
                break;
            case ShapeToolMode.Drawing:
                StartDrawingMode();
                break;
            case ShapeToolMode.Transform:
                break;
        }
    }

    private void StartDrawingMode()
    {
        startDrawingPos = SnapAndHighlight(controller.LastPrecisePosition);
    }

    protected virtual void StopMode(ShapeToolMode mode)
    {
        switch (mode)
        {
            case ShapeToolMode.Preview:
                break;
            case ShapeToolMode.Drawing:
                break;
            case ShapeToolMode.Transform:
                StopTransformMode();
                break;
        }
    }

    private void StopTransformMode()
    {
        document!.TransformHandler.HideTransform();
        document!.LineToolOverlayHandler.Hide();
    }

    public override void OnLeftMouseButtonDown(VecD pos)
    {
        if (ActiveMode == ShapeToolMode.Preview)
        {
            ActiveMode = ShapeToolMode.Drawing;
        }
    }

    public override void OnPrecisePositionChange(VecD pos)
    {
        if (ActiveMode == ShapeToolMode.Preview)
        {
            PrecisePositionChangePreviewMode(pos);
        }
        else if (ActiveMode == ShapeToolMode.Drawing)
        {
            PrecisePositionChangeDrawingMode(pos);
        }
        else if (ActiveMode == ShapeToolMode.Transform)
        {
            PrecisePositionChangeTransformMode(pos);
        }
    }

    public override void OnLeftMouseButtonUp()
    {
        HighlightSnapping(null, null);
        ActiveMode = ShapeToolMode.Transform;
    }

    public bool IsTransforming => ActiveMode == ShapeToolMode.Transform; 

    public virtual void OnTransformMoved(ShapeCorners corners)
    {
        
    }

    public virtual void OnTransformApplied()
    {
        ActiveMode = ShapeToolMode.Preview;
        AddMemberToSnapping();
        HighlightSnapping(null, null);
    }

    public virtual void OnLineOverlayMoved(VecD start, VecD end)
    {
    }

    public virtual void OnSelectedObjectNudged(VecI distance)
    {
    }

    public override void ForceStop()
    {
        StopMode(activeMode);
        AddMemberToSnapping();
        HighlightSnapping(null, null);
    }

    protected void HighlightSnapping(string? snapX, string? snapY)
    {
        document!.SnappingHandler.SnappingController.HighlightedXAxis = snapX;
        document!.SnappingHandler.SnappingController.HighlightedYAxis = snapY;
        document.SnappingHandler.SnappingController.HighlightedPoint = null;
    }

    protected void AddMemberToSnapping()
    {
        var member = document.StructureHelper.Find(memberId);
        document!.SnappingHandler.AddFromBounds(memberId.ToString(), () => member?.TightBounds ?? RectD.Empty);
    }
    
    protected VecD SnapAndHighlight(VecD pos)
    {
        VecD snapped = document.SnappingHandler.SnappingController.GetSnapPoint(pos, out string snapX, out string snapY);
        HighlightSnapping(snapX, snapY);
        return (VecI)snapped;
    }

    protected virtual void PrecisePositionChangePreviewMode(VecD pos)
    {
        VecD mouseSnap =
            document.SnappingHandler.SnappingController.GetSnapPoint(pos, out string snapXAxis,
                out string snapYAxis);
        HighlightSnapping(snapXAxis, snapYAxis);

        if (!string.IsNullOrEmpty(snapXAxis) || !string.IsNullOrEmpty(snapYAxis))
        {
            document.SnappingHandler.SnappingController.HighlightedPoint = mouseSnap;
        }
        else
        {
            document.SnappingHandler.SnappingController.HighlightedPoint = null;
        }
    }
    
    protected virtual void PrecisePositionChangeDrawingMode(VecD pos) { }
    protected virtual void PrecisePositionChangeTransformMode(VecD pos) { }
    public abstract void OnMidChangeUndo();
    public abstract void OnMidChangeRedo();
    public abstract bool CanUndo { get; } 
    public abstract bool CanRedo { get; }

    public bool IsFeatureEnabled(IExecutorFeature feature)
    {
        if (feature is ITransformableExecutor)
        {
            return IsTransforming;
        }
        
        if (feature is IMidChangeUndoableExecutor)
        {
            return ActiveMode == ShapeToolMode.Transform;
        }
        
        return false;
    }
}

enum ShapeToolMode
{
    Preview,
    Drawing,
    Transform
}