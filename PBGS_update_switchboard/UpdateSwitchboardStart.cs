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
            FilteredElementCollector newViewFilter = new FilteredElementCollector(doc);
            

            /*получение всех листов ревита в качестве элементов,
            далее через цикл создается list листов.
            Потом проиходит формирование массива и сортировка с помощью массива
            */
            ICollection<Element> allsheets = newSheetsFilter.OfCategory(BuiltInCategory.OST_Sheets).WhereElementIsNotElementType().ToElements();
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

            IList<Element> allviewdrawting  = newViewFilter.OfClass(typeof(ViewDrafting)).ToElements();
            List<View> аllViewDrawtingList = new List<View>();
            foreach (Element viewEl in allviewdrawting)
            {
                View view= viewEl as View;
                аllViewDrawtingList.Add(view);
            }
            View[] аllViewDrawtingArray = аllViewDrawtingList.ToArray();

            UserControl userControl = new UserControl(allSheetsArray, аllViewDrawtingArray);
            userControl.ShowDialog();

            //Получение названий листов и видов из userControl
            List<string> allCheckedSheet = new List<string>();
            allCheckedSheet = userControl.allCheckedSheet;
            List<string> allCheckedView = new List<string>();
            allCheckedView = userControl.allCheckedView;

            //Открытие выбранного вида
            UIDocument activeView = commandData.Application.ActiveUIDocument;

            
            View mainView = null;

            foreach (Element viewEl in allviewdrawting)
            {
                View view = viewEl as View;
                if (view.Name == allCheckedView[0].ToString())
                {
                    
                    //viewAnot = doc.GetElement(view.GetDependentElements(viewAnotFilter));
                    mainView = view;
                    activeView.ActiveView = mainView;
                }
                
            }

            List<AnnotationSymbol> viewAnot = new List<AnnotationSymbol>();
            
           // FilteredElementCollector newViewsFilter = new FilteredElementCollector(doc).OfClass(typeof(AnnotationSymbol));

            //ElementFilter viewAnotFilter = new ElementClassFilter(AnnotationSymbol);
            //ICollection<Element> allviewdrdsfawting = newViewsFilter.OfClass(typeof(AnnotationSymbol));

            foreach (ElementId elid in mainView.GetDependentElements(null).ToList()) 
            {
                Element elsymbol = doc.GetElement(elid);
                if (elsymbol.Name == "Type 1")
                {
                AnnotationSymbol symbol = elsymbol as AnnotationSymbol;
                viewAnot.Add(symbol);
                foreach (Parameter pr in symbol.ParametersMap)
                    {
                        string prname = pr.Definition.;
                        double pr1 = pr.AsDouble();
                        string pr2 = pr.AsString();
                        int pr3 = pr.AsInteger();
                        string pr4 = pr.AsValueString();
                    }
                }
            }
            //viewAnot = doc.GetElement(mainView.GetDependentElements(viewAnotFilter).ToList());



            //Console.WriteLine("dsfjlk");
            //Console.WriteLine(sheets);
            //Console.WriteLine("dsfjlk");

            return Result.Succeeded;
        }
    }
}
