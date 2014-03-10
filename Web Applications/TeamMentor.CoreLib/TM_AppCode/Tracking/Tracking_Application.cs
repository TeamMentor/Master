using System;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;

namespace TeamMentor.CoreLib
{
    //used to track and save application specific data
    public class Tracking_Application
    {
        public static string DEFAULT_APPLICATION_LOGS_FOLDER_NAME = "Application_Logs";

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
            try
            {                                
                tracking.LogFilesLocation  = baseFolder.pathCombine(Tracking_Application.DEFAULT_APPLICATION_LOGS_FOLDER_NAME);
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
            try
            {
                return tracking.saveLog();
            }
            catch (Exception ex)
            {
                ex.log("[Tracking_Application.stop]");
            }
            return tracking;
        }
        public static  Tracking_Application saveLog(this Tracking_Application tracking)
        {
            try
            {
                var logData = PublicDI.log.LogRedirectionTarget.prop("LogData").str() ;
                if (logData.notNull())
                {
                    //tracking.Name        = DateTime.Now.str().safeFileName();
                    var logFile = tracking.LogFilePath;
                    //var logFile = tracking.Location.pathCombine("ApplicationLog.txt");
                    "Saving Application Tracking Log to: {0}".info(logFile);

                    var tmArticle = new TeamMentor_Article
                        {
                            Metadata = {Title = "Log Files"},
                            Content = {Data = {Value = logData}}
                        };
                    tmArticle.saveAs(logFile + ".xml");

                    logData.saveAs(logFile);
                }
            }
            catch (Exception ex)
            {
                ex.log("[Tracking_Application] in saveLog");
            }
            return tracking;
        }
    }
}
