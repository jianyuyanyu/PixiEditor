﻿using PixiEditor.Extensions.CommonApi.FlyUI;
using PixiEditor.Extensions.Sdk.Attributes;

namespace PixiEditor.Extensions.Sdk.Api.FlyUI;

[ControlTypeId("Row")]
public class Row : MultiChildLayoutElement
{
    public MainAxisAlignment MainAxisAlignment { get; set; }
    public CrossAxisAlignment CrossAxisAlignment { get; set; }

    public Row(params LayoutElement[] children)
    {
        Children = new List<LayoutElement>(children);
        MainAxisAlignment = MainAxisAlignment.Start;
        CrossAxisAlignment = CrossAxisAlignment.Start;
    }

    public Row(
        MainAxisAlignment mainAxisAlignment = MainAxisAlignment.Start,
        CrossAxisAlignment crossAxisAlignment = CrossAxisAlignment.Start,
        LayoutElement[] children = null, Cursor? cursor = null) : base(cursor)
    {
        MainAxisAlignment = mainAxisAlignment;
        CrossAxisAlignment = crossAxisAlignment;
        Children = new List<LayoutElement>(children);
    }

    protected override ControlDefinition CreateControl()
    {
        ControlDefinition controlDefinition = new ControlDefinition(UniqueId, GetType());
        controlDefinition.AddProperty(MainAxisAlignment);
        controlDefinition.AddProperty(CrossAxisAlignment);
        controlDefinition.Children.AddRange(Children.Select(x => x.BuildNative()));

        return controlDefinition;
    }
}
