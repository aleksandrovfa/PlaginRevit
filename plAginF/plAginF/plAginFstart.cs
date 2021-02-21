using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Architecture;
// проба gitcgdf
// проба gitcgdf2ывааыва
// проба gitcgdf3
namespace plAginF
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class plAginFstart : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            FilteredElementCollector newSheetsFilter = new FilteredElementCollector(doc);
            ICollection<Element> allRooms = newSheetsFilter.OfCategory(BuiltInCategory.OST_Rooms).WhereElementIsNotElementType().ToElements();

            List<Level> allRoomLevel = new List<Level>();
            List<string> levelNames = new List<string>();
            List<Room> allRoomsList = new List<Room>();
            foreach (Element roomEl in allRooms)
            {
                Room room = roomEl as Room;
                allRoomsList.Add(room);
                Level level = room.Level;

                
                if (!levelNames.Contains(level.Name))
                {
                    levelNames.Add(level.Name);
                    allRoomLevel.Add(level);
                }
             }
            Room[] allRoomsArray = allRoomsList.ToArray();
            Array.Sort(allRoomsArray, new RoomComparerByNum());

            UserWindRoom userWind = new UserWindRoom(allRoomsArray, allRoomLevel, doc);

            userWind.ShowDialog();
            return Result.Succeeded;
        }
    }
}
