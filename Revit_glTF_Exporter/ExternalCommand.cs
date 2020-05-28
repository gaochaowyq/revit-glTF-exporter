using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;
using Microsoft.Win32;
using System.IO;

namespace Revit_glTF_Exporter
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class ExternalCommand : IExternalCommand
    {
        /// <summary>
        ///     External Command
        /// </summary>
        /// <param name ="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {

                UIApplication uiApp = commandData.Application;
                UIDocument uiDoc = uiApp.ActiveUIDocument;
                Application app = uiApp.Application;
                Document doc = uiDoc.Document;

                View3D view = doc.ActiveView as View3D;

                //Check Revit Version
                if (!commandData.Application.Application.VersionName.Contains("2018"))
                {
                    using (TaskDialog taskDialog = new TaskDialog("Cannot Continue"))
                    {
                        taskDialog.TitleAutoPrefix = false;
                        taskDialog.MainInstruction = "Incompatible Version of Revit";
                        taskDialog.MainContent = "This addin just works on Revit 2018";
                        taskDialog.Show();
                    }
                    return Result.Cancelled;
                }

                List<Element> wallCollector = new FilteredElementCollector(doc)
                                        .OfCategory(BuiltInCategory.OST_Walls)
                                        .WhereElementIsNotElementType()
                                        .ToElements()
                                        .ToList();

                List<Element> floorCollector = new FilteredElementCollector(doc)
                                        .OfCategory(BuiltInCategory.OST_Floors)
                                        .WhereElementIsNotElementType()
                                        .ToElements()
                                        .ToList();

                List<Element> furnitureCollector = new FilteredElementCollector(doc)
                                        .OfCategory(BuiltInCategory.OST_Furniture)
                                        .WhereElementIsNotElementType()
                                        .ToElements()
                                        .ToList();

                List<Element> fixedObjects = wallCollector.Concat(floorCollector).ToList();
                List<Element> movableObjects = furnitureCollector;


                Settings settings = new Settings(doc, view, movableObjects, fixedObjects);
                settings.ShowDialog();

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
        
    }
}
