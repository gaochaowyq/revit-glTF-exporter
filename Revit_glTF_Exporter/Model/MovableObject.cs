using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace Revit_glTF_Exporter.Model
{
    class MovableObject
    {
        Category Category { get; set; }
        FamilySymbol FamilySymbol { get; set; }
        string ElementName { get; set; }
        ElementId ElementId { get; set; }
        XYZ Location { get; set; }
    }
}
