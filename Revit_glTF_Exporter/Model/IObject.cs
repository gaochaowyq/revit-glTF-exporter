using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace Revit_glTF_Exporter.Model
{
    public interface IObject
    {
        Category Category { get; set; }

        FamilySymbol FamilySymbol { get; set; }

        string ElementName { get; set; }

        ElementId ElementId { get; set; }

        XYZ Location { get; set; }
    }
}
