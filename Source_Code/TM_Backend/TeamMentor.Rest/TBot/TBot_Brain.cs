using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentSharp.CoreLib;
using FluentSharp.Web;
using FluentSharp.Web35;
using RazorEngine;
using RazorEngine.Templating;
using TeamMentor.FileStorage;

namespace TeamMentor.CoreLib
{    
    [Serializable]
    public class TBot_Brain : MarshalByRefObject
    {
        public static string TBOT_MAIN_HTML_PAGE    = "/TBot/TbotMain.html";
        public static string TBOT_SCRIPTS_FOLDER  = "/TBot";
        public static string                    TBotScriptsFolder    { get; set; }
        public static Dictionary<string,string> AvailableScripts     { get; set; }
        public static List<int>                 ScriptContentHashes  { get; set; }
        public static ITemplateService          TemplateService      { get; set; }
         

        //public DateTime         StartTime       { get; set; }
        public TM_REST         TmRest          { get; set; }

        static TBot_Brain()
        {
            try
            {                
                ScriptContentHashes = new List<int>();
                TemplateService  = (ITemplateService) typeof (Razor).prop("TemplateService");
                TBotScriptsFolder = HttpContextFactory.Server.notNull() ? HttpContextFactory.Server.MapPath(TBOT_SCRIPTS_FOLDER) : null;
                SetAvailableScripts();
            }
            catch (Exception ex)
            {
                ex.log();
            }
            
        }
        private TBot_Brain()
        { }
        //[Admin]        
        public TBot_Brain(TM_REST tmRest)
        {
            TmRest = tmRest;
            checkIfUserIsAdmin();
            
            //StartTime = DateTime.Now;       
            
        }
        public Guid user_SessionId()
        {
            return TmRest.TmWebServices.tmAuthentication.sessionID;
        }
        public  string user_CsrfToken()
        {
            return user_SessionId().csrfToken();
        }
        public void checkIfUserIsAdmin()
        {
            // so that we can have direct links to TBOT pages
            // need to check it like this since the CSRT is not vailable 
            var sessionId = user_SessionId();
            if (sessionId.validSession())
            {
                var tmUser     = sessionId.session_TmUser();
                var groupId    = sessionId.session_GroupID();                
                if (tmUser.notNull() && groupId == (int)UserGroup.Admin)
                    UserGroup.Admin.setThreadPrincipalWithRoles();    
            }
            UserRole.Admin.demand();
        }

        public Stream GetHtml(string content)
        {
            return GetHtml(content, true,-1);
        }
        public Stream GetHtml(string content, bool htmlEncode, double executionTime)
        {
            var tbotMainHtmlFile = HttpContextFactory.Server.MapPath(TBOT_MAIN_HTML_PAGE);
            var tbotMainHtml = (tbotMainHtmlFile.fileExists())
                                    ? tbotMainHtmlFile.fileContents()
                                    : "[TBot] could not find file: {0}".format(tbotMainHtmlFile);            
            
            
            var html = tbotMainHtml.replace("{{TBOT_PAGE}}", (htmlEncode) ? content.htmlEncode() : content)
                                   .replace("{{CSRFToken}}", user_CsrfToken())
                                   .replace("{{ExecutionTime}}", executionTime.str());            
            
            return html.stream_UFT8();
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
                if (ex.Message =="Thread was being aborted.")
                    return "";

                ex.log("[TBot Brain] [ExecuteRazorPage] {0} : {1}".format(page, ex.Message));
                return "Opps: Something went wrong: {0}".format(ex.Message);
            }
            return "";
        }
        public Stream Render(string page)
        {
            var result = ExecuteRazorPage(page);            
            return (result.valid())
                    ? result.stream_UFT8()
                    : "".stream_UFT8();            
        }
        public Stream Json(string page)
        {
            var result = ExecuteRazorPage(page);                 

            return (result.valid())
                    ? result.stream_UFT8()
                    : "{}".stream_UFT8();            
        }
        public Stream Run(string page)
        {           
            var start = DateTime.Now;           
            var result = ExecuteRazorPage(page);
            return result.valid() 
                    ? GetHtml(result.trim(), false, (DateTime.Now - start).TotalSeconds) 
                    : GetHtml("Couldn't find requested actions");                            
        }
        public Stream List()
        {                        
            var filesHtml = AvailableScripts.Aggregate("Here are the commands I found:<ul>", 
                                (current, items) => current + "<li><a href='/rest/tbot/run/{0}'>{0}</a> - {1}</li>"
                                                                .format(items.Key, items.Value.fileContents().hash()));
            filesHtml += "</ul>";
            return GetHtml(filesHtml, false,-1);            
        }

        public Dictionary<string,string> availableScripts()
        {
            return TBot_Brain.AvailableScripts;
        }        
        // static methods

        public static TBot_Brain Create()
        {            
            return new TBot_Brain();
        }
        
        public static List<string> TBotScriptsFiles()
        {
            var files = TBotScriptsFolder.files(true, "*.cshtml");            
            if (TM_FileStorage.Current.notNull())
            {
                var siteDataFolder = TM_FileStorage.Current.path_SiteData().pathCombine("TBot");
                if (siteDataFolder.dirExists())
                    files.add(siteDataFolder.files(true, "*.cshtml"));
            }
            return files;
        }
        public static Dictionary<string, string> SetAvailableScripts()
        {
            var files = TBotScriptsFiles();
            var mappings = new Dictionary<string, string>();
            foreach (var file in files)
                mappings.add(file.fileName_WithoutExtension(), file);            

            AvailableScripts = mappings;
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
