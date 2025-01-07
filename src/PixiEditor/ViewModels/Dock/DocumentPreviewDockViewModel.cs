﻿using Avalonia.Media;
using PixiEditor.Helpers.Converters;
using PixiEditor.Extensions.Common.Localization;
using PixiEditor.UI.Common.Fonts;
using PixiEditor.ViewModels.Document;
using PixiEditor.ViewModels.SubViewModels;

namespace PixiEditor.ViewModels.Dock;

internal class DocumentPreviewDockViewModel : DockableViewModel
{
    public const string TabId = "DocumentPreview";

    public override string Id => TabId;
    public override string Title => new LocalizedString("PREVIEW_TITLE");
    public override bool CanFloat => true;
    public override bool CanClose => true;

    private ColorsViewModel colorsSubViewModel;

    public ColorsViewModel ColorsSubViewModel
    {
        get => colorsSubViewModel;
        set => SetProperty(ref colorsSubViewModel, value);
    }

    private DocumentManagerViewModel documentManagerSubViewModel;

    public DocumentManagerViewModel DocumentManagerSubViewModel
    {
        get => documentManagerSubViewModel;
        set => SetProperty(ref documentManagerSubViewModel, value);
    }

    public DocumentPreviewDockViewModel(ColorsViewModel colorsSubViewModel, DocumentManagerViewModel documentManagerViewModel)
    {
        ColorsSubViewModel = colorsSubViewModel;
        DocumentManagerSubViewModel = documentManagerViewModel;
        TabCustomizationSettings.Icon = PixiPerfectIcons.ToIcon(PixiPerfectIcons.Compass);
    }
}