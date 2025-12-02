using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using Windows.ApplicationModel.Email.DataProvider;
using DB = Autodesk.Revit.DB; 

namespace AXZ.Models
{
    public class CollectPhaseParameters
    {
        public static string CreateSharedParameterFilePath()
        {
            string content = "# This is a Revit shared parameter file.\r\n# Do not edit manually.\r\n*META\tVERSION\tMINVERSION\r\nMETA\t2\t1\r\n*GROUP\tID\tNAME\r\nGROUP\t7\tAXZ_Algemeen\r\n*PARAM\tGUID\tNAME\tDATATYPE\tDATACATEGORY\tGROUP\tVISIBLE\tDESCRIPTION\tUSERMODIFIABLE\tHIDEWHENNOVALUE\r\nPARAM\tb1994837-884e-42de-9b1b-e69dd1258884\tSP_PhaseCreated 2\tTEXT\t\t7\t1\t\t1\t0\r\nPARAM\t8b3cec3e-135d-46cd-b1fe-05c44975f6e9\tSP_PhaseCreated 1\tTEXT\t\t7\t1\t\t1\t0\r\nPARAM\t1012e454-f3e9-4d12-9588-6916092e755c\tSP_PhaseCreated 3\tTEXT\t\t7\t1\t\t1\t0\r\nPARAM\t1b755d79-f0ca-4bea-90f1-1a7c540256a5\tSP_PhaseDemolished[L]\tTEXT\t\t7\t1\t\t0\t0\r\nPARAM\t1a2d8423-8c59-49b7-800a-5eee978176ed\tSP_PhaseDemolished 1\tTEXT\t\t7\t1\t\t1\t0\r\nPARAM\t31bf9f61-b289-4789-b3a5-f1131121a3c1\tSP_PhaseDemolished 2\tTEXT\t\t7\t1\t\t1\t0\r\nPARAM\t6ad742dd-4f2e-4730-91d9-01bb77ca5e88\tSP_PhaseDemolished 3\tTEXT\t\t7\t1\t\t1\t0\r\nPARAM\t2c40208b-95bf-44b3-bd2a-8023b3d96107\tSP_PhaseCreated[L]\tTEXT\t\t7\t1\t\t0\t0\r\nPARAM\t6fa1ee9c-b3e1-4d7e-b333-e4b06563b0df\tSP_ViewPhase\tTEXT\t\t7\t1\t\t1\t0";
            string filepath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "AXZSharedParameters.txt");
            System.IO.File.WriteAllText(filepath, content);
            return filepath;
        }
        public static SharedParameterElement GetSP_PhaseCreated1(Document doc)
        {
            return GetSharedParameterByName(doc, "SP_PhaseCreated 1");
        }
        public static SharedParameterElement GetSP_PhaseCreated2(Document doc)
        {
            return GetSharedParameterByName(doc, "SP_PhaseCreated 2");
        }
        public static SharedParameterElement GetSP_PhaseCreated3(Document doc)
        {
            return GetSharedParameterByName(doc, "SP_PhaseCreated 3");
        }
        public static SharedParameterElement GetSP_PhaseDemolished1(Document doc)
        {
            return GetSharedParameterByName(doc, "SP_PhaseDemolished 1");
        }
        public static SharedParameterElement GetSP_PhaseDemolished2(Document doc)
        {
            return GetSharedParameterByName(doc, "SP_PhaseDemolished 2");
        }
        public static SharedParameterElement GetSP_PhaseDemolished3(Document doc)
        {
            return GetSharedParameterByName(doc, "SP_PhaseDemolished 3");
        }
        public static List<Category> GetFilterableCategoriesForParameter(ParameterElement parameter)
        {
            List<Category> categories = new List<Category>();
            Document document = parameter.Document;
            BindingMap bindings = document.ParameterBindings;
            Autodesk.Revit.DB.Binding binding = bindings.get_Item(parameter.GetDefinition());
            if (binding != null)
            {
                if (binding is InstanceBinding)
                {
                    InstanceBinding instanceBinding = binding as InstanceBinding;
                    foreach(Category cat in instanceBinding.Categories)
                    {
                        categories.Add(cat);
                    }
                }
                else if (binding is TypeBinding)
                {
                    TypeBinding typeBinding = binding as TypeBinding;
                    foreach (Category cat in typeBinding.Categories)
                    {
                        categories.Add(cat);
                    }
                }
            }
            
            return categories;


        }
        public static List<Category> GetPhaseParameterCategories(Document document)
        {
            List<string> categoryNames = new List<string>
            {
                "Abutments",
                "Abutments - Abutment Foundations",
                "Abutments - Abutment Piles",
                "Abutments - Abutment Walls",
                "Abutments - Approach Slabs",
                "Air Terminals",
                "Alignments",
                "Assemblies",
                "Audio Visual Devices",
                "Bearings",
                "Bridge Cables",
                "Bridge Decks",
                "Bridge Framing",
                "Bridge Framing - Arches",
                "Bridge Framing - Cross Bracing",
                "Bridge Framing - Diaphragms",
                "Bridge Framing - Girders",
                "Bridge Framing - Trusses",
                "Cable Tray Fittings",
                "Cable Trays",
                "Casework",
                "Ceilings",
                "Columns",
                "Communication Devices",
                "Conduit Fittings",
                "Conduits",
                "Curtain Panels",
                "Curtain Systems",
                "Curtain Wall Mullions",
                "Data Devices",
                "Doors",
                "Duct Accessories",
                "Duct Insulations",
                "Duct Linings",
                "Duct Placeholders",
                "Electrical Equipment",
                "Electrical Fixtures",
                "Entourage",
                "Expansion Joints",
                "Fire Alarm Devices",
                "Fire Protection",
                "Flex Ducts",
                "Flex Pipes",
                "Floors",
                "Floors - Slab Edges",
                "Food Service Equipment",
                "Furniture",
                "Furniture Systems",
                "Generic Models",
                "Hardscape",
                "Lighting Devices",
                "Lighting Fixtures",
                "Mass",
                "Mass - Mass Exterior Wall",
                "Mass - Mass Floor",
                "Mass - Mass Glazing",
                "Mass - Mass Interior Wall",
                "Mass - Mass Opening",
                "Mass - Mass Roof",
                "Mass - Mass Skylight",
                "Mass - Mass Zone",
                "Mechanical Control Devices",
                "Mechanical Equipment",
                "Medical Equipment",
                "MEP Ancillary Framing",
                "MEP Fabrication Containment",
                "MEP Fabrication Ductwork",
                "MEP Fabrication Ductwork - Insulation",
                "MEP Fabrication Ductwork - Lining",
                "MEP Fabrication Ductwork Stiffeners",
                "MEP Fabrication Hangers",
                "MEP Fabrication Pipework",
                "MEP Fabrication Pipework - Insulation",
                "Model Groups",
                "Nurse Call Devices",
                "Parking",
                "Parts",
                "Piers",
                "Piers - Pier Caps",
                "Piers - Pier Columns",
                "Piers - Pier Foundations",
                "Piers - Pier Piles",
                "Piers - Pier Towers",
                "Piers - Pier Walls",
                "Pipe Accessories",
                "Pipe Fittings",
                "Pipe Insulations",
                "Pipe Placeholders",
                "Pipes",
                "Planting",
                "Plumbing Equipment",
                "Plumbing Fixtures",
                "Railings",
                "Railings - Handrails",
                "Railings - Top Rails",
                "Ramps",
                "Roads",
                "Roofs",
                "Roofs - Fascias",
                "Roofs - Gutters",
                "Roofs - Roof Soffits",
                "Shaft Openings",
                "Signage",
                "Site",
                "Site - Pads",
                "Specialty Equipment",
                "Sprinklers",
                "Stairs",
                "Structural Area Reinforcement",
                "Structural Beam Systems",
                "Structural Columns",
                "Structural Connections",
                "Structural Connections - Anchors",
                "Structural Connections - Bolts",
                "Structural Connections - Plates",
                "Structural Connections - Profiles",
                "Structural Connections - Shear Studs",
                "Structural Fabric Areas",
                "Structural Fabric Reinforcement",
                "Structural Foundations",
                "Structural Framing",
                "Structural Internal Loads - Internal Area Loads",
                "Structural Internal Loads - Internal Line Loads",
                "Structural Internal Loads - Internal Point Loads",
                "Structural Loads - Area Loads",
                "Structural Loads - Line Loads",
                "Structural Loads - Point Loads",
                "Structural Path Reinforcement",
                "Structural Rebar",
                "Structural Rebar Couplers",
                "Structural Stiffeners",
                "Structural Tendons",
                "Structural Trusses",
                "Telephone Devices",
                "Temporary Structures",
                "Topography",
                "Toposolid",
                "Vertical Circulation",
                "Vibration Management",
                "Vibration Management - Vibration Dampers",
                "Vibration Management - Vibration Isolators",
                "Walls",
                "Walls - Wall Sweeps",
                "Windows",
                "Wires"
            };
            List<Category> categories = new List<Category>();
            foreach (Category cat in document.Settings.Categories)
            {
                if (categoryNames.Contains(cat.Name))
                {
                    categories.Add(cat);
                }
            }
            return categories;
        }
        public static bool HasParameterBinding(ParameterElement parameter)
        {
            Document document = parameter.Document;
            BindingMap bindings = document.ParameterBindings;
            Autodesk.Revit.DB.Binding binding = bindings.get_Item(parameter.GetDefinition());

            if (binding != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void RepairPhasingParameterBindings(Document doc)
        {

            string sharedParamFilePath = CollectPhaseParameters.CreateSharedParameterFilePath();
            List<ParameterElement> parameters = new List<ParameterElement>()
            {
                CollectPhaseParameters.GetSP_PhaseCreated1(doc),
                CollectPhaseParameters.GetSP_PhaseCreated2(doc),
                CollectPhaseParameters.GetSP_PhaseCreated3(doc),
                CollectPhaseParameters.GetSharedParameterByName(doc, "SP_PhaseCreated[L]"),
                CollectPhaseParameters.GetSP_PhaseDemolished1(doc),
                CollectPhaseParameters.GetSP_PhaseDemolished2(doc),
                CollectPhaseParameters.GetSP_PhaseDemolished3(doc),
                CollectPhaseParameters.GetSharedParameterByName(doc, "SP_PhaseDemolished[L]"),
                CollectPhaseParameters.GetSharedParameterByName(doc, "SP_ViewPhase")
            };

            BindingMap map = doc.ParameterBindings;
            using (Transaction transaction = new Transaction(doc, "Repair Existing Phase Parameters"))
            {
                transaction.Start();
                foreach (ParameterElement param in parameters)
                {
                    //Get the Filterable Categories for the Parameter   
                    List<Category> filterableCategories = GetFilterableCategoriesForParameter(param);

                    //Get the Categories that should be assigned to the Parameter
                    List<Category> categories = CollectPhaseParameters.GetPhaseParameterCategories(doc);
                    List<BuiltInCategory> builtInCategories = CollectPhaseParameters.ParseToBuiltInCategory(categories);
                    
                    bool hasParameterBinding = CollectPhaseParameters.HasParameterBinding(param);
                    bool categoriesMatch = filterableCategories.Count == categories.Count;
                    if (param.Name == "SP_ViewPhase")
                    {
                        //For the SP_ViewPhase parameter, we only want to assign the View category
                        categoriesMatch = filterableCategories.Count == 1;
                    }

                    #region REPORT THE CHECKS
                    if (!hasParameterBinding)
                    {
                        Debug.Log($"Parameter '{param.Name}' does not have a binding.");
                    }
                    else 
                    {                         
                        Debug.Log($"Parameter '{param.Name}' has a binding.");
                    }


                    if (!categoriesMatch)
                    {
                        Debug.Log($"Parameter '{param.Name}' has {filterableCategories.Count} categories assigned, but should have {categories.Count}.");

                        Debug.Log("--------------- Wrong Categories ---------------");
                        foreach (string filterableCategoryName in filterableCategories.Select(c => c.Name))
                        {
                            if(!categories.Select(cat => cat.Name).Contains(filterableCategoryName))
                            {
                                //Category is incorrect
                                Debug.Log($" - Category '{filterableCategoryName}' is INCORRECT and should be removed.");
                            }
                        }
                        Debug.Log("--------------- Missing Categories ---------------");
                        foreach (string category in categories.Select(c => c.Name))
                        {
                            if (!filterableCategories.Select(cat => cat.Name).Contains(category))
                            {
                                Debug.Log($" - Category '{category}' is missing.");
                            }
                        }
                    }
                    else
                    {
                        Debug.Log($"Parameter '{param.Name}' has the correct number of categories assigned ({filterableCategories.Count})."); 
                    }
                    #endregion

                    //If a Parameter does not have a binding or if the assigned categories are not matching, We find the Binding and re-assign it
                    if (!CollectPhaseParameters.HasParameterBinding(param))
                    {
                        DefinitionBindingMapIterator it = map.ForwardIterator();
                        it.Reset();
                        while (it.MoveNext())
                        {
                            Definition def = it.Key;
                            if (def != null)
                            {
                                if (def.Name == param.Name)
                                {
                                    Debug.Log("Re-entering binding for parameter: " + param.Name);
                                    try
                                    {
                                        DB.Binding binding = (DB.Binding)it.Current;
                                        map.Remove(def);
                                        map.Insert(param.GetDefinition(), binding, GroupTypeId.Phasing);
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.Log("Error while re-assigning binding for parameter: " + param.Name + ". Exception: " + ex.Message);
                                    }
                                }
                            }
                        }

                    }
                    
                    if (!categoriesMatch)
                    {
                        DB.Binding newBinding = new InstanceBinding(CreateCategorySet(doc, builtInCategories));
                        try
                        {
                            Debug.Log("Re-assigning a New binding for parameter: " + param.Name);
                            map.ReInsert(param.GetDefinition(), newBinding, GroupTypeId.Phasing);
                        }
                        catch (Exception ex)
                        {
                            Debug.Log("Error while re-assigning binding for parameter: " + param.Name + ". Exception: " + ex.Message);
                        }
                    }
                }
                transaction.Commit();
            }
        }
        public static bool DoesParameterExists(Document doc, string paramName, ExternalDefinition definition)
        {
            string guid = definition.GUID.ToString();
            bool exists = false;
            List<SharedParameterElement> possibleParameter = new FilteredElementCollector(doc)
                .OfClass(typeof(SharedParameterElement))
                .Cast<SharedParameterElement>()
                .Where(p => p.Name == paramName)
                .ToList();
            foreach (var param in possibleParameter)
            {
                if (param.GuidValue.ToString() == guid)
                {
                    exists = true;
                    break;
                }
            }
            return false;
        }
        public static Definition FindDefinition(DefinitionFile file, string paramName)
        {
            return file.Groups
                       .Cast<DefinitionGroup>()
                       .Select(g => g.Definitions.get_Item(paramName))
                       .FirstOrDefault(def => def != null);
        }
        public static CategorySet CreateCategorySet(Document doc, IEnumerable<BuiltInCategory> categories)
        {
            CategorySet set = new();
            foreach (var bic in categories)
            {
                Category cat = doc.Settings.Categories.get_Item(bic);
                if (cat != null) set.Insert(cat);
            }
            return set;
        }
        public static string DebugCheckBindingsForParameters(List<ParameterElement> parameters)
        {
            string result = "";
            List<Category> categories = new List<Category>();
            Document document = parameters[0].Document;
            BindingMap bindings = document.ParameterBindings;

            foreach (ParameterElement parameter in parameters)
            {
                Autodesk.Revit.DB.Binding binding = bindings.get_Item(parameter.GetDefinition()); 
                if (binding != null)
                {
                    string bindingType = binding.GetType().Name;
                    result += $"Parameter {parameter.Name} has a {bindingType} binding.\n";
                }
                else
                {
                    result+= $"Parameter {parameter.Name} has no binding.\n";
                }
            }
            

            return result;


        }
        public static SharedParameterElement GetSharedParameterByName(Document doc, string paramName)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(SharedParameterElement));
            foreach (SharedParameterElement spe in collector)
            {
                if (spe.Name == paramName)
                {
                    return spe;
                }
            }
            return null;
        }
        public static List<BuiltInCategory> ParseToBuiltInCategory(List<Category> categories)
        {
            List<BuiltInCategory> builtInCategories = new List<BuiltInCategory>();
            foreach (Category cat in categories)
            {
                builtInCategories.Add(cat.BuiltInCategory);
            }
            return builtInCategories;
        }
    }
}
