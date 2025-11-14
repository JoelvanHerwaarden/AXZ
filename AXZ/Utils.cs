using Autodesk.Internal.InfoCenter;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using adskWindows = Autodesk.Windows;

namespace AXZ
{
    public class Utils
    {
        /// <summary>
        /// Retrieves the main Revit application window as a WPF Window object.
        /// Useful for setting the owner of custom dialogs or integrating UI elements with Revit's main window.
        /// </summary>
        /// <param name="commandData">Provides access to the Revit application and its main window handle.</param>
        /// <returns>The main Revit application window as a WPF Window object.</returns>
        public static Window RevitWindow(ExternalCommandData commandData)
        {
            IntPtr RevitWindowHandle = commandData.Application.MainWindowHandle;
            HwndSource hwndSource = HwndSource.FromHwnd(RevitWindowHandle);
            Window RevitWindow = hwndSource.RootVisual as Window;
            return RevitWindow;
        }
        public static void Show(string message)
        {
            Autodesk.Revit.UI.TaskDialog taskDialog = new Autodesk.Revit.UI.TaskDialog("Revit")
            {
                ExpandedContent = message,
                MainContent = message,
                MainIcon = Autodesk.Revit.UI.TaskDialogIcon.TaskDialogIconInformation
            };
            Autodesk.Revit.UI.TaskDialog.Show("Revit", message);
        }
        /// <summary>
        /// Shows an information balloon in the Revit InfoCenter with the specified message.
        /// </summary>
        /// <param name="message"></param>
        public static void ShowInfoBalloon(string message)
		{
			ResultItem result = new ResultItem
			{
				Title = message,
				Category = "AXZ Addin",
				IsNew = true,
				Timestamp = DateTime.Now
			};

			adskWindows.ComponentManager.InfoCenterPaletteManager.ShowBalloon(result);
		}
	}
}
