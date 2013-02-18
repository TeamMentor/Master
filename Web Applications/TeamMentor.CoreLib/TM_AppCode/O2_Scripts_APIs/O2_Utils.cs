using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
    public class O2_Utils
    {
        public static void showLogViewer_if_LocalHost()
        {
            try
            {                
                if (HttpContextFactory.Context.Request.IsLocal)                
                    "".popupWindow().add_LogViewer();          
            }
            catch (Exception ex)
            {
                ex.log();
            }
        }
    }
}
