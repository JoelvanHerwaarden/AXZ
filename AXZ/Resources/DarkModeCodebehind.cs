using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AXZ.Resources
{
    public partial class DarkStylesCodeBehind
    {
        private void PrimaryButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            System.Windows.Controls.Button button = (System.Windows.Controls.Button)sender;
            System.Windows.Media.Brush background = button.Background;
            System.Windows.Media.Brush foreground = button.Foreground;
            if (foreground.IsFrozen)
            {
                foreground = foreground.Clone();
            }
            button.Foreground = background;
            button.Background = foreground;
        }
        private void PrimaryButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            System.Windows.Controls.Button button = (System.Windows.Controls.Button)sender;
            System.Windows.Media.Brush foreground = button.Background;
            if (foreground.IsFrozen)
            {
                foreground = foreground.Clone();
            }
            System.Windows.Media.Brush background = button.Foreground;
            button.Foreground = foreground;
            button.Background = background;
        }
        private void DotButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button button = (System.Windows.Controls.Button)sender;
            string text = button.Content.ToString();
            System.Windows.Forms.Clipboard.SetText(text);

        }

        private void GiveFocusToCell_Event(object sender, RoutedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            dg.BeginEdit();
        }
    }
}
