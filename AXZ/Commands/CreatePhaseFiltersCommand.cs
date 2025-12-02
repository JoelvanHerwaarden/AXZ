using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using AXZ.Models;
using AXZ.UI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using DB = Autodesk.Revit.DB;
using AS = Autodesk.Revit.ApplicationServices;

namespace AXZ.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CreatePhaseFiltersCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uIDocument = commandData.Application.ActiveUIDocument;
            Document document = commandData.Application.ActiveUIDocument.Document;
            Document doc = commandData.Application.ActiveUIDocument.Document;

            CollectPhaseParameters.RepairPhasingParameterBindings(document);
            Selection selection = uIDocument.Selection;
            Parameter viewPhaseParameter = document.ActiveView.LookupParameter("SP_ViewPhase");
            if(viewPhaseParameter == null)
            {
                Utils.Show("The active view does not have the 'SP_ViewPhase' parameter.");
                return Result.Failed;
            }
            string code = viewPhaseParameter.AsValueString();
            if (code == string.Empty | code == null)
            {
                Utils.Show("The 'SP_ViewPhase' parameter in the active view is empty. Please set it before creating filters.\n\nMake sure to seperate the phases using dashes: For Example 100-100-200");
                return Result.Failed;
            }
            string phase = viewPhaseParameter.AsString();

            ViewPhaseWindow window = new ViewPhaseWindow("Create Phase Filters", document, Utils.RevitWindow(commandData));
            window.ShowDialog();
            if(window.Cancelled)
            {
                return Result.Cancelled;
            }

            //In the End Change this
            string selectPhaseLevelParameter = "";
            string demolishPhaseLevelParameter = "";
            string filterNamePrefix = "";

            switch (window.SelectedPhaseLevel)
            {
                case "Phase 1":
                    selectPhaseLevelParameter = "SP_PhaseCreated 1";
                    demolishPhaseLevelParameter = "SP_PhaseDemolished 1";
                    filterNamePrefix = "SP_ViewPhase 1";
                    phase = phase.Split("-")[0];
                    break;
                case "Phase 2":
                    selectPhaseLevelParameter = "SP_PhaseCreated 2";
                    demolishPhaseLevelParameter = "SP_PhaseDemolished 2";
                    filterNamePrefix = "SP_ViewPhase 2";
                    phase = phase.Split("-")[1];
                    break;
                case "Phase 3":
                    selectPhaseLevelParameter = "SP_PhaseCreated 3";
                    demolishPhaseLevelParameter = "SP_PhaseDemolished 3";
                    filterNamePrefix = "SP_ViewPhase 3";
                    phase = phase.Split("-")[2];
                    break;
                case "Phase Combined":
                    selectPhaseLevelParameter = "SP_PhaseCreated[L]";
                    demolishPhaseLevelParameter = "SP_PhaseDemolished[L]";
                    filterNamePrefix = "SP_ViewPhaseCombined";
                    break;
            }

            List<string> filterNames = new List<string>();

            using (Transaction transaction = new Transaction(document, "Create View Filter by Parameter"))
            {
                transaction.Start();

                string newFilterName = string.Format("{0}_{1}_New", filterNamePrefix, phase);
                string demoFilterName = string.Format("{0}_{1}_Demolished", filterNamePrefix, phase);
                string tempFilterName = string.Format("{0}_{1}_Temporary", filterNamePrefix, phase);
                string tempInPhaseFilterName = string.Format("{0}_{1}_Temporary In Current Phase", filterNamePrefix, phase);  
                string notVisFilterName = string.Format("{0}_{1}_NotVisible", filterNamePrefix, phase);

                Debug.Log($"Creating filters: {newFilterName}, {demoFilterName}, {tempFilterName}, {notVisFilterName}");
                SharedParameterElement startPhase = CollectPhaseParameters.GetSharedParameterByName(document, selectPhaseLevelParameter);
                Debug.Log($"Retrieved start phase parameter: {startPhase.Name}");
                SharedParameterElement demoPhase = CollectPhaseParameters.GetSharedParameterByName(document, demolishPhaseLevelParameter);
                Debug.Log($"Retrieved demolish phase parameter: {demoPhase.Name}"); 
                ParameterFilterElement filterElementNew = FilterUtils.CreateNewFilter(startPhase, phase, newFilterName);
                Debug.Log("New filter created successfully.");
                ParameterFilterElement filterElementDemo = FilterUtils.CreateDemolishedFilter(demoPhase, phase, demoFilterName);
                Debug.Log("Demolished filter created successfully.");
                ParameterFilterElement filterElementTempInPhase = FilterUtils.CreateTempInPhaseFilter(startPhase, demoPhase, phase, tempInPhaseFilterName);
                Debug.Log("Temporary In Phase filter created successfully.");
                ParameterFilterElement filterElementTemp = FilterUtils.CreateTempFilter(startPhase, demoPhase, phase, tempFilterName);
                Debug.Log("Temporary filter created successfully.");
                ParameterFilterElement filterElementNotVis = FilterUtils.CreateNotVisibleFilter(startPhase, demoPhase, phase, notVisFilterName);
                Debug.Log("Not Visible filter created successfully.");
                Debug.Log("Filters created successfully.");
                filterNames.Add(filterElementNew.Name);
                filterNames.Add(filterElementDemo.Name);
                filterNames.Add(filterElementTemp.Name);
                filterNames.Add(filterElementNotVis.Name);

                if(window.AddToView)
                {
                    Autodesk.Revit.DB.View activeView = document.ActiveView;
                    if (activeView.ViewTemplateId != ElementId.InvalidElementId)
                    {
                        activeView = (Autodesk.Revit.DB.View)document.GetElement(activeView.ViewTemplateId);
                    }

                    //Remove Old Filters
                    foreach(ElementId id in FilterUtils.GetAlreadyAppliedFilters(activeView))
                    {
                        activeView.RemoveFilter(id);
                    }

                    ElementId solidFillPattern = new FilteredElementCollector(document)
                        .OfClass(typeof(FillPatternElement))
                        .Where(FillPatternElement => FillPatternElement.Name.Equals("<Solid fill>"))
                        .Cast<FillPatternElement>()
                        .First()
                        .Id;

                    DB.Color New = new DB.Color(0, 255, 0);
                    DB.Color Demo = new DB.Color(255, 0, 0);
                    DB.Color TempInPhase = new DB.Color(0, 0, 255);
                    DB.Color TempOverall = new DB.Color(0, 255, 255);


                    OverrideGraphicSettings graphicsTempInPhase = new OverrideGraphicSettings();
                    graphicsTempInPhase.SetCutForegroundPatternColor(TempInPhase);
                    graphicsTempInPhase.SetSurfaceForegroundPatternColor(TempInPhase);
                    graphicsTempInPhase.SetCutForegroundPatternId(solidFillPattern);
                    graphicsTempInPhase.SetSurfaceForegroundPatternId(solidFillPattern);
                    activeView.AddFilter(filterElementTempInPhase.Id);
                    activeView.SetFilterOverrides(filterElementTempInPhase.Id, graphicsTempInPhase);
                    
                    OverrideGraphicSettings graphicsNew = new OverrideGraphicSettings();
                    graphicsNew.SetCutForegroundPatternColor(New);
                    graphicsNew.SetSurfaceForegroundPatternColor(New);
                    graphicsNew.SetCutForegroundPatternId(solidFillPattern);
                    graphicsNew.SetSurfaceForegroundPatternId(solidFillPattern);
                    activeView.AddFilter(filterElementNew.Id);
                    activeView.SetFilterOverrides(filterElementNew.Id, graphicsNew);

                    OverrideGraphicSettings graphicsDemo = new OverrideGraphicSettings();
                    graphicsDemo.SetCutForegroundPatternColor(Demo);
                    graphicsDemo.SetSurfaceForegroundPatternColor(Demo);
                    graphicsDemo.SetCutForegroundPatternId(solidFillPattern);
                    graphicsDemo.SetSurfaceForegroundPatternId(solidFillPattern);
                    activeView.AddFilter(filterElementDemo.Id);
                    activeView.SetFilterOverrides(filterElementDemo.Id, graphicsDemo);



                    OverrideGraphicSettings graphicsTemp = new OverrideGraphicSettings();
                    graphicsTemp.SetCutForegroundPatternColor(TempOverall);
                    graphicsTemp.SetSurfaceForegroundPatternColor(TempOverall);
                    graphicsTemp.SetCutForegroundPatternId(solidFillPattern);
                    graphicsTemp.SetSurfaceForegroundPatternId(solidFillPattern);
                    activeView.AddFilter(filterElementTemp.Id);
                    activeView.SetFilterOverrides(filterElementTemp.Id, graphicsTemp);

                    activeView.AddFilter(filterElementNotVis.Id);
                    activeView.SetFilterVisibility(filterElementNotVis.Id, false);
                }

                transaction.Commit();
            }
            Utils.ShowInfoBalloon("Created filters:\n" + string.Join("\n", filterNames));

            return Result.Succeeded;
        }
    }
    public class FilterUtils
    {
        public static ParameterFilterElement CreateNewFilter(SharedParameterElement startPhase, string filterValue, string filterName)
        {
            Document document = startPhase.Document;
            ParameterFilterElement viewFilter = new FilteredElementCollector(document)
                .OfClass(typeof(ParameterFilterElement))
                .Cast<ParameterFilterElement>()
                .FirstOrDefault(f => f.Name == filterName);
            if (viewFilter != null)
            {
                return viewFilter;
            }
            List<Category> cats = CollectPhaseParameters.GetPhaseParameterCategories(document);
            //List<Category> cats = CollectPhaseParameters.GetFilterableCategoriesForParameter(startPhase);

            List<ElementId> allowedCategoryIds = cats.Select(cat => cat.Id).ToList();
            FilterRule hasValueRule = ParameterFilterRuleFactory.CreateNotEqualsRule(startPhase.Id, "");
            FilterRule secondRule = ParameterFilterRuleFactory.CreateEqualsRule(startPhase.Id, filterValue);

            LogicalAndFilter filter = new LogicalAndFilter(new List<ElementFilter>
            {
                new ElementParameterFilter(hasValueRule),
                new ElementParameterFilter(secondRule)
            } as IList<ElementFilter>);

            viewFilter = ParameterFilterElement.Create(
                document,
                filterName,
                allowedCategoryIds,
                filter
            );
            return viewFilter;
        }
        public static ParameterFilterElement CreateDemolishedFilter(SharedParameterElement demoPhase, string filterValue, string filterName)
        {
            Document document = demoPhase.Document;
            ParameterFilterElement viewFilter = new FilteredElementCollector(document)
                .OfClass(typeof(ParameterFilterElement))
                .Cast<ParameterFilterElement>()
                .FirstOrDefault(f => f.Name == filterName);
            if (viewFilter != null)
            {
                return viewFilter;
            }

            List<Category> cats = CollectPhaseParameters.GetPhaseParameterCategories(document);
            //List<Category> cats = CollectPhaseParameters.GetFilterableCategoriesForParameter(demoPhase);

            List<ElementId> allowedCategoryIds = cats.Select(cat => cat.Id).ToList();
            FilterRule hasValueRule = ParameterFilterRuleFactory.CreateNotEqualsRule(demoPhase.Id, "");
            FilterRule secondRule = ParameterFilterRuleFactory.CreateEqualsRule(demoPhase.Id, filterValue);

            LogicalAndFilter filter = new LogicalAndFilter(new List<ElementFilter>
            {
                new ElementParameterFilter(hasValueRule),
                new ElementParameterFilter(secondRule)
            } as IList<ElementFilter>);
            viewFilter = ParameterFilterElement.Create(
                document,
                filterName,
                allowedCategoryIds,
                filter
            );
            return viewFilter;
        }
        public static ParameterFilterElement CreateTempInPhaseFilter(SharedParameterElement startPhase, SharedParameterElement demoPhase, string filterValue, string filterName)
        {
            Document document = startPhase.Document;
            ParameterFilterElement viewFilter = new FilteredElementCollector(document)
                .OfClass(typeof(ParameterFilterElement))
                .Cast<ParameterFilterElement>()
                .FirstOrDefault(f => f.Name == filterName);
            if (viewFilter != null)
            {
                return viewFilter;
            }
            List<Category> cats = CollectPhaseParameters.GetPhaseParameterCategories(document);
            //List<Category> cats = CollectPhaseParameters.GetFilterableCategoriesForParameter(startPhase);

            List<ElementId> allowedCategoryIds = cats.Select(cat => cat.Id).ToList();

            FilterRule firstRule = ParameterFilterRuleFactory.CreateEqualsRule(startPhase.Id, filterValue);
            ElementParameterFilter firstFilter = new ElementParameterFilter(firstRule);
            FilterRule secondRule = ParameterFilterRuleFactory.CreateEqualsRule(demoPhase.Id, filterValue);
            ElementParameterFilter secondFilter = new ElementParameterFilter(secondRule);

            LogicalAndFilter filter = new LogicalAndFilter(new List<ElementFilter> { firstFilter, secondFilter } as IList<ElementFilter>);

            viewFilter = ParameterFilterElement.Create(
                document,
                filterName,
                allowedCategoryIds,
                filter
            );
            return viewFilter;
        }
        public static ParameterFilterElement CreateTempFilter(SharedParameterElement startPhase, SharedParameterElement demoPhase, string filterValue, string filterName)
        {
            Document document = startPhase.Document;
            ParameterFilterElement viewFilter = new FilteredElementCollector(document)
                .OfClass(typeof(ParameterFilterElement))
                .Cast<ParameterFilterElement>()
                .FirstOrDefault(f => f.Name == filterName);
            if (viewFilter != null)
            {                return viewFilter;
            }
            List<Category> cats = CollectPhaseParameters.GetPhaseParameterCategories(document);
            //List<Category> startCats = CollectPhaseParameters.GetFilterableCategoriesForParameter(startPhase);

            List<ElementId> allowedCategoryIds = cats.Select(cat => cat.Id).ToList();
            FilterRule hasValueRule = ParameterFilterRuleFactory.CreateNotEqualsRule(startPhase.Id, "");
            ElementParameterFilter hasValueFilter = new ElementParameterFilter(hasValueRule);

            FilterRule secondRule = ParameterFilterRuleFactory.CreateLessRule(startPhase.Id, filterValue);
            ElementParameterFilter secondFilter = new ElementParameterFilter(secondRule);

            FilterRule secondHasValueRule = ParameterFilterRuleFactory.CreateNotEqualsRule(demoPhase.Id, "");
            ElementParameterFilter secondHasValueFilter = new ElementParameterFilter(secondHasValueRule);

            FilterRule thirdRule = ParameterFilterRuleFactory.CreateGreaterRule(demoPhase.Id, filterValue);
            ElementParameterFilter thirdFilter = new ElementParameterFilter(thirdRule);

            LogicalAndFilter filter = new LogicalAndFilter(new List<ElementFilter> { hasValueFilter, secondFilter, secondHasValueFilter, thirdFilter } as IList<ElementFilter>);

            viewFilter = ParameterFilterElement.Create(
                document,
                filterName,
                allowedCategoryIds,
                filter
            );
            return viewFilter;
        }
        public static ParameterFilterElement CreateNotVisibleFilter(SharedParameterElement startPhase, SharedParameterElement demoPhase, string filterValue, string filterName)
        {
            Document document = startPhase.Document;
            ParameterFilterElement viewFilter = new FilteredElementCollector(document)
                .OfClass(typeof(ParameterFilterElement))
                .Cast<ParameterFilterElement>()
                .FirstOrDefault(f => f.Name == filterName);
            if (viewFilter != null)
            {
                return viewFilter;
            }
            List<Category> cats = CollectPhaseParameters.GetPhaseParameterCategories(document);
            List<ElementId> allowedCategoryIds = cats.Select(cat => cat.Id).ToList();

            FilterRule hasStartValueRule = ParameterFilterRuleFactory.CreateNotEqualsRule(demoPhase.Id, "");
            ElementParameterFilter hasStartValueFilter = new ElementParameterFilter(hasStartValueRule);

            FilterRule startRule = ParameterFilterRuleFactory.CreateLessRule(demoPhase.Id, filterValue);
            ElementParameterFilter startFilter = new ElementParameterFilter(startRule);
            LogicalAndFilter startPhaseFilter = new LogicalAndFilter(hasStartValueFilter, startFilter);

            FilterRule hasDemoValueRule = ParameterFilterRuleFactory.CreateNotEqualsRule(startPhase.Id, "");
            ElementParameterFilter hasDemoValueFilter = new ElementParameterFilter(hasDemoValueRule);

            FilterRule demoRule = ParameterFilterRuleFactory.CreateGreaterRule(startPhase.Id, filterValue);
            ElementParameterFilter demoFilter = new ElementParameterFilter(demoRule);
            LogicalAndFilter demoPhaseFilter = new LogicalAndFilter(hasDemoValueFilter, demoFilter);

            LogicalOrFilter finalFilter = new LogicalOrFilter(startPhaseFilter, demoPhaseFilter);

            viewFilter = ParameterFilterElement.Create(
                document,
                filterName,
                allowedCategoryIds,
                finalFilter
            );
            return viewFilter;
        }
        public static List<Category> GetFilterableCategories(Document document, List<ElementId> parameters, List<CategoryType> types = null)
        {
            List<Category> filterableCategories = new List<Category>();
            if (types == null)
            {
                types = new List<CategoryType>() { CategoryType.Invalid, CategoryType.Internal, CategoryType.Model, CategoryType.Annotation, CategoryType.AnalyticalModel };
            }
            foreach (Category category in document.Settings.Categories)
            {
                if (types.Contains(category.CategoryType))
                {
                    List<ElementId> categoryIds = new List<ElementId>() { category.Id };
                    if (ParameterFilterUtilities.GetInapplicableParameters(document, categoryIds, parameters).Count == 0)
                    {
                        filterableCategories.Add(category);
                    }
                }
            }
            return filterableCategories;
        }

        public static List<ElementId> GetAlreadyAppliedFilters(Autodesk.Revit.DB.View view)
        {
            List<ElementId> result = new List<ElementId>();
            foreach (ElementId id in view.GetFilters())
            {
                Element filter = view.Document.GetElement(id);
                if (filter.Name.StartsWith("SP_ViewPhase"))
                {
                    result.Add(id);
                }
            }
            return result;
        }
    }
}
