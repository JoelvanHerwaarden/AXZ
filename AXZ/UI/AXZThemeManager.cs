using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;
using System.Windows;

namespace AXZ.UI
{
    public class AXZThemeManager
    {
        public static AXZThemeManager Current;
        public List<AXZRibbonButton> Buttons; 
        public AXZThemeManager(AXZApplication application, List<AXZRibbonButton> buttons)
        {
            Current = this;
            Buttons = buttons;
        }
        private static ResourceDictionary GetThemeResourceDictionary()
        {
            ResourceDictionary resources = new ResourceDictionary();
            string result = "LightModeStyles.xaml";
            if (Autodesk.Revit.UI.UIThemeManager.CurrentTheme == UITheme.Dark)
            {
                result = "DarkModeStyles.xaml";
            }
            resources.Source = new Uri(string.Format("/AXZ;component/Resources/{0}", result), UriKind.RelativeOrAbsolute);

            return resources;
        }
        public static void ApplyTheme(Window window)
        {
            window.Resources.MergedDictionaries.Clear();
            window.Resources.MergedDictionaries.Add(GetThemeResourceDictionary());
        }
        public void UpdateIcons()
        {
            foreach(AXZRibbonButton button in  Buttons)
            {
                if (button.SupportsThemeChange())
                {
                    button.ChangeIcon();
                }
            }
        }
    }
}
