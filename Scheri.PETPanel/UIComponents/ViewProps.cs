using Avalonia.Controls;
using Avalonia;

namespace Scheri.PETPanel.UIComponents
{
    public class ViewProps:AvaloniaObject
    {
        public static readonly AttachedProperty<string> TitleProperty =
            AvaloniaProperty.RegisterAttached<UserControl, string>("Title",typeof(ViewProps), "PET PANEL");
        public static string GetTitle(UserControl control)
        {
            return control.GetValue(TitleProperty);
        }

        public static void SetTitle(UserControl control, string value)
        {
            control.SetValue(TitleProperty, value);
        }
    }
}
