using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamMentor.CoreLib
{
    public class TM_Status
    {
        public static bool Loading_UserData    { get; set; }
        public static bool Loading_XmlDatabase { get; set; }
        public static bool Loaded_UserData { get; set; }
        public static bool Loaded_XmlDatabase { get; set; }
        public static bool In_Setup_XmlDatabase { get; set; }
        
        static TM_Status()
        {
            reset();
        }
        public static void reset()
        {
            Loading_UserData        = false;
            Loading_XmlDatabase     = false;
            Loaded_UserData         = false;
            Loaded_XmlDatabase      = false;
            In_Setup_XmlDatabase    = false;
        }
    }
}
