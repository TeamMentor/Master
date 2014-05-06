using System;
using System.Text;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;

namespace TeamMentor.CoreLib
{
    //used to track and save application specific data
    public class Tracking_Application
    {
        public string Name              { get; set; }
        public string LogFilesLocation  { get; set; }
        public string LogFilePath       { get; set; }

        public Tracking_Application(string baseFolder)
        {
            this.start(baseFolder);
        }
    }

    public static class Tracking_Application_ExtensionMethods
    {        
        public static Tracking_Application start(this Tracking_Application tracking, string baseFolder)
        {
            if (baseFolder.valid())                
                try
                {                                
                    tracking.LogFilesLocation  = baseFolder.pathCombine(TMConsts.APPLICATION_LOGS_FOLDER_NAME);
                    tracking.LogFilesLocation.createDir();

                    tracking.Name        = DateTime.Now.str().safeFileName();
                    tracking.LogFilePath = tracking.LogFilesLocation.pathCombine("{0}_TMApplicationLogs.txt".format(tracking.Name));                
                }
                catch (Exception ex)
                {
                    ex.log("[Tracking_Application.start]");
                }            
            return tracking;
        }
        public static  Tracking_Application stop(this Tracking_Application tracking)
        {
            return tracking.saveLog();
        }        
        public static  Tracking_Application saveLog(this Tracking_Application tracking)
        {
            try
            {
                var logData = PublicDI.log.LogRedirectionTarget.prop("LogData").str() ;
                if (logData.notNull())
                {                    
                    var logFile = tracking.LogFilePath;                    
                    "[Tracking_Application Saving Application Tracking Log to: {0}".info(logFile);
                    logData.saveAs(logFile);
                }
            }
            catch (Exception ex)
            {
                ex.log("[Tracking_Application] in saveLog");
            }
            return tracking;
        }
        public static string logData(this Tracking_Application tracking)
        {
            return PublicDI.log.LogRedirectionTarget.prop("LogData").str();
        }
        public static Tracking_Application clearLog(this Tracking_Application tracking, bool saveLog = true)
        {
            var logData = PublicDI.log.LogRedirectionTarget.prop("LogData");
            if (logData.notNull() && logData is StringBuilder)
                (logData as StringBuilder).Clear();
                   
            return (saveLog) 
                        ? tracking.saveLog() 
                        : tracking;
        }
    }
}
