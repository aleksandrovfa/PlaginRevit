using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Architecture;

namespace PBGS_update_switchboard
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class UpdateSwitchboardStart : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            FilteredElementCollector newSheetsFilter = new FilteredElementCollector(doc);
            ICollection<Element> allSheets = newSheetsFilter.OfCategory(BuiltInCategory.OST_Sheets).WhereElementIsNotElementType().ToElements();

            List<string> allSheetsNames = new List<string>();

            foreach (Element sheetEl in allSheets)
            {
                allSheetsNames.Add(sheetEl.Name);

            }
                return Result.Succeeded;
        }
    }
}
