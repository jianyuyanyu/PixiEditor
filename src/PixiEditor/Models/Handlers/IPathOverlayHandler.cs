﻿using Drawie.Backend.Core.Vector;

namespace PixiEditor.Models.Handlers;

public interface IPathOverlayHandler : IHandler
{
    public void Show(VectorPath path, bool showApplyButton, Action<VectorPath>? customAddToUndo = null);
    public void Hide();
    public event Action<VectorPath> PathChanged;
    public bool IsActive { get; }
    public bool HasUndo { get; }
    public bool HasRedo { get; }
    public void Undo();
    public void Redo();
}
