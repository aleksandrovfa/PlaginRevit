using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

namespace plAginF
{
    /// <summary>
    /// Логика взаимодействия для UserSheets.xaml
    /// </summary>
    public partial class UserWindRoom : Window
    {
        Room[] allRooms;
        List<Level> allLevels;
        public UserWindRoom(Room[] rooms, List<Level> levels)
        {
            InitializeComponent();
            allRooms = rooms;
            allLevels = levels;
            AllRoomsView.ItemsSource = allRooms;
            AllRoomsView.DisplayMemberPath = "Name";

        }
        private void SortRoomsInProject(Object sender, EventArgs e)
        {
            LevelViewWind levelWind = new LevelViewWind(allLevels, allRooms);
            levelWind.ShowDialog();
            //MessageBox.Show("Всё работает");
        }
    }
}
