using System;
using System.Collections.Generic;
using System.Text;
using Autodesk.Revit.DB;
using DB = Autodesk.Revit.DB;

namespace AXZ
{
    public class PhaseFilterOverride
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public OverrideGraphicSettings OGSNew { get; private set; }
        public OverrideGraphicSettings OGSDemo { get; private set; }
        public OverrideGraphicSettings OGSTemp { get; private set; }
        public OverrideGraphicSettings OGSTempCurrentPhase { get; private set; }


        public bool NewVisible { get; private set; } 
        public bool DemoVisible { get; private set; }
        public bool TempVisible { get; private set; }
        public bool TempinPhaseVisible { get; private set; }


        public PhaseFilterOverride(string name, 
            string description, 
            OverrideGraphicSettings OverrideGraphicSettingsNew, 
            OverrideGraphicSettings OverrideGraphicSettingsDemolished, 
            OverrideGraphicSettings OverrideGraphicSettingsTemporary,
            OverrideGraphicSettings OverrideGraphicSettingsTemporaryCurrentPhase,
            bool NewObjectsVisible = true,
            bool DemolishedObjectsVisible = true,
            bool TemporaryObjectsVisible = true,
            bool OnePhaseTemporaryObjectsVisible = true)
        {
            Name = name;
            Description = description;
            OGSNew = OverrideGraphicSettingsNew;
            OGSDemo = OverrideGraphicSettingsDemolished;
            OGSTemp = OverrideGraphicSettingsTemporary;
            OGSTempCurrentPhase = OverrideGraphicSettingsTemporaryCurrentPhase;
            NewVisible= NewObjectsVisible;
            DemoVisible = DemolishedObjectsVisible;
            TempVisible = TemporaryObjectsVisible;
            TempinPhaseVisible = OnePhaseTemporaryObjectsVisible;   
        }

        public static PhaseFilterOverride RGB(Document document)
        {
            DB.Color New = new DB.Color(0, 255, 0);
            DB.Color Demo = new DB.Color(255, 0, 0);
            DB.Color TempInPhase = new DB.Color(0, 0, 255);
            DB.Color TempOverall = new DB.Color(0, 255, 255);


            //NEW OVERRIDES
            OverrideGraphicSettings graphicsNew = new OverrideGraphicSettings();
            graphicsNew.SetProjectionLineColor(New);
            graphicsNew.SetCutLineColor(New);

            //TEMP IN PHASE OVERRIDES
            OverrideGraphicSettings graphicsTempInPhase = new OverrideGraphicSettings();
            graphicsTempInPhase.SetProjectionLineColor(TempInPhase);
            graphicsTempInPhase.SetCutLineColor(TempInPhase);

            //DEMO OVERRIDES
            OverrideGraphicSettings graphicsDemo = new OverrideGraphicSettings();
            graphicsDemo.SetProjectionLineColor(Demo);
            graphicsDemo.SetCutLineColor(Demo);

            //TEMP OVERRIDES
            OverrideGraphicSettings graphicsTemp = new OverrideGraphicSettings();
            graphicsTemp.SetProjectionLineColor(TempOverall);
            graphicsTemp.SetCutLineColor(TempOverall);

            return new PhaseFilterOverride("RGBC: Demo (Red), New (Green), Temp (Blue), Temp in Phase (Cyan)",
                "Display new objects in Green, Demolished in Red, Temporary in Cyan and One phase Temporary in Blue",
                graphicsNew, graphicsDemo, graphicsTemp, graphicsTempInPhase);
        }
        public static PhaseFilterOverride RGBSolid(Document document)
        {
            DB.Color New = new DB.Color(0, 255, 0);
            DB.Color Demo = new DB.Color(255, 0, 0);
            DB.Color TempInPhase = new DB.Color(0, 0, 255);
            DB.Color TempOverall = new DB.Color(0, 255, 255);

            ElementId solidFillPattern = new FilteredElementCollector(document)
                        .OfClass(typeof(FillPatternElement))
                        .Where(FillPatternElement => FillPatternElement.Name.Equals("<Solid fill>"))
                        .Cast<FillPatternElement>()
                        .First()
                        .Id;

            //NEW OVERRIDES
            OverrideGraphicSettings graphicsNew = new OverrideGraphicSettings();
            graphicsNew.SetCutForegroundPatternColor(New);
            graphicsNew.SetSurfaceForegroundPatternColor(New);
            graphicsNew.SetCutForegroundPatternId(solidFillPattern);
            graphicsNew.SetSurfaceForegroundPatternId(solidFillPattern);

            //TEMP IN PHASE OVERRIDES
            OverrideGraphicSettings graphicsTempInPhase = new OverrideGraphicSettings();
            graphicsTempInPhase.SetCutForegroundPatternColor(TempInPhase);
            graphicsTempInPhase.SetSurfaceForegroundPatternColor(TempInPhase);
            graphicsTempInPhase.SetCutForegroundPatternId(solidFillPattern);
            graphicsTempInPhase.SetSurfaceForegroundPatternId(solidFillPattern);

            //DEMO OVERRIDES
            OverrideGraphicSettings graphicsDemo = new OverrideGraphicSettings();
            graphicsDemo.SetCutForegroundPatternColor(Demo);
            graphicsDemo.SetSurfaceForegroundPatternColor(Demo);
            graphicsDemo.SetCutForegroundPatternId(solidFillPattern);
            graphicsDemo.SetSurfaceForegroundPatternId(solidFillPattern);

            //TEMP OVERRIDES
            OverrideGraphicSettings graphicsTemp = new OverrideGraphicSettings();
            graphicsTemp.SetCutForegroundPatternColor(TempOverall);
            graphicsTemp.SetSurfaceForegroundPatternColor(TempOverall);
            graphicsTemp.SetCutForegroundPatternId(solidFillPattern);
            graphicsTemp.SetSurfaceForegroundPatternId(solidFillPattern);

            return new PhaseFilterOverride("RGBC Solid: Demo (Red), New (Green), Temp (Blue), Temp in Phase (Cyan)",
                "Display new objects in Green, Demolished in Red, Temporary in Cyan and One phase Temporary in Blue",
                graphicsNew, graphicsDemo, graphicsTemp, graphicsTempInPhase);
        }
        public static PhaseFilterOverride ShowCurrent(Document document)
        {
            //DEFAULT OVERRIDES
            OverrideGraphicSettings defaultGraphics = new OverrideGraphicSettings();

            return new PhaseFilterOverride("Show Current: Show Only what is visible in the phase, Demolished not visible",
                "Display Only the objects which exist in the phase. Objects demolished in this phase are not shown",
                defaultGraphics, defaultGraphics, defaultGraphics, defaultGraphics,
                true, false, true, true);
        }

    }
}
