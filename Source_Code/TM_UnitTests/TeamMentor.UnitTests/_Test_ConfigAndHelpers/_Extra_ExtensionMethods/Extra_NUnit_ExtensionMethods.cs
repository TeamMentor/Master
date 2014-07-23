using System;
using System.Collections.Generic;
using System.Threading;
using FluentSharp.CoreLib;

namespace FluentSharp.NUnit
{
    public static class Extra_IO_ExtensionMethods
    {
        /// <summary>
        /// Returns a unique list of all files inside the list of folders provided
        /// </summary>
        /// <param name="folders"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public static List<string> files(this List<string> folders, bool recursive = false)
        {
            if (folders.isNull())
                return new List<string>();

            var allFiles = new List<string>();
            foreach(var folder in folders)
                if(folder.isFolder())
                    allFiles.add(folder.files(recursive));
            return allFiles.unique();
        }
    }
    public static class Extra_Thread_ExtensionMethods
    {
        public static Thread join(this Thread thread, int maxWait_Miliseconds = 2000)
        {
            if(thread.notNull())
            try
            {
                thread.Join(maxWait_Miliseconds);
            }
            catch(Exception ex)
            {
                ex.log("[Thread][join]");
            }
            return thread;
        }
    }       
}
