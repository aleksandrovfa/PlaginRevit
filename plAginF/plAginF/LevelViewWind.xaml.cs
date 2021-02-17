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
        public LevelViewWind(List<Level> levels, Room[] rooms)
        {
            InitializeComponent();
            allRooms = rooms;
            allLevels = levels;

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
        }
    }
}
