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
using Revit_glTF_Exporter.Model;

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
                List<View> views = new FilteredElementCollector(doc)
                           .OfClass(typeof(View))
                           .ToElements()
                           .Cast<View>()
                           .ToList();

                View template1 = views.Where(x => (x.ViewName == "MovableElements" && x.IsTemplate == true)).FirstOrDefault();
                View template2 = views.Where(x => (x.ViewName == "FixedElements" && x.IsTemplate == true)).FirstOrDefault();

                bool v1 = views.Any(x=> x.ViewName == "MovableElements" && x.ViewName == "FixedElements" && x.IsTemplate == true);

                if (!v1)
                {
                    TaskDialog.Show("View Templates", "Please charge the correct View Templates");
                    return Result.Failed;
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

                List<Element> fixedObjectsCollector = wallCollector.Concat(floorCollector).ToList();
                List<Element> movableObjectsCollector = furnitureCollector;

                FixedObjects fixedObjects = new FixedObjects();

                foreach (Element element in fixedObjectsCollector)
                {
                    FixedObject fixedObject = new FixedObject();
                    fixedObject.Category = element.Category;
                    fixedObject.EId = element.Id;
                    fixedObject.ElementName = element.Name;
                    //FamilyInstance fam = element as FamilyInstance; 
                    //fixedObject.FamilySymbol = fam.Symbol.Name;
                    //fixedObject.Location = element.Location;
                    fixedObjects.ObjectsList.Add(fixedObject);
                }

                MovableObjects movableObjects = new MovableObjects();

                foreach (Element element in movableObjectsCollector)
                {
                    MovableObject movableObject = new MovableObject();
                    movableObject.Category = element.Category;
                    movableObject.EId = element.Id;
                    movableObject.ElementName = element.Name;
                    //FamilyInstance fam = element as FamilyInstance;
                    //movableObject.FamilySymbol = fam.Symbol.Name;
                    //movableObject.Location = element.Location;
                    movableObjects.ObjectsList.Add(movableObject);
                }

                Settings settings = new Settings(doc, view, fixedObjects, movableObjects);
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
