using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using O2.Interfaces.O2Core;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
    public class MemoryLogger : IO2Log
    {
        public StringBuilder LogData         { get ; set;}     
        public IO2Log LogRedirectionTarget   { get ; set;}
        public bool alsoShowInConsole        { get ; set;}

        public MemoryLogger()
        {
            LogData = new StringBuilder();
        }
        public void d(string debugMessage)
        {
            LogData.AppendLine(debugMessage);
        }

        public void debug(string debugMessageFormat, params object[] variables)
        {
            LogData.AppendLine(debugMessageFormat.format(variables));
        }

        public void debug(string debugMessage)
        {
            LogData.AppendLine(debugMessage);
        }

        public void e(string errorMessage)
        {
            LogData.AppendLine(errorMessage);
        }

        public void error(string errorMessageFormat, params object[] variables)
        {
            LogData.AppendLine(errorMessageFormat.format(variables));
        }

        public void error(string errorMessage)
        {
            LogData.AppendLine(errorMessage);
        }

        public void ex(Exception ex, string comment, bool showStackTrace)
        {
            LogData.AppendLine("[Exception] {0} {1}".format(comment, ex.Message));
            if (showStackTrace)
                LogData.AppendLine("            " + ex.StackTrace);
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

        public void i(string infoMessage)
        {
            LogData.AppendLine(infoMessage);
        }

        public void info(string infoMessageFormat, params object[] variables)
        {
            LogData.AppendLine(infoMessageFormat.format(variables));
        }

        public void info(string infoMessage)
        {
            LogData.AppendLine(infoMessage);
        }

        public void logToChache(string text)
        {
            LogData.AppendLine(text);
        }

        public void write(string messageFormat, params object[] variables)
        {
            LogData.AppendLine(messageFormat.format(variables));
        }

        public void write(string message)
        {
            throw new NotImplementedException();
        }
    }
}
