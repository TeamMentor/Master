using System;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.CoreLib.Interfaces;

namespace TeamMentor.CoreLib
{
    public class Logger_Firebase : Logger_LogItem
    {
        public static Logger_Firebase Current { get; set; }

        public API_Firebase apiFirebase = new API_Firebase(TMConsts.FIREBASE_AREA_DEBUG_MSGS);

        public override string writeMemory(string message)
        {
            //apiFirebase.push(message);
            return base.writeMemory("[Firebase] " + message);
        }
    	
        public override  Log_Item logItem(Log_Item item)
        {
            
            if (TM_UserData.Current.firebase_Log_DebugMsg())
            {
                var submitData = new API_Firebase.SubmitData(item, API_Firebase.Submit_Type.ADD);
                apiFirebase.submit(submitData);            
            }
            return item;
        }

        public static Logger_Firebase createAndHook()
        {          
            try
            {                
                "[Logger_Firebase] [createAndHook] Setting up Logger_Firebase".info();                
                var loggerFirebase = new Logger_Firebase();
                if (loggerFirebase.apiFirebase.site_Configured().isFalse())
                {
                    "[Logger_Firebase] Firebase not configured, so disabling Firebase real-time logs/data support".debug();                    
                    return null;
                }
                Current  = loggerFirebase;
                PublicDI.log.LogRedirectionTarget = Current;
                return Current;
            }
            catch(Exception ex)
            {
                ex.log();
                return null;
            }
        }
    }
}