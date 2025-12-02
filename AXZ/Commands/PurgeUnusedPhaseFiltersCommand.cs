using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace AXZ.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class PurgeUnusedPhaseFiltersCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document document = commandData.Application.ActiveUIDocument.Document;
            IList<ElementId> filterIds = new FilteredElementCollector(document).OfClass(typeof(ParameterFilterElement))
                .ToElements()
                .Where(e => e.Name.StartsWith("SP_ViewPhase"))
                .Select(e => e.Id)
                .ToList();
            IList<ElementId> purgableFilterIds = new List<ElementId>(); 
            foreach (ElementId filterId in filterIds)
            {
                bool found = false;
                foreach (Autodesk.Revit.DB.View view in new FilteredElementCollector(document).OfClass(typeof(Autodesk.Revit.DB.View)).ToElements())
                {
                    try
                    {
                        if (view.GetFilters().Contains(filterId))
                        {
                            found = true;
                            break;
                        }
                    }
                    catch { continue; } // Some views may not support filters
                }
                if (!found)
                {
                    purgableFilterIds.Add(filterId);
                }
            }
            if(purgableFilterIds.Count > 0)
            {
                if(TaskDialogResult.Yes == Autodesk.Revit.UI.TaskDialog.Show("Purge Unused Phase Filters", 
                    $"This will remove {purgableFilterIds.Count} unused phase filters from the document. Do you want to proceed?", 
                    TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No))
                {
                    using (Transaction transaction = new Transaction(document, "Remove Unused Phase Filters"))
                    {
                        transaction.Start();
                        document.Delete(purgableFilterIds);
                        transaction.Commit();
                    }
                }
            }
            else
            {
                Autodesk.Revit.UI.TaskDialog.Show("Purge Unused Phase Filters", "No unused phase filters found in the document.");
            }
            return Result.Succeeded;
        }
    }
}
