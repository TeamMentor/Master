using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using O2.DotNetWrappers.ExtensionMethods;
using RazorEngine;
using RazorEngine.Templating;

namespace TeamMentor.CoreLib
{
    //[Admin] 
    public class TBot_Brain
    {
        public static string TBOT_MAIN_HTML_PAGE    = "/TBot/TbotMain.html";
        public static string TBOT_SCRIPTS_FOLDER  = "/TBot";
        public static Dictionary<string,string> AvailableScripts     { get; set; }
        public static List<int>                 ScriptContentHashes  { get; set; }
        public static ITemplateService          TemplateService { get; set; }

        public DateTime         StartTime       { get; set; }
        
        static TBot_Brain()
        {
            ScriptContentHashes = new List<int>();
            TemplateService  = (ITemplateService) typeof (Razor).prop("TemplateService");
            AvailableScripts = HttpContextFactory.Server.MapPath(TBOT_SCRIPTS_FOLDER)
                                                 .files(true, "*.cshtml")
                                                 .ToDictionary((file) => file.fileName_WithoutExtension());
        }

        public TBot_Brain()
        {
            StartTime = DateTime.Now;            
        }

        public Stream GetHtml(string content, bool htmlEncode = true)
        {
            var tbotMainHtmlFile = HttpContextFactory.Server.MapPath(TBOT_MAIN_HTML_PAGE);
            var tbotMainHtml = (tbotMainHtmlFile.fileExists())
                                    ? tbotMainHtmlFile.fileContents()
                                    : "[TBot] could not find file: {0}".format(tbotMainHtmlFile);            
            
            var html = tbotMainHtml.format((htmlEncode) ? content.htmlEncode() : content);
            var executionTime = DateTime.Now - StartTime;
            html += "<hr>script executed in: {0}s".format(executionTime.TotalSeconds);
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
                if (AvailableScripts.hasKey(what))
                {
                    var csFile = AvailableScripts[what];
                    
                    var fileContents = csFile.fileContents();
                    var fileContentsHash = fileContents.hash();
                    if (TemplateService.HasTemplate(csFile).isFalse() || ScriptContentHashes.contains(fileContentsHash).isFalse())
                    {                        
                        Razor.Compile(fileContents, csFile);
                        ScriptContentHashes.add(fileContentsHash);
                    }
                    return GetHtml(Razor.Run(csFile, new TM_REST()).trim(), false);
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
            var filesHtml = AvailableScripts.Aggregate("Here are the commands I found:<ul>", 
                                (current, items) => current + "<li><a href='/rest/tbot/ask/{0}'>{0}</a> - {1}</li>"
                                                                .format(items.Key, items.Value.fileContents().hash()));
            filesHtml += "</ul>";
            return GetHtml(filesHtml, false);            
        }                
    }
}
