using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Structure;

namespace PBGS_update_switchboard
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class UpdateSwitchboardStart : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            UIApplication uiApp = commandData.Application;
            try
            {
                
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

                IList<Element> allviewdrawting = newViewFilter.OfClass(typeof(ViewDrafting)).ToElements();
                List<View> аllViewDrawtingList = new List<View>();
                foreach (Element viewEl in allviewdrawting)
                {
                    View view = viewEl as View;
                    аllViewDrawtingList.Add(view);
                }
                View[] аllViewDrawtingArray = аllViewDrawtingList.ToArray();

                UserControl userControl = new UserControl(allSheetsArray, аllViewDrawtingArray);
                userControl.ShowDialog();

                //Получение названий листов и видов из userControl
                List<string> allCheckedSheet = new List<string>();
                allCheckedSheet = userControl.ALLCHECKEDSHEET;
                List<string> allCheckedView = new List<string>();
                allCheckedView = userControl.ALLCHEKEDVIEW;

                //Открытие выбранного вида и сохрание
                UIDocument activeView = commandData.Application.ActiveUIDocument;
                View mainView = null;
                // перебор всех доступных чертежных видов и включение отмеченного у которого совпало название с отмеченным.
                foreach (Element viewEl in allviewdrawting)
                {
                    View view = viewEl as View;
                    if (view.Name == allCheckedView[0].ToString())
                    {
                        mainView = view;
                        activeView.ActiveView = mainView;
                    }

                }
                // перебор всех доступных листов и добавление в словарь.
                List<ViewSheet> mainSheets = new List<ViewSheet>();
                foreach (Element sheetEl in allsheets)
                {
                    ViewSheet sheet = sheetEl as ViewSheet;
                    if (allCheckedSheet.Contains(sheet.Title))
                    {
                        mainSheets.Add(sheet);
                    }

                }

                //* Считывание всех аннотаций групп и создание главного и минорного словаря с аннотациями.
                //Минорный словарь это словарь словарей, первый ключ - название листа, второй название группы.

                ElementClassFilter filter = new ElementClassFilter(typeof(FamilyInstance));

                Dictionary<string, AnnotationSymbol> dictAnotMain = new Dictionary<string, AnnotationSymbol>();
                dictAnotMain = GetAnnotationView(mainView, doc);

                Dictionary<string, Dictionary<string, AnnotationSymbol>> dictAnotMinor =
                    new Dictionary<string, Dictionary<string, AnnotationSymbol>>();
                foreach (ViewSheet sheet in mainSheets)
                {
                    dictAnotMinor.Add(sheet.Title, GetAnnotationSheets(sheet, doc));
                }
                //************************************************************************
                
                Selection sel = uiApp.ActiveUIDocument.Selection;

                foreach (var dictAnotminor in dictAnotMinor)
                {
                    List<ElementId> anotNotFoundInView = new List<ElementId>();
                    Dictionary<string, string> showGroup = new Dictionary<string, string>();
                    Dictionary<string, string> changeAllAnot = new Dictionary<string, string>();
                    Transaction trans = new Transaction(doc);
                    trans.Start("1");


                    foreach (var dictAnot in dictAnotminor.Value)
                    {
                        if (!dictAnotMain.Keys.Contains(dictAnot.Key))
                        {
                            showGroup.Add(dictAnot.Key, ": не найдена");
                            anotNotFoundInView.Add(dictAnot.Value.Id);
                        }
                        else
                        {
                            Dictionary<string, string> changeOneAnot = 
                                CompareAnnotations(dictAnotMain[dictAnot.Key], dictAnot.Value);
                            if (changeOneAnot.Count > 0)
                            {
                                showGroup.Add(dictAnot.Key, ": обновлено");
                                changeAllAnot.Add(dictAnot.Key, string.Join(Environment.NewLine, changeOneAnot));
                            }
                            else showGroup.Add(dictAnot.Key, ": всё норм");
                            dictAnotMain.Remove(dictAnot.Key);
                        }
                    }

                    MessageBox.Show(dictAnotminor.Key + "\n" + string.Join(Environment.NewLine, showGroup), "Проверка");
                    if (anotNotFoundInView.Count > 0)
                    {
                        XYZ point = sel.PickPoint("Укажите точку размещения групп щита");
                        Group group = null;
                        ICollection<ElementId> ids1 = anotNotFoundInView;
                        group = doc.Create.NewGroup(ids1);
                        Group group1 = doc.Create.PlaceGroup(point, group.GroupType);
                        group.UngroupMembers();
                        //group1.UngroupMembers();

                    }
                    trans.Commit();

                    if (userControl.CreateComments.IsChecked == true & changeAllAnot.Count > 0)
                    {
                        trans.Start("2");
                        XYZ point = sel.PickPoint("Укажите точку примечаний изменения");
                        ElementId defaultTypeId = doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);
                        //string text = string.Join(Environment.NewLine, changeAllAnot.Keys + "\n" + 
                        //              string.Join(Environment.NewLine, changeAllAnot.Values));
                        TextNote note = TextNote.Create(doc, mainView.Id, point, 
                            string.Join(Environment.NewLine, changeAllAnot), defaultTypeId);
                        trans.Commit();
                    }
                }
                if (userControl.DelAnnotanion.IsChecked == true & dictAnotMain.Count >0)
                {
                    List<string> notFoundAnotName = new List<string>();
                    List<ElementId> notFoundAnotid = new List<ElementId>();
                    foreach (var row in dictAnotMain)
                    {
                        notFoundAnotName.Add(row.Key);
                        notFoundAnotid.Add(row.Value.Id);
                    }
                    var result = MessageBox.Show("Следующие группы не найдены на листах: \n" +
                        String.Join(", ", notFoundAnotName.ToArray()), "Удалить группы?", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        Transaction trans = new Transaction(doc);
                        trans.Start("2");
                        ICollection<ElementId> el = notFoundAnotid;
                        doc.Delete(el);
                        trans.Commit();
                    }
                }
            }
            //Обработка исключения при щелчке правой кнопкой или нажатии ESC
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }
            //Обработка других ошибок
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        return Result.Succeeded;
        }
        //ниже две одинаковых функции, одна для view, вторая для viewsheet
        //на выходе получается словарь с аннтациями, ключами служат названия групп
        private Dictionary<string, AnnotationSymbol> GetAnnotationView(View view, Document doc)
        {
            ElementClassFilter filter = new ElementClassFilter(typeof(FamilyInstance));
            Dictionary<string, AnnotationSymbol> dictAnot = new Dictionary<string, AnnotationSymbol>();
            foreach (ElementId elid in view.GetDependentElements(filter).ToList())
            {
                Element elsymbol = doc.GetElement(elid);
                if (elsymbol.Category.Name == "Типовые аннотации")
                {
                    AnnotationSymbol symbol = elsymbol as AnnotationSymbol;
                    if (symbol.AnnotationSymbolType.FamilyName == "# Reports - Схема - Силовой щит - Данные - ГРЩ" |
                                symbol.AnnotationSymbolType.FamilyName == "# Reports - Схема - Силовой щит - Данные" |
                                symbol.AnnotationSymbolType.FamilyName == "# Reports - Схема - Силовой щит - Данные - ГРЩ1")
                    {
                        foreach (Parameter pr in symbol.ParametersMap)
                        {
                            if (pr.Definition.Name == "Обозначение")
                            {
                                dictAnot.Add(pr.AsString(), symbol);
                            }
                        }

                    }
                }
            }
            return dictAnot;

        }

        private Dictionary<string, AnnotationSymbol> GetAnnotationSheets(ViewSheet view, Document doc)
        {
            ElementClassFilter filter = new ElementClassFilter(typeof(FamilyInstance));
            Dictionary<string, AnnotationSymbol> dictAnot = new Dictionary<string, AnnotationSymbol>();
            foreach (ElementId elid in view.GetDependentElements(filter).ToList())
            {
                Element elsymbol = doc.GetElement(elid);
                if (elsymbol.Category.Name == "Типовые аннотации")
                {
                    AnnotationSymbol symbol = elsymbol as AnnotationSymbol;
                    if (symbol.AnnotationSymbolType.FamilyName == "# Reports - Схема - Силовой щит - Данные - ГРЩ" |
                                symbol.AnnotationSymbolType.FamilyName == "# Reports - Схема - Силовой щит - Данные"|
                                symbol.AnnotationSymbolType.FamilyName == "# Reports - Схема - Силовой щит - Данные - ГРЩ1")
                    {
                        foreach (Parameter pr in symbol.ParametersMap)
                        {
                            if (pr.Definition.Name == "Обозначение")
                            {
                                dictAnot.Add(pr.AsString(), symbol);
                            }
                        }

                    }
                }
            }
            return dictAnot;
        }

        private Dictionary<string,string> CompareAnnotations (AnnotationSymbol A1, AnnotationSymbol A2)
        {
            Dictionary<String, String> change = new Dictionary<String, String>();
            foreach (Parameter parameterА1 in A1.Parameters)
            {
                if (parameterА1.HasValue & !parameterА1.IsReadOnly 
                    & (parameterА1.Definition.Name != "Высота")
                    & (parameterА1.Definition.Name != "Image - Index"))
                {
                    if (parameterА1.StorageType == StorageType.Integer)
                    {
                        string name1 =  parameterА1.Definition.Name;
                        int value1 = parameterА1.AsInteger();
                        IList<Parameter> parameterА2 = A2.GetParameters(name1);
                        int value2 = parameterА2[0].AsInteger();
                        if (value1 != value2)
                        {
                            change.Add(name1, "Изменено  с " + value1 + " на " + value2);
                            parameterА1.Set(value2);
                        }
                    }
                    if (parameterА1.StorageType == StorageType.String)
                    {
                        string name1 = parameterА1.Definition.Name;
                        string value1 = parameterА1.AsString();
                        ElementId id1 = parameterА1.Id;
                        IList<Parameter> parameterА2 = A2.GetParameters(name1);
                        string value2 = parameterА2[0].AsString();
                        if (value1 != value2)
                        {
                            change.Add(name1, "Изменено значения с " + value1 + " на " + value2);
                            parameterА1.Set(value2);
                        }

                    }
                
                    if (parameterА1.StorageType == StorageType.Double)
                    {
                        string name1 = parameterА1.Definition.Name;
                        double value1 = parameterА1.AsDouble();
                        ElementId id1 = parameterА1.Id;
                        IList<Parameter> parameterА2 = A2.GetParameters(name1);
                        double value2 = parameterА2[0].AsDouble();
                        if (value1 != value2)
                        {
                            change.Add(name1, "Изменено значения с " + value1 + " на " + value2);
                            parameterА1.Set(value2);
                        }

                    }
                }
            }
            return  change;
        }
    }
}
