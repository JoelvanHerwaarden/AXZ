using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace AXZ
{
    public  class AXZApplication : IExternalApplication
    {
        private static RibbonPanel PhasingPanel { get; set; }
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            string tabName = "AXZ";
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            string versionNumber = string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
            application.CreateRibbonTab(tabName);
            PhasingPanel = application.CreateRibbonPanel(tabName, string.Format("Phasing {0}", versionNumber));
            CreateRibbonButton("Create\nParameters", "AXZCreatePhasingParameters", "AXZ.Commands.CreateSharedParametersCommand", PhasingPanel, "PhaseCreateParameters.bmp", "Create Phasing Parameters used in the project");
            PushButton button = CreateRibbonButton("Update\nCodes", "AXZUpdateLockedPhasingCodes", "AXZ.Commands.ManualUpdatePhaseCodesCommand", PhasingPanel, "PhaseAssignPhaseCodes.bmp", "Update the Locked parameter for all elements with Phase parameter filled in");
            button.Enabled = false; // Disabled until further notice
            CreateRibbonButton("Create\nFilters", "AXZCreatePhaseFilters", "AXZ.Commands.CreatePhaseFiltersCommand", PhasingPanel, "PhaseCreateFilters.bmp", "Create Phase filters for the current view.");
            CreateRibbonButton("Purge\nFilters", "AXZPurgePhaseFilters", "AXZ.Commands.PurgeUnusedPhaseFiltersCommand", PhasingPanel, "PhasePurgeFilters.bmp", "Create Phase filters for the current view.");
            return Result.Succeeded;
        }
        private PushButton CreateRibbonButton(string Title, string buttonName, string Command, RibbonPanel panel, string imageName = null, string toolTip = null)
        {
            string assemblyPath = Assembly.GetExecutingAssembly().Location;
            string folderPath = Path.GetDirectoryName(assemblyPath);
            PushButtonData pushButtonData = new PushButtonData(
                buttonName,
                Title,
                assemblyPath,
                Command);
            PushButton pushButton = panel.AddItem(pushButtonData) as PushButton;
            if (imageName != null)
            {
                string pictureUri = string.Format(Path.Combine(folderPath, "Resources", imageName));
                BitmapImage bitmap = new BitmapImage(new Uri(pictureUri));
                pushButton.LargeImage = bitmap;
            }

            if (toolTip != null)
            {
                pushButton.ToolTip = toolTip;
            }

            return pushButton;
        }
    }
}
