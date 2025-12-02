using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace AXZ.Commands
{
    public class PhaseParametersUpdater : IUpdater
    {
        public static PhaseParametersUpdater Current;
        public static readonly string UpdaterName = "Phase Parameters Updater";
        public static readonly string UpdaterInfo = String.Format("Updates phase code parameters based on individual phase parameters." +
            "Updater Name = {0}" +
            "Updater Version = 1.0", UpdaterName);
        public static readonly Guid UpdaterGuid = new Guid("db005d6e-d5b6-429a-80d8-066cf05520c6");

        public UpdaterId _updaterId;


        public PhaseParametersUpdater(AddInId addinId)
        {
            _updaterId = new UpdaterId(addinId, UpdaterGuid);
            Current = this;
        }

        public void Execute(UpdaterData data)
        {
            Document doc = data.GetDocument();
            foreach (ElementId id in data.GetModifiedElementIds())
            {
                Element elem = doc.GetElement(id);
                if (elem == null) continue;
                Parameter p_Create1 = elem.LookupParameter("SP_PhaseCreated 1");
                Parameter p_Create2 = elem.LookupParameter("SP_PhaseCreated 2");
                Parameter p_Create3 = elem.LookupParameter("SP_PhaseCreated 3");
                Parameter p_CreateCode = elem.LookupParameter("SP_PhaseCreated[L]");
                if (p_Create1 != null && p_Create2 != null && p_Create3 != null)
                {
                    string combinedValue = $"{p_Create1.AsString()}-{p_Create2.AsString()}-{p_Create3.AsString()}";
                    if(combinedValue != "--")
                    {
                        p_CreateCode.Set(combinedValue);
                    }
                }

                Parameter p_Demo1 = elem.LookupParameter("SP_PhaseDemolished 1");
                Parameter p_Demo2 = elem.LookupParameter("SP_PhaseDemolished 2");
                Parameter p_Demo3 = elem.LookupParameter("SP_PhaseDemolished 3");
                Parameter p_DemoCode = elem.LookupParameter("SP_PhaseDemolished[L]");
                if (p_Demo1 != null && p_Demo2 != null && p_Demo3 != null)
                {
                    if (p_Demo1.AsValueString() != "" || p_Demo2.AsValueString() != "" || p_Demo3.AsValueString() != "")
                    {
                        if(p_Demo1.AsValueString() == "")
                        {
                            p_Demo1.Set("000");
                        }
                        if (p_Demo2.AsValueString() == "")
                        {
                            p_Demo2.Set("000");

                        }
                        if (p_Demo3.AsValueString() == "")
                        {
                            p_Demo3.Set("000");

                        }
                        string combinedValueDemo = $"{p_Demo1.AsString()}-{p_Demo2.AsString()}-{p_Demo3.AsString()}";
                        if(combinedValueDemo != "--")
                        {
                            p_DemoCode.Set(combinedValueDemo); // or compute based on other params
                        }
                    }
                }
            }
            Utils.ShowInfoBalloon("Phase codes updated for modified elements.");
        }

        public string GetAdditionalInformation() => "Parameter updater";
        public ChangePriority GetChangePriority() => ChangePriority.Annotations;
        public UpdaterId GetUpdaterId() => _updaterId;
        public string GetUpdaterName() => UpdaterName;
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class RegisterAssignPhaseCodesCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication app = commandData.Application;
            //Create new Updater and Register it to the Revit App
            UpdaterRegistry.RegisterUpdater(new PhaseParametersUpdater(app.ActiveAddInId));

            //Build Triggers on when the Updater will run
            List<string> parameterNames = new List<string>
            {
                "SP_PhaseCreated 1",
                "SP_PhaseCreated 2",
                "SP_PhaseCreated 3",
                "SP_PhaseDemolished 1",
                "SP_PhaseDemolished 2",
                "SP_PhaseDemolished 3"
            };

            Document doc = app.ActiveUIDocument.Document;
            ElementIsElementTypeFilter filter = new ElementIsElementTypeFilter(true);
            List<SharedParameterElement> parameters = new FilteredElementCollector(doc)
                .OfClass(typeof(SharedParameterElement))
                .Cast<SharedParameterElement>()
                .Where(sp => parameterNames.Contains(sp.Name))
                .ToList();



            foreach(var param in parameters)
            {
                UpdaterRegistry.AddTrigger(
                    new PhaseParametersUpdater(app.ActiveAddInId).GetUpdaterId(),
                    filter,
                    Element.GetChangeTypeParameter(param.Id)
                );
            }

            Utils.ShowInfoBalloon("Phase Parameters Updater registered.");
            return Result.Succeeded;
        }
    }
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class UnRegisterAssignPhaseCodesCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication app = commandData.Application;

            PhaseParametersUpdater dummy = new PhaseParametersUpdater(app.ActiveAddInId);
            UpdaterRegistry.UnregisterUpdater(dummy.GetUpdaterId());
            Utils.ShowInfoBalloon("Phase Parameters Updater unregistered.");
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ManualUpdatePhaseCodesCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication app = commandData.Application;

            Utils.ShowInfoBalloon("Phase Parameters Updated.");
            return Result.Succeeded;
        }
    }


}

