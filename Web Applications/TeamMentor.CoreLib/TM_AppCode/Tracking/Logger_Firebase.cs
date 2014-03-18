using System;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.CoreLib.Interfaces;

namespace TeamMentor.CoreLib
{
    public class Logger_Firebase : Logger_LogItem
    {
        public static Logger_Firebase Current { get; set; }

        public API_Firebase apiFirebase = new API_Firebase();

        public override string writeMemory(string message)
        {
            //apiFirebase.push(message);
            return base.writeMemory("[Firebase] " + message);
        }
    	
        public override  Log_Item logItem(Log_Item item)
        {
            apiFirebase.push(item);
            return item;
        }

        public static Logger_Firebase createAndHook()
        {          
            try
            {
                "[Logger_Firebase][createAndHook] Setting up Logger_Firebase".info();
                Current = new Logger_Firebase();
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