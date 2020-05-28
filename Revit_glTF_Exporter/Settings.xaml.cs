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
using Autodesk.Revit.UI;


namespace Revit_glTF_Exporter
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings(List<Element> _fixedObjects, List<Element> _movableObjects)
        {
            InitializeComponent();

            fixedObjects.ItemsSource = _fixedObjects;
            movableObjects.ItemsSource = _movableObjects;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
