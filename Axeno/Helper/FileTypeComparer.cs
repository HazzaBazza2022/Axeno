using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axeno.Helper
{
    public class FileTypeComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            FileManagerlv fx = x as FileManagerlv;
            FileManagerlv fy = y as FileManagerlv;

            if (fx == null || fy == null)
            {
                return 0;
            }

            if (fx.fType == "Folder" && fy.fType == "File")
            {
                return -1;
            }
            else if (fx.fType == "File" && fy.fType == "Folder")
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }

}
