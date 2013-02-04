using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using O2.DotNetWrappers.ExtensionMethods;
using RazorEngine;
using RazorEngine.Templating;

namespace TeamMentor.CoreLib
{
    //[PrincipalPermission(SecurityAction.Demand, Role = "Admin")] 
    public class TBot_Brain
    {
        public static string TBot_Main_HTML_Page    = "/TBot/TbotMain.html";
        public static string TBot_Scripts_Folder  = "/TBot";
        public static Dictionary<string,string> AvailableScripts     { get; set; }
        public static List<int>                 ScriptContentHashes  { get; set; }

        static TBot_Brain()
        {
            ScriptContentHashes = new List<int>();
            AvailableScripts = HttpContextFactory.Server.MapPath(TBot_Scripts_Folder)
                                                 .files(true, "*.cshtml")
                                                 .ToDictionary((file) => file.fileName_WithoutExtension());
        }                

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
                /*if (askFile.fileExists())
                {
                    var code = askFile.fileContents();
                    var returnValue = code.compileAndExecuteCodeSnippet().str();
                    return GetHtml(returnValue);
                }*/                
                if (AvailableScripts.hasKey(what))
                {
                    var csFile = AvailableScripts[what];
                    var templateService = (ITemplateService) typeof (Razor).prop("TemplateService");
                    var fileContents = csFile.fileContents();
                    var fileContentsHash = fileContents.hash();
                    if (templateService.HasTemplate(csFile).isFalse() || ScriptContentHashes.contains(fileContentsHash).isFalse())
                    {                        
                        Razor.Compile(fileContents, csFile);
                        ScriptContentHashes.add(fileContentsHash);
                    }
                    return GetHtml(Razor.Run(csFile), false);
                    //return GetHtml(Razor.Parse(csFile.fileContents()));
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
            var filesHtml = "Here are the commands I found:<ul>";
            foreach (var items in AvailableScripts)
            {                
                filesHtml += "<li><a href='/rest/tbot/ask/{0}'>{0}</a> - {1}</li>".format(items.Key, items.Value.fileContents().hash());                    
            }
            filesHtml += "</ul>";
            return GetHtml(filesHtml, false);
            
        }
        
    }
}
