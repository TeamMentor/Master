using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.Interfaces;

namespace FluentSharp.CoreLib.API
{
    public class Logger_File_Append : IO2Log
    {        
        public IO2Log           LogRedirectionTarget   { get ; set;}
        public bool             alsoShowInConsole      { get ; set;}
        public bool             alsoWriteToDebug       { get ; set;}        
        public StringBuilder    LogData                { get ; set;}   
 
        public String           TargetFile             { get; set; }


        public Logger_File_Append(string targetFile)
        {
            alsoWriteToDebug = true;
            LogData = new StringBuilder();
            TargetFile = targetFile;
            if (TargetFile.file_Doesnt_Exist())
            { 
                ("---------------------------------".line()+
                 "Logger_File_Append started at {0}".line().line().format(DateTime.Now)).saveAs(TargetFile);                
            }
        }

        private void writeLine(string message)
        {
            write(message.line());
        }
        public void write(string messageFormat, params object[] variables)
        {
            try
            {
                var message = variables.isNull()
                              ? messageFormat
                              : messageFormat.format(variables);
                if (alsoWriteToDebug)
                    Debug.Write(message);
                if (alsoShowInConsole)
                    Console.Write(message);
                LogData.Append(message);
                try
                {
                    var file = File.AppendText(TargetFile);
                    file.Write(message);
                    file.Close();
                }
                catch(Exception ex)
                {
                     Debug.Write("ERROR IN [Logger_DiagnosticsDebug][File.AppendText " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                Debug.Write("[FluentSharp][ERROR IN Logger_DiagnosticsDebug] " + ex.Message);		        
            }			
        }
        public void debug(string debugMessageFormat, params object[] variables)
        {
            writeLine("DEBUG: " + debugMessageFormat.format(variables));
        }
        public void error(string errorMessageFormat, params object[] variables)
        {
            writeLine("ERROR: " + errorMessageFormat.format(variables));
        }
        public void ex(Exception ex, string comment, bool showStackTrace)
        {
            writeLine("Exception: {0} {1}".format(comment, ex.Message));
            if (showStackTrace)
                writeLine("            " + ex.StackTrace);
        }
        public void ex(Exception ex, bool showStackTrace)
        {
            this.ex(ex, "", showStackTrace);
        }
        public void ex(Exception ex, string comment)
        {
            this.ex(ex, comment, false);
        }
        public void ex(Exception ex)
        {
            this.ex(ex, "", false);
        }
        public void info(string infoMessageFormat, params object[] variables)
        {
            writeLine("INFO: " + infoMessageFormat.format(variables));
        }
        public void logToChache(string text)
        {
            writeLine(text);
        }        
    }
}
