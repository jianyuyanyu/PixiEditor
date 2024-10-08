﻿namespace PixiEditor.Models.Handlers;

internal interface IToolSetHandler : IHandler
{
    public string Name { get; }
    public ICollection<IToolHandler> Tools { get; }
}
