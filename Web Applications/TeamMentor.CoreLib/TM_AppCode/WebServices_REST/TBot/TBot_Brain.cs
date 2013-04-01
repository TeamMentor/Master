using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using O2.DotNetWrappers.ExtensionMethods;
using RazorEngine;
using RazorEngine.Templating;

namespace TeamMentor.CoreLib
{    
    public class TBot_Brain
    {
        public static string TBOT_MAIN_HTML_PAGE    = "/TBot/TbotMain.html";
        public static string TBOT_SCRIPTS_FOLDER  = "/TBot";
        public static Dictionary<string,string> AvailableScripts     { get; set; }
        public static List<int>                 ScriptContentHashes  { get; set; }
        public static ITemplateService          TemplateService { get; set; }

        public DateTime         StartTime       { get; set; }
        public ITM_REST         TmRest          { get; set; }

        static TBot_Brain()
        {
            try
            {
                ScriptContentHashes = new List<int>();
                TemplateService  = (ITemplateService) typeof (Razor).prop("TemplateService");
                AvailableScripts = GetAvailableScripts();

                //fix for missing "System.Windows.Forms" reference that sometimes appeared
                var windowsForms = "System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089".assembly();
                var assemblies =  (ConcurrentBag<Assembly>)TemplateService.field("_assemblies");
                assemblies.Add(windowsForms);
            }
            catch (Exception ex)
            {
                ex.log();
            }
            
        }
        
        [Admin]
        public TBot_Brain(ITM_REST tmRest)
        {
            TmRest = tmRest;
            StartTime = DateTime.Now;       
            
        }

        public Stream GetHtml(string content)
        {
            return GetHtml(content, true);
        }
        public Stream GetHtml(string content, bool htmlEncode)
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

        public string ExecuteRazorPage(string page)
        {
            try
            {                
                if (AvailableScripts.hasKey(page))
                {                    
                    var csFile = AvailableScripts[page];

                    var fileContents = csFile.fileContents();
                    var fileContentsHash = fileContents.hash();
                    if (TemplateService.HasTemplate(csFile).isFalse() || ScriptContentHashes.contains(fileContentsHash).isFalse())
                        {
                            Razor.Compile(fileContents, csFile);
                            ScriptContentHashes.add(fileContentsHash);
                        }
                    return Razor.Run(csFile, TmRest);
                }
            }
            catch (Exception ex)
            {
                return "Opps: Something went wrong: {0}".format(ex.Message);
            }
            return null;
        }

        public Stream Run(string page)
        {                                  
            var result = ExecuteRazorPage(page);
            return result.valid() 
                    ? GetHtml(result.trim(), false) 
                    : GetHtml("Couldn't find requested actions");                            
        }
        public Stream List()
        {                        
            var filesHtml = AvailableScripts.Aggregate("Here are the commands I found:<ul>", 
                                (current, items) => current + "<li><a href='/rest/tbot/run/{0}'>{0}</a> - {1}</li>"
                                                                .format(items.Key, items.Value.fileContents().hash()));
            filesHtml += "</ul>";
            return GetHtml(filesHtml, false);            
        }


        // static methods

        public static string TBotScriptsFolder()
        {
            return HttpContextFactory.Server.MapPath(TBOT_SCRIPTS_FOLDER);
        }
        public static List<string> TBotScriptsFiles()
        {
            var files = TBotScriptsFolder().files(true, "*.cshtml");
            var userDataFolder = TM_UserData.Current.Path_UserData.pathCombine("TBot");
            if (userDataFolder.dirExists())
                files.add(userDataFolder.files(true, "*.cshtml"));
            return files;
        }
        public static Dictionary<string, string> GetAvailableScripts()
        {
            var files = TBotScriptsFiles();
            var mappings = new Dictionary<string, string>();
            foreach (var file in files)
                mappings.add(file.fileName_WithoutExtension(), file);            
            return mappings;
            //return files.toDictionary((file) => file.fileName_WithoutExtension()); //this doesn't handle duplicate files names
        }
    }

    public static class TBOT_ExtensionMethods
    {
        public static List<string> scriptsNames(this TBot_Brain tBot)
        {
            return TBot_Brain.AvailableScripts.Keys.toList();
        }
    }
}
