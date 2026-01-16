using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace Scheri.PETPanel.UIComponents;

public class NavigateTypeMessage
{
    public Type ViewType { get; }
    public NavigateTypeMessage(Type viewType)
    {
        if (!typeof(UserControl).IsAssignableFrom(viewType))
        {
            throw new ArgumentException("viewType must be Derived Type of Avalonia.Controls.Control");
        }
        ViewType = viewType;
    }
}

