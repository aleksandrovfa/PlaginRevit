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
            FilteredElementCollector newFilter = new FilteredElementCollector(doc);

            /*получение всех листов ревита в качестве элементо,
            далее через цикл создается list листов.
            Потом проиходит формирование массива и сортировка с помощью массива
            */
            ICollection<Element> allsheets = newFilter.OfCategory(BuiltInCategory.OST_Sheets).WhereElementIsNotElementType().ToElements();
            List<ViewSheet> аllSheetsList = new List<ViewSheet>();
            foreach (Element sheetEl in allsheets)
            {
                ViewSheet sheet = sheetEl as ViewSheet;
                аllSheetsList.Add(sheet);
            }
            ViewSheet[] allSheetsArray = аllSheetsList.ToArray();
            Array.Sort(allSheetsArray, new SheetsComparerByNum());

            /*получение всех видов и что то бла бла 
            */
            //views = [];
            //FilteredElementCollector collector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views);
            //views = collector.ToElements() as List<Autodesk.Revit.DB.View>;

            FilteredElementCollector viewCollector = new FilteredElementCollector(doc);
            viewCollector.OfClass(typeof(ViewDrafting));


            IList<Element> allviews = newFilter.OfCategory(BuiltInCategory.OST_Views).ToElements(); ;




            UserControl userControl = new UserControl(allSheetsArray);

            userControl.ShowDialog();
            //Console.WriteLine("dsfjlk");
            //Console.WriteLine(sheets);
            //Console.WriteLine("dsfjlk");

            return Result.Succeeded;
        }
    }
}
