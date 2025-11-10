using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace AXZ
{
    public  class AXZApplication : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            throw new NotImplementedException();
        }

        public Result OnStartup(UIControlledApplication application)
        {
            throw new NotImplementedException();
        }
    }
}
