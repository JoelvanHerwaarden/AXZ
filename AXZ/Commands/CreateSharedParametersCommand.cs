using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AXZ.Models;
using AXZ2025;

namespace AXZ.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CreateSharedParametersCommand : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            Autodesk.Revit.ApplicationServices.Application app = uiApp.Application;
            Document doc = uiApp.ActiveUIDocument.Document;

            string sharedParamFilePath = CollectPhaseParameters.CreateSharedParameterFilePath();
            List<string> parameterNames = new List<string>
            {
                "SP_PhaseCreated 1",
                "SP_PhaseCreated 2",
                "SP_PhaseCreated 3",
                "SP_PhaseDemolished[L]",
                "SP_PhaseDemolished 1",
                "SP_PhaseDemolished 2",
                "SP_PhaseDemolished 3",
                "SP_PhaseCreated[L]"
            };
            string viewParameterName = "SP_ViewPhase";
            List<BuiltInCategory> categories = CollectPhaseParameters.ParseToBuiltInCategory(CollectPhaseParameters.GetPhaseParameterCategories(doc));

            app.SharedParametersFilename = sharedParamFilePath;
            DefinitionFile sharedParamFile = app.OpenSharedParameterFile();

            if (sharedParamFile == null)
            {
                Autodesk.Revit.UI.TaskDialog.Show("Error", "Shared parameter file not found or invalid.");
                return Result.Failed;
            }

            List<string> createdParams = new();
            string resultMsg = "";  
            using (Transaction tx = new(doc, "Create AXZ Shared Parameters for Phasing"))
            {
                tx.Start();

                foreach (string paramName in parameterNames)
                {
                    Definition definition = FindDefinition(sharedParamFile, paramName);
                    if (definition == null)
                    {
                        resultMsg += string.Format("Parameter definition for '{0}' not found in the shared parameter file.\n", paramName);
                     }

                    CategorySet catSet = CreateCategorySet(doc, categories);
                    InstanceBinding binding = app.Create.NewInstanceBinding(catSet);

                    if (doc.ParameterBindings.Insert(definition, binding, GroupTypeId.Phasing))
                    {
                        resultMsg += string.Format("Parameter '{0}' created successfully.\n", paramName);
                    }
                    else
                    {
                        resultMsg += string.Format("Parameter '{0}' already exists or failed to create.\n", paramName);
                    }
                }

                Definition viewParameterDefinition = FindDefinition(sharedParamFile, viewParameterName);
                if (viewParameterDefinition != null)
                {
                    CategorySet ViewCatSet = new CategorySet();
                    ViewCatSet.Insert(doc.Settings.Categories.get_Item(BuiltInCategory.OST_Views));
                    InstanceBinding viewBinding = app.Create.NewInstanceBinding(ViewCatSet);

                    if (doc.ParameterBindings.Insert(viewParameterDefinition, viewBinding, GroupTypeId.Phasing))
                    {
                        resultMsg += string.Format("Parameter '{0}' created successfully.\n", viewParameterName);
                    }
                    else
                    {
                        resultMsg += string.Format("Parameter '{0}' already exists or failed to create.\n", viewParameterName);
                    }
                }
                else
                {
                    resultMsg += string.Format("Parameter definition for '{0}' not found in the shared parameter file.\n", viewParameterName);
                }


                tx.Commit();
            }

            Autodesk.Revit.UI.TaskDialog.Show("Create Shared Parameters Phasing", resultMsg);
            
            System.IO.File.Delete(sharedParamFilePath);
            return Result.Succeeded;
        }

        private static Definition FindDefinition(DefinitionFile file, string paramName)
        {
            return file.Groups
                       .Cast<DefinitionGroup>()
                       .Select(g => g.Definitions.get_Item(paramName))
                       .FirstOrDefault(def => def != null);
        }

        private static CategorySet CreateCategorySet(Document doc, IEnumerable<BuiltInCategory> categories)
        {
            CategorySet set = new();
            foreach (var bic in categories)
            {
                Category cat = doc.Settings.Categories.get_Item(bic);
                if (cat != null) set.Insert(cat);
            }
            return set;
        }
    }
}
