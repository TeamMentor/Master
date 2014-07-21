using System;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    [Serializable]
    public class TM_Status : MarshalByRefObject
    {        
        public static string      Version                             {	get; set; }
        /// <summary>
        /// Gets the current TM_Status object
        /// </summary>
        public static TM_Status   Current                             { get; set; }
        public Database_Status    TM_Database_Status                  { get; set; }
        public bool               TM_Database_In_Setup_Workflow       { get; set; }
        public bool               TM_Database_Location_Using_AppData  { get; set; }
        
        /// <summary>
        /// Static ctor of TM_Status
        /// 
        /// This one of the few TM object that is garanteed to always exist (due to the presense of this static ctor)
        /// </summary>
        static TM_Status()
        {
            Version = typeof(TM_Status).assembly().version();
            Current = new TM_Status();
        }
        public TM_Status()
        {            
            TM_Database_Status                 = Database_Status.Not_Initialized;
            TM_Database_In_Setup_Workflow      = false;
            TM_Database_Location_Using_AppData = false;             
        }        

        public enum Database_Status
        { 
            Not_Initialized, 
            Ready,
            Loading_XmlDatabase, 
            Loading_UserData
        }
    }
}
