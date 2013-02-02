using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
    [PrincipalPermission(SecurityAction.Demand, Role = "Admin")] 
    public class TBot_Brain
    {
        public static string TBot_Main_HTML_Page    = "/TBot/TbotMain.html";
        public static string TBot_Questions_Folder  = "/TBot/Questions";

        public Stream GetHtml(string content)
        {
            var tbotMainHtmlFile = HttpContextFactory.Server.MapPath(TBot_Main_HTML_Page);
            var tbotMainHtml = (tbotMainHtmlFile.fileExists())
                                    ? tbotMainHtmlFile.fileContents()
                                    : "[TBot] could not find file: {0}".format(tbotMainHtmlFile);            
            var html = tbotMainHtml.format(content.htmlEncode());		
            return html.stream_UFT8();
        }

        public Stream RenderPage()
        {
            var message = "this is some message";
            return GetHtml(message);
        }

        public Stream Ask(string what)
        {                        
            var askFile = HttpContextFactory.Server.MapPath(TBot_Questions_Folder).pathCombine(what.urlEncode());
            if (askFile.fileExists())
            {
                var code = askFile.fileContents();
                var returnValue = code.compileAndExecuteCodeSnippet().str();
                return GetHtml(returnValue);
            }
            return GetHtml("Couldn't find requested question");            
        }

        public Stream List()
        {
            var questionsFolder = HttpContextFactory.Server.MapPath(TBot_Questions_Folder);
            if (questionsFolder.dirExists())
            {
                var files = questionsFolder.files(true).join(",");
                
                return GetHtml(files);
            }
            return GetHtml("Couldn't find the Questions Folders");            
        }
    }
}
