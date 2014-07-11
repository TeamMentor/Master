using System;
using System.Collections;
using System.Collections.Generic;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.REPL;
using FluentSharp.REPL.Controls;
using FluentSharp.Watin;
using FluentSharp.WinForms;
using WatiN.Core;
using WatiN.Core.DialogHandlers;
using WatiN.Core.Interfaces;

namespace FluentSharp.Watin
{
    public static class Extra_WatiN_IE_ExtensionMethods_DialogHandlers
    {
        //to replace the methods here: https://github.com/o2platform/FluentSharp_Fork.WatiN/blob/master/FluentSharp.WatiN/ExtensionMethods/to_Fix/WatiN_IE_ExtensionMethods_DialogHandlers.cs
        public static WatiN_IE setDialogWatcher(this WatiN_IE watinIe)
 		{ 
            if (watinIe.isNull())
                return null;
            if (watinIe.IE.DialogWatcher == null)
            {	
	            //set the value of IE.DialogWatcher with the current inproc IE dialogWatcher			

	            var dialogWatcher = DialogWatcher.GetDialogWatcher(Processes.getCurrentProcess().MainWindowHandle);		
	            var property      = PublicDI.reflection.getPropertyInfo("DialogWatcher",typeof(DomContainer)); ;
	            PublicDI.reflection.setProperty(property, watinIe.IE, dialogWatcher);
	
	            dialogWatcher.CloseUnhandledDialogs = false;
            }
            return watinIe;
        }

        public static DialogWatcher getDialogWatcher(this WatiN_IE watinIe)
 		{            
            if(watinIe.isNull() || watinIe.IE.isNull())
                return null;
 			watinIe.setDialogWatcher();
 			return watinIe.IE.DialogWatcher;
 		}
        public static WatiN_IE clear_DialogHandlers(this WatiN_IE watinIe)
 		{
 			watinIe.getDialogWatcher().clear();
 			return watinIe;
 		}
 		public static DialogWatcher clear(this DialogWatcher dialogWatcher)
 		{
 			if (dialogWatcher.notNull())
 				dialogWatcher.Clear();          // will call dialogWatcher._handlers.Clear(); 
 			return dialogWatcher;
 		}
        public static List<IDialogHandler> dialogHandlers(this WatiN_IE watinIe)
 		{
 			watinIe.setDialogWatcher();			// make sure this is set 			 			
            var dialogHandlers = (IList<IDialogHandler>)watinIe.IE.DialogWatcher.field("_handlers");
            if (dialogHandlers.notNull())
                return dialogHandlers.toList();
            return new List<IDialogHandler>(); 		 		
 		}
        public static T dialogHandler<T>(this WatiN_IE watinIe) where T : IDialogHandler 
 		{
 			foreach(var dialogHandler in watinIe.dialogHandlers())
 				if (dialogHandler is T)
 					return (T)dialogHandler;
 			return default(T);
 		}
        public static AlertAndConfirmDialogHandler getAlertsHandler(this WatiN_IE watinIe)
 		{
 			var alertHandler = watinIe.dialogHandler<AlertAndConfirmDialogHandler>();
 			if (alertHandler.isNull())
 			{
 				alertHandler = new AlertAndConfirmDialogHandler();
				watinIe.IE.AddDialogHandler(alertHandler); 
			}
			return alertHandler;
 		}

        public static AlertAndConfirmDialogHandler reset(this AlertAndConfirmDialogHandler alertHandler)
 		{
 			alertHandler.Clear();
 			return alertHandler;
 		}
        public static List<string> alerts(this AlertAndConfirmDialogHandler alertHandler)
 		{
 			return alertHandler.Alerts.toList(); 			
 		}
        public static string lastAlert(this AlertAndConfirmDialogHandler alertHandler)
 		{
 			if (alertHandler.notNull() && 
 				alertHandler.alerts().notNull() && 
 				alertHandler.alerts().size()>0) 				
 			{
	 			return alertHandler.alerts().last();
	 		}
 			return "";
 		}

        public static string open_and_HandleFileDownload(this WatiN_IE watinIe , string url, string fileName)
 		{
			var tmpFile = fileName.tempFile();
			var waitUntilHandled = 20;
			var waitUntilDownload = 300;
			
			var fileDownloadHandler = watinIe.dialogHandler<FileDownloadHandler>();

			if (fileDownloadHandler.notNull())
			{
				watinIe.IE.RemoveDialogHandler(fileDownloadHandler); 
			}
			
			fileDownloadHandler = new FileDownloadHandler(tmpFile); 
			watinIe.IE.AddDialogHandler(fileDownloadHandler); 
			
			 
			fileDownloadHandler.field("saveAsFilename",tmpFile);
			fileDownloadHandler.field("hasHandledFileDownloadDialog",false);
			
			watinIe.open_ASync(url);
			try
			{
				fileDownloadHandler.WaitUntilFileDownloadDialogIsHandled(waitUntilHandled);			
				"after: {0}".info("WaitUntilFileDownloadDialogIsHandled");
				fileDownloadHandler.WaitUntilDownloadCompleted(waitUntilDownload);
				"after: {0}".info("WaitUntilDownloadCompleted");
			}
			catch(Exception ex)
			{
				"[WatiN_IE][open_and_HandleFileDownload] {0}".error(ex.Message);
			}
				
			if (fileDownloadHandler.SaveAsFilename.fileExists())
			{
				"[WatiN_IE] downloaded ok '{0}' into '{1}'".info(url, fileDownloadHandler.SaveAsFilename);
				watinIe.IE.RemoveDialogHandler(fileDownloadHandler); 
				return fileDownloadHandler.SaveAsFilename;
			}
			"[WatiN_IE] failed to download '{0}' ".info(url);
			return null;
		}
    }
    public static class Extra_Watin_ExtensionMethods
    {   
        public static WatiN_IE add_IE(this string title)
        {
            return title.add_IE_PopupWindow();
        }
        public static WatiN_IE add_IE_PopupWindow(this string title, int width, int height)
        {
            var ie = title.add_IE_PopupWindow();
            var parentForm = ie.parentForm();
            parentForm.width(width);
            parentForm.height(height);
            return ie;
        }
        public static WatiN_IE refresh(this WatiN_IE ie)
        {
            if (ie.url().valid())
                ie.open(ie.url());            
            return ie;
        }

        //this ones need to stay inside the Unit Test
        public static WatiN_IE script_IE_WaitForClose(this WatiN_IE ie)
        {
            ie.script_IE().waitForClose();
            return ie;
        }
        public static ascx_Simple_Script_Editor script_IE(this WatiN_IE ie)
        {
            ie.parentForm().show();                                     // in case it is hidden
            var codeToAppend = "//using FluentSharp.Watin".line() +     // required refereces to allow easy scripting
                               "//using WatiN.Core       ".line() +
                               "//O2Ref:WatiN.Core.dll   ";

            var scriptEditor = ie.script_Me("ie")                       // set varaible name to 'ie' instead of 'watiN_IE'         
                                 .code_Append(codeToAppend);

            return scriptEditor;
        }
    }
}
