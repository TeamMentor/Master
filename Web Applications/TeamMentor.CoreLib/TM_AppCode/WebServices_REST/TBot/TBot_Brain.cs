using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using O2.DotNetWrappers.ExtensionMethods;
using RazorEngine;

namespace TeamMentor.CoreLib
{
    //[PrincipalPermission(SecurityAction.Demand, Role = "Admin")] 
    public class TBot_Brain
    {
        public static string TBot_Main_HTML_Page    = "/TBot/TbotMain.html";
        public static string TBot_Questions_Folder  = "/TBot/Questions";

        public Stream GetHtml(string content, bool htmlEncode = true)
        {
            var tbotMainHtmlFile = HttpContextFactory.Server.MapPath(TBot_Main_HTML_Page);
            var tbotMainHtml = (tbotMainHtmlFile.fileExists())
                                    ? tbotMainHtmlFile.fileContents()
                                    : "[TBot] could not find file: {0}".format(tbotMainHtmlFile);            
            
            var html = tbotMainHtml.format((htmlEncode) ? content.htmlEncode() : content);		
            return html.stream_UFT8();
        }

        public Stream RenderPage()
        {
            var message = "this is some message";
            return GetHtml(message);
        }

        public Stream Ask(string what)
        {
            try
            {
                var askFile = HttpContextFactory.Server.MapPath(TBot_Questions_Folder).pathCombine(what.urlEncode());
            if (askFile.fileExists())
            {
                var code = askFile.fileContents();
                var returnValue = code.compileAndExecuteCodeSnippet().str();
                return GetHtml(returnValue);
            }
            var csFile = askFile + ".cshtml";
            if (csFile.fileExists())
            {
                return GetHtml(Razor.Parse(csFile.fileContents()));
                //return GetHtml("Found .cshtml file: " + csFile);
            }
            return GetHtml("Couldn't find requested question");            
            }
            catch (Exception ex)
            {
                return GetHtml("Opps: Something went wrong: {0}".format(ex.Message));
            }     
            
        }

        public Stream List()
        {
            var questionsFolder = HttpContextFactory.Server.MapPath(TBot_Questions_Folder);
            if (questionsFolder.dirExists())
            {                
                var filesHtml = "Here are the commands I found:<ul>";
                foreach (var file in questionsFolder.files(true))
                {
                    var name = file.fileName_WithoutExtension();
                    filesHtml += "<li><a href='/rest/tbot/ask/{0}'>{0}</a></li>".format(name);
                }
                filesHtml += "</ul>";
                return GetHtml(filesHtml, false);
            }
            return GetHtml("Couldn't find the Questions Folders");            
        }
    }
}
