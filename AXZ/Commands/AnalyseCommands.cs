using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace AXZ.Commands
{
    public class ParameterFormatCheckCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            throw new NotImplementedException();
        }
    }

    public class Checks
    {
        public enum FormattingIssue
        {
            Empty,
            MissingValues,
            Incorrect,
            Correct
        }
        public static void ParameterFormatCheck(Document doc)
        {
            List<Element> allElementsWithPhaseParameters = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .ToElements()
                .Where(e => e.LookupParameter("SP_PhaseCreated 1") != null)
                .ToList();

            Dictionary<ElementId, Dictionary<string, FormattingIssue>> report = new();

            foreach(Element e in allElementsWithPhaseParameters)
            {
                Dictionary<string, FormattingIssue> elementReport = new();
                Parameter phaseCreated1Param = e.LookupParameter("SP_PhaseCreated 1");
                Parameter phaseCreated2Param = e.LookupParameter("SP_PhaseCreated 2");
                Parameter phaseCreated3Param = e.LookupParameter("SP_PhaseCreated 3");
                Parameter phaseCreatedLParam = e.LookupParameter("SP_PhaseCreated[L]");
                Parameter phaseDemo1Param = e.LookupParameter("SP_PhaseDemolished 1");
                Parameter phaseDemo2Param = e.LookupParameter("SP_PhaseDemolished 2");
                Parameter phaseDemo3Param = e.LookupParameter("SP_PhaseDemolished 3");
                Parameter phaseDemoLParam = e.LookupParameter("SP_PhaseDemolished[L]");
                elementReport["SP_PhaseCreated 1"] = CheckParameterFormat(phaseCreated1Param);
                elementReport["SP_PhaseCreated 2"] = CheckParameterFormat(phaseCreated2Param);
                elementReport["SP_PhaseCreated 3"] = CheckParameterFormat(phaseCreated3Param);
                elementReport["SP_PhaseCreated[L]"] = CheckParameterFormat(phaseCreatedLParam);
                elementReport["SP_PhaseDemolished 1"] = CheckParameterFormat(phaseDemo1Param);
                elementReport["SP_PhaseDemolished 2"] = CheckParameterFormat(phaseDemo2Param);
                elementReport["SP_PhaseDemolished 3"] = CheckParameterFormat(phaseDemo3Param);
                elementReport["SP_PhaseDemolished[L]"] = CheckParameterFormat(phaseDemoLParam);
            }
        }

        private static FormattingIssue CheckParameterFormat(Parameter param)
        {
            if (!param.HasValue)
            {
                return FormattingIssue.Empty;
            }
            string value = param.AsString();
            if (value.Length != 3)
            {
                return FormattingIssue.Empty;
            }
            else if(value.Length == 3 && (value.All(char.IsDigit) || value == "XXX"))
            {
                return FormattingIssue.Correct;
            }
            else
            {   
                return FormattingIssue.Incorrect;
            }
        }   
    }
}
