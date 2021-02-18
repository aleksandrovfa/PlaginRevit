using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace PBGS_update_switchboard
{
    class SheetsComparerByNum : IComparer<ViewSheet>
    {
        public int Compare(ViewSheet x, ViewSheet y)
        {
            if (Convert.ToDouble(x.SheetNumber) > Convert.ToDouble(y.SheetNumber))
                return 1;
            else if (Convert.ToDouble(x.SheetNumber) < Convert.ToDouble(y.SheetNumber))
                return -1;
            else
                return 0;
        }
    }
}
