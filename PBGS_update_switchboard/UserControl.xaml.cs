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

namespace PBGS_update_switchboard
{
    /// <summary>
    /// Логика взаимодействия для UserControl.xaml
    /// </summary>
    public partial class UserControl : Window
    {
        ViewSheet[] allSheets;
        View[] аllViewDrawting;
        public List<string> ALLCHEKEDVIEW;
        public List<string> ALLCHECKEDSHEET;

        public UserControl(ViewSheet[] allSheetsArray, View[] аllViewDrawtingArray)
        {
            InitializeComponent();
            allSheets = allSheetsArray;
            foreach (ViewSheet sheet in allSheets)
            {
                CheckBox checksheet = new CheckBox();
                checksheet.Content = (sheet.Title);
                SheetPanel.Children.Add(checksheet);
            }

            аllViewDrawting = аllViewDrawtingArray;
            foreach (View view in аllViewDrawting)
            {
                CheckBox checkview = new CheckBox();
                checkview.Content = (view.Name);
                ViewPanel.Children.Add(checkview);
            }
        }
        private void Update(Object sender, EventArgs e)
        {
            UIElementCollection comboBoxesSheet = SheetPanel.Children;
            this.ALLCHECKEDSHEET = new List<string>();
            foreach (UIElement element in comboBoxesSheet)
            {
                CheckBox checkBox = element as CheckBox;
                if (checkBox.IsChecked == true)
                    ALLCHECKEDSHEET.Add(checkBox.Content.ToString());
            }

            UIElementCollection comboBoxesView = ViewPanel.Children;
            this.ALLCHEKEDVIEW = new List<string>();
            foreach (UIElement element in comboBoxesView)
            {
                CheckBox checkBox = element as CheckBox;
                if (checkBox.IsChecked == true)
                    ALLCHEKEDVIEW.Add(checkBox.Content.ToString());
            }

            if (ALLCHECKEDSHEET.Count == 0)
                MessageBox.Show("Не выбрано ни одного листа,\nхоть один то надо выбрать.");
            else if (ALLCHEKEDVIEW.Count == 0)
                MessageBox.Show("Не выбрано ни одного чертежного вида");
            else if (ALLCHEKEDVIEW.Count > 1)
                MessageBox.Show("Выбрано больше одного чертежного вида.\nСинхронизации на несколько листов не будет. :)");
            else this.Close();
        }


    }
}
