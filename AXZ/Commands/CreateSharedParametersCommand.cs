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
using DB = Autodesk.Revit.DB;

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
                Debug.Log("Shared parameter file not found or invalid.");
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
                    Definition definition = CollectPhaseParameters.FindDefinition(sharedParamFile, paramName);
                    
                    if (definition == null)
                    {
                        string defMsg = string.Format("Parameter definition for '{0}' not found in the shared parameter file.\n", paramName);
                        Debug.Log(defMsg);
                        resultMsg += defMsg;
                    }
                    if (!CollectPhaseParameters.DoesParameterExists(doc, paramName, definition as ExternalDefinition))
                    {
                        CategorySet catSet = CollectPhaseParameters.CreateCategorySet(doc, categories);
                        InstanceBinding binding = app.Create.NewInstanceBinding(catSet);

                        if (doc.ParameterBindings.Insert(definition, binding, GroupTypeId.Phasing))
                        {
                            string msg = string.Format("Parameter '{0}' created successfully.\n", paramName);
                            Debug.Log(msg);
                            resultMsg += msg;
                        }
                    }

                    else
                    {
                        string msg = string.Format("Parameter '{0}' already exists or failed to create.\n", paramName);
                        Debug.Log(msg);
                        resultMsg += msg;
                    }

                }

                Definition viewParameterDefinition = CollectPhaseParameters.FindDefinition(sharedParamFile, viewParameterName);
                if (viewParameterDefinition != null)
                {
                    if(!CollectPhaseParameters.DoesParameterExists(doc, viewParameterName, viewParameterDefinition as ExternalDefinition))
                    {
                        CategorySet ViewCatSet = new CategorySet();
                        ViewCatSet.Insert(doc.Settings.Categories.get_Item(BuiltInCategory.OST_Views));
                        InstanceBinding viewBinding = app.Create.NewInstanceBinding(ViewCatSet);

                        if (doc.ParameterBindings.Insert(viewParameterDefinition, viewBinding, GroupTypeId.Phasing))
                        {
                            string msg = string.Format("Parameter '{0}' created successfully.\n", viewParameterName);
                            Debug.Log(msg);
                            resultMsg += msg;
                        }
                    }
                    
                    else
                    {
                        string msg = string.Format("Parameter '{0}' already exists or failed to create.\n", viewParameterName);
                        Debug.Log(msg);
                        resultMsg += msg;
                    }
                }
                else
                {
                    string msg = string.Format("Parameter definition for '{0}' not found in the shared parameter file.\n", viewParameterName); 
                    Debug.Log(msg);
                    resultMsg += msg;
                }


                tx.Commit();
            }
            if(resultMsg != "")
            {
                Autodesk.Revit.UI.TaskDialog.Show("Create Shared Parameters Phasing", resultMsg);
            }
            else
            {
                Utils.Show("All shared parameters for phasing already exist.");
            }

            System.IO.File.Delete(sharedParamFilePath);

            CollectPhaseParameters.RepairPhasingParameterBindings(doc);
            return Result.Succeeded;
        }
        
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class RepairSharedParametersCommand : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            CollectPhaseParameters.RepairPhasingParameterBindings(doc);
            List<ParameterElement> parameters = new List<ParameterElement>()
            {
                CollectPhaseParameters.GetSP_PhaseCreated1(commandData.Application.ActiveUIDocument.Document),
                CollectPhaseParameters.GetSP_PhaseCreated2(commandData.Application.ActiveUIDocument.Document),
                CollectPhaseParameters.GetSP_PhaseCreated3(commandData.Application.ActiveUIDocument.Document),
                CollectPhaseParameters.GetSharedParameterByName(commandData.Application.ActiveUIDocument.Document, "SP_PhaseCreated[L]"),
                CollectPhaseParameters.GetSP_PhaseDemolished1(commandData.Application.ActiveUIDocument.Document),
                CollectPhaseParameters.GetSP_PhaseDemolished2(commandData.Application.ActiveUIDocument.Document),
                CollectPhaseParameters.GetSP_PhaseDemolished3(commandData.Application.ActiveUIDocument.Document),
                CollectPhaseParameters.GetSharedParameterByName(commandData.Application.ActiveUIDocument.Document, "SP_PhaseDemolished[L]"),
                CollectPhaseParameters.GetSharedParameterByName(commandData.Application.ActiveUIDocument.Document, "SP_ViewPhase")
            };
            Utils.Show(CollectPhaseParameters.DebugCheckBindingsForParameters(parameters));
            return Result.Succeeded;
        }
    }
}
