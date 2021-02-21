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
    /// Логика взаимодействия для LevelView.xaml
    /// </summary>
    public partial class LevelViewWind : Window
    {
        List<Level> allLevels;
        Room[] allRooms;
        Document _doc;
        public LevelViewWind(List<Level> levels, Room[] rooms, Document doc)
        {
            InitializeComponent();
            allRooms = rooms;
            allLevels = levels;
            _doc = doc;

            foreach(Level level in allLevels)
            {
                CheckBox checkLevel = new CheckBox();
                checkLevel.Content = level.Name;

                AllLevelsView.Children.Add(checkLevel);
            }
            
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            double startValue = Convert.ToDouble(StartNumberValueView.Text);
            UIElementCollection comboBoxes = AllLevelsView.Children;

            //ComboBox comboBox = comboBoxes[0] as ComboBox;
            List<String> allCheckedLevels = new List<string>();
            foreach (UIElement element in comboBoxes)
            {
                CheckBox checkBox = element as CheckBox;
                //if ()
                if (checkBox.IsChecked == true)
                    allCheckedLevels.Add(checkBox.Content.ToString());
            }
            using(Transaction t = new Transaction(_doc))
            {
                t.Start("SetNumber");
            for(int i = 0; i<allRooms.Count(); i++)
            {
                Room room = allRooms[i];
                string roomLevelName = room.Level.Name;
                if (allCheckedLevels.Contains(roomLevelName))
                    {
                        double newNumber = startValue + i;
                        room.get_Parameter(BuiltInParameter.ROOM_NUMBER).Set(newNumber.ToString());
                    }
            }
                t.Commit();

            }
        }
    }
}
