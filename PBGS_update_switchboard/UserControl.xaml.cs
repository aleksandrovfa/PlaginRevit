﻿using System;
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
    }
}
