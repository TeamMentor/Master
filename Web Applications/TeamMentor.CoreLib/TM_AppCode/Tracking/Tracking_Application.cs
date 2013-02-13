using System;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
    //used to track and save application specific data
    public class Tracking_Application
    {
        public static string DEFAULT_APPLICATION_LOGS_FOLDER_NAME = "Application Logs";

        public string Name              { get; set; }
        public string Location          { get; set; }

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
                var now = DateTime.Now;
                var test = now.str(); 
                tracking.Name        = DateTime.Now.str().safeFileName();
                tracking.Location    = baseFolder.pathCombine(Tracking_Application.DEFAULT_APPLICATION_LOGS_FOLDER_NAME)
                                                 .pathCombine(tracking.Name);

                tracking.Location.createDir();
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
                var logFile = tracking.Location.pathCombine("ApplicationLog.txt");
                "Saving Application Tracking Log to: {0}".info(logFile);
                var logData = O2.Kernel.PublicDI.log.LogRedirectionTarget.prop("LogData").str() ;
                logData.saveAs(logFile);
    

                var tmArticle = new TeamMentor_Article 
                                    {
                                        Metadata = { Title = "Log Files"} ,
                                        Content =  { Data  = { Value = logData } } 
                                    };
                tmArticle.saveAs(logFile + ".xml");
            }
            catch (Exception ex)
            {
                ex.log("[Tracking_Application.stop]");
            }
            return tracking;
        }
        public static  Tracking_Application save(this Tracking_Application tracking)
        {
            return tracking;
        }
    }
}
