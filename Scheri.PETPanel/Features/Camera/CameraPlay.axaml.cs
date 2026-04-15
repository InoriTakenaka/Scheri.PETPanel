using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;

namespace Scheri.PETPanel.Features;

public partial class CameraPlay : UserControl
{
    public CameraPlay()
    {
        InitializeComponent();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        CameraView.SetMediaPath("rtsp://192.168.1.244:554");
    }
}