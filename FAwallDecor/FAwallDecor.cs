using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;


namespace FAwallDecor
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class FAwallDecor : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Получение объектов приложения и документа
            UIApplication uiApp = commandData.Application;
            Document doc = uiApp.ActiveUIDocument.Document;
            Selection sel = uiApp.ActiveUIDocument.Selection;

            RoomPickFilter roomPickFilter = new RoomPickFilter();
            IList<Reference> rooms = sel.PickObjects(ObjectType.Element, roomPickFilter,"Выберите помещения для отделки");
            //MessageBox.Show("збс");
            foreach(Reference r in rooms)
            {
                Room room = doc.GetElement(r) as Room;
                IList<IList<BoundarySegment>> loops = room.GetBoundarySegments(new SpatialElementBoundaryOptions());
                foreach (IList<BoundarySegment> l in loops)
                {
                    foreach (BoundarySegment wallid in l)
                    {
                        Wall wall = doc.GetElement(wallid.ElementId) as Wall;


                    }
                }
            }


            return Result.Succeeded;
        }
    }
    public class RoomPickFilter : ISelectionFilter
    {
        public bool AllowElement(Element e)
        {
            return (e.Category.Id.IntegerValue.Equals(
            (int)BuiltInCategory.OST_Rooms));
        }
        public bool AllowReference(Reference r, XYZ p)
        {
            return false;
        }
    }
}
