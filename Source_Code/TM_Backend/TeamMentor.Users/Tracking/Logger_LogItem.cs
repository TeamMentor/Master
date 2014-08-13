using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.Interfaces;

namespace TeamMentor.CoreLib
{
    public class Log_Item
    {
    	public string   When { get; set; }
    	public string   Type { get; set; }
    	public string   Text { get; set; }
    	
    	public Log_Item()
    	{
    		When = DateTime.Now.jsDate();
    		Type = "NA";
    		Text = "....";
    	}
    	public Log_Item(String type,  String text) : this()
    	{
    		Type = type;
    		Text = text;
    	}
    }

    public class Logger_LogItem : IO2Log
    {        
        public IO2Log           LogRedirectionTarget   { get ; set;}
        public bool             alsoShowInConsole      { get ; set;}
        public StringBuilder    LogData                { get ; set;}     
        public List<Log_Item>   LogItems               { get ; set;}     

        public Logger_LogItem()
        {
            LogData = new StringBuilder();
            LogItems = new List<Log_Item>();
        }
        
        public virtual Log_Item logItem(string type, string text)
        {
            return logItem(new Log_Item(type, text));	
        }
        public virtual Log_Item logItem(Log_Item logItem)
        {
            LogItems.add(logItem);
            return logItem;
        }

        public virtual string writeMemory(string message)
        {    
            if (message.valid())
                LogData.AppendLine(message);
            return message;
        }        

        public void write(string messageFormat, params object[] variables)
        {        	
            
            var message = variables.isNull()
                                ? messageFormat
                                : messageFormat.format(variables);               
            writeMemory(message);            			
        }

        public List<string> lines()
        {
            return LogData.str().lines();            
        }

        // from IO2Log interface
        public void debug(string debugMessageFormat, params object[] variables)
        {
            logItem("DEBUG", debugMessageFormat.format(variables));	
            write  ("DEBUG: " + debugMessageFormat.format(variables));
        }
        public void error(string errorMessageFormat, params object[] variables)
        {
            logItem("ERROR", errorMessageFormat.format(variables));
            write  ("ERROR: " + errorMessageFormat.format(variables));
        }
        public void ex(Exception ex, string comment, bool showStackTrace)
        {
            var exceptionMessage = "{0} {1}".format(comment, ex.Message);
            if (showStackTrace && ex.StackTrace.valid())
                exceptionMessage += ("            " + ex.StackTrace).lineBefore();
            logItem("EXCEPTION", exceptionMessage);
            write  ("EXCEPTION: " + exceptionMessage);            
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
            logItem("INFO", infoMessageFormat.format(variables));
            write  ("INFO: " + infoMessageFormat.format(variables));
        }
        public void logToChache(string text)
        {
            write(text);
        }        
    }
    
}