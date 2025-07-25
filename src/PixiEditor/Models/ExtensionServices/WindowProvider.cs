﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PixiEditor.Extensions.CommonApi.Windowing;
using PixiEditor.Extensions.Helpers;
using PixiEditor.Extensions.Runtime;
using PixiEditor.Extensions.Windowing;
using PixiEditor.UI.Common.Localization;
using PixiEditor.Views.Dialogs;

namespace PixiEditor.Models.ExtensionServices;

public class WindowProvider : IWindowProvider
{
    private readonly Dictionary<string, Type> registeredWindows = new();
    private readonly Dictionary<string, List<Action<IPopupWindow>>> windowOpenedCallbacks = new();
    private ExtensionLoader extensionLoader;
    private IServiceProvider services;

    internal WindowProvider(ExtensionLoader loader, IServiceProvider services)
    {
        this.extensionLoader = loader;
        this.services = services;
        PixiEditorPopup.PopupLoaded += PixiEditorPopupOnPopupLoaded;
        PixiEditorPopup.PopupClosed += PixiEditorPopupOnPopupClosed;
    }

    private void PixiEditorPopupOnPopupLoaded(PixiEditorPopup obj)
    {
        string id = extensionLoader.GetTypeId(obj.GetType());
        PopupWindow popupWindow = new PopupWindow(obj);
        if(windowOpenedCallbacks.TryGetValue(id, out List<Action<IPopupWindow>> actions))
        {
            foreach (var action in actions)
            {
                action?.Invoke(popupWindow);
            }
        }
    }

    private void PixiEditorPopupOnPopupClosed(PixiEditorPopup obj)
    {

    }

    public WindowProvider RegisterWindow<T>() where T : IPopupWindow
    {
        Type type = typeof(T);
        string? id = extensionLoader.GetTypeId(type);
        if (id is null)
        {
            throw new ArgumentException($"Window {type} doesn't seem to be part of an extension.");
        }

        if (!registeredWindows.TryAdd(id, type))
        {
            throw new ArgumentException($"Window with id {id} is already registered.");
        }

        return this;
    }

    public IPopupWindow CreatePopupWindow(string title, object body)
    {
        return new PopupWindow(new PixiEditorPopup { Title = title, Content = body });
    }

    public IPopupWindow GetWindow(BuiltInWindowType type)
    {
        string id = type.GetDescription();
        return GetWindow($"PixiEditor.{id}");
    }

    public IPopupWindow GetWindow(string windowId)
    {
        if (registeredWindows.TryGetValue(windowId, out Type? handler))
        {
            object[] args = TryGetConstructorArgs(handler);
            return new PopupWindow((IPopupWindow)Activator.CreateInstance(handler, args));
        }

        throw new ArgumentException($"Window with id {windowId} does not exist");
    }

    public void SubscribeWindowOpened(BuiltInWindowType type, Action<IPopupWindow> action)
    {
        string id = type.GetDescription();
        string fullId = $"PixiEditor.{id}";
        if (registeredWindows.ContainsKey(fullId))
        {
            if (!windowOpenedCallbacks.TryGetValue(fullId, out List<Action<IPopupWindow>> actions))
            {
                actions = new List<Action<IPopupWindow>>();
                windowOpenedCallbacks[fullId] = actions;
            }

            actions.Add(action);
        }
        else
        {
            throw new ArgumentException($"Window with id {id} does not exist");
        }
    }

    private object?[] TryGetConstructorArgs(Type handler)
    {
        ConstructorInfo[] constructors = handler.GetConstructors();
        if (constructors.Length == 0)
        {
            return Array.Empty<object>();
        }

        ConstructorInfo constructor = constructors[0];
        ParameterInfo[] parameters = constructor.GetParameters();
        if (parameters.Length == 0)
        {
            return Array.Empty<object>();
        }

        return parameters.Select(x => services.GetService(x.ParameterType)).ToArray();
    }
}
