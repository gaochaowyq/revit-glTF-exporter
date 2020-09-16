using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Win32;
using Revit_glTF_Exporter.Model;

namespace Revit_glTF_Exporter
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>

    
    public partial class Settings : Window
    {
        Document _doc;
        View3D _view;
        ElementId _CurrentTemplateId;
        View _templateMovableElements;
        View _templateFixedElements;
        List<View> _templates;
        public Settings(Document doc, View3D view, FixedObjects _fixedObjects, MovableObjects _movableObjects)
        {
            InitializeComponent();

            this._doc = doc;
            this._view = view;
            this._templates = new List<View>();


            List<View> views = new FilteredElementCollector(doc)
                            .OfClass(typeof(View))
                            .ToElements()
                            .Cast<View>()
                            .ToList();
#if REVIT2019 || REVIT2020 || REVIT2021
            _templateMovableElements = views.Where(x => (x.Name == "MovableElements" && x.IsTemplate == true)).FirstOrDefault();
            _templateFixedElements = views.Where(x => (x.Name == "FixedElements" && x.IsTemplate == true)).FirstOrDefault();
#else
            _templateMovableElements = views.Where(x => (x.ViewName == "MovableElements" && x.IsTemplate == true)).FirstOrDefault();
            _templateFixedElements = views.Where(x => (x.ViewName == "FixedElements" && x.IsTemplate == true)).FirstOrDefault();
#endif



            _templates.Add(_templateMovableElements);
            _templates.Add(_templateFixedElements);

            _CurrentTemplateId = _view.ViewTemplateId;

           





            fixedObjects.ItemsSource = _fixedObjects.ObjectsList;
            movableObjects.ItemsSource = _movableObjects.ObjectsList;
        }

        private void ExportMovableElements(object sender, RoutedEventArgs e)
        {
            if (_view == null)
            {
                TaskDialog.Show("glTFRevitExport", "You must be in a 3D view to export.");
            }
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.FileName = "NewProject"; // default file name
            fileDialog.DefaultExt = ".gltf"; // default file extension

            bool? dialogResult = fileDialog.ShowDialog();
            if (dialogResult == true)
            {
                string filename = fileDialog.FileName;
                string directory = System.IO.Path.GetDirectoryName(filename) + "\\";

                ExportView3D(_view, filename, directory, true);
            }
        }

        private void ExportFixedElements(object sender, RoutedEventArgs e)
        {
            if (_view == null)
            {
                TaskDialog.Show("glTFRevitExport", "You must be in a 3D view to export.");
            }
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.FileName = "NewProject"; // default file name
            fileDialog.DefaultExt = ".gltf"; // default file extension

            bool? dialogResult = fileDialog.ShowDialog();
            if (dialogResult == true)
            {
                string filename = fileDialog.FileName;
                string directory = System.IO.Path.GetDirectoryName(filename) + "\\";

                ExportView3D(_view, filename, directory, false);
            }
        }

        public void ExportView3D(View3D view3d, string filename, string directory, bool mode)
        {

            Document doc = view3d.Document;
            View _template;
            if (mode)
                _template = _templates[0];
            else
                _template = _templates[1];


            using (Transaction t = new Transaction(doc, "Transaction Name"))
            {
                t.Start();
                view3d.ViewTemplateId = _template.Id;
                t.Commit();
            }
                
            // Use our custom implementation of IExportContext as the exporter context.
            glTFExportContext ctx = new glTFExportContext(doc, filename , directory + "\\");
            // Create a new custom exporter with the context.
            CustomExporter exporter = new CustomExporter(doc, ctx);
                
            exporter.ShouldStopOnError = true;
            exporter.Export(view3d);
            using (Transaction t = new Transaction(doc, "Transaction Name"))
            {
                t.Start();
                view3d.ViewTemplateId = _CurrentTemplateId;
                t.Commit();
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
