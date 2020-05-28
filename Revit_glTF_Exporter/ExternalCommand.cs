using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;

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

                //You can see a list of BuiltinCategories for revit:
                //https://www.revitapidocs.com/2019/ba1c5b30-242f-5fdc-8ea9-ec3b61e6e722.htm

                Settings settings = new Settings(movableObjects, fixedObjects);
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
