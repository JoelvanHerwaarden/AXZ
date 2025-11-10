using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AXZ.UI
{
    public class AXZRibbonButton
    {
        private RibbonButton _ribbonButton { get; set; }
        private bool _supportsThemeChange { get; set; }
        public AXZRibbonButton(RibbonButton ribbonButton, bool SupportsThemeChange = false)
        {
            this._ribbonButton = ribbonButton;
            this._supportsThemeChange = SupportsThemeChange;
        }
        public bool SupportsThemeChange()
        {
            return _supportsThemeChange;
        }
        public RibbonButton RibbonItem()
        {
            return this._ribbonButton;
        }

        public void ChangeIcon()
        {
            try
            {
                string themeSuffix = "dark";
                string themeNewSuffix = "light";
                Debug.Log("Current Theme: " + UIThemeManager.CurrentTheme.ToString());

                var theme = UIThemeManager.CurrentTheme;
                if (theme == UITheme.Dark)
                {
                    themeSuffix = "light";
                    themeNewSuffix = "dark";
                }

                ImageSource image = this._ribbonButton.Image;

                ImageSource largeImage = this._ribbonButton.LargeImage;

                if (image != null)
                {
                    BitmapSource source = (BitmapSource)image;
                    string sourcePath = source.ToString().Replace("file:///", "");
                    Debug.Log("Image source path: "+sourcePath);
                    string newSourcePath = sourcePath.Replace(themeSuffix, themeNewSuffix);
                    Debug.Log("Image new source path: " + newSourcePath);

                    BitmapImage bitmap = new BitmapImage(new Uri(newSourcePath));
                    this._ribbonButton.Image = bitmap;
                }

                if (largeImage != null)
                {
                    BitmapSource source = (BitmapSource)largeImage;
                    string sourcePath = source.ToString().Replace("file:///", "");
                    Debug.Log("Image source path: " + sourcePath);
                    string newSourcePath = sourcePath.Replace(themeSuffix, themeNewSuffix);
                    Debug.Log("Image new source path: " + newSourcePath);
                    BitmapImage bitmap = new BitmapImage(new Uri(newSourcePath));
                    this._ribbonButton.LargeImage = bitmap;
                }
            }
            catch(Exception ex)
            {
                Debug.Log(ex.Message, LogLevel.Error);
            }
            
        }
    }
}
