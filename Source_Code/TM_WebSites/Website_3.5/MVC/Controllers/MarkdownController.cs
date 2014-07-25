using System.Collections.Generic;
using System.Web.Mvc;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;

namespace TeamMentor.Website
{
    public class MarkdownController : Controller
    {        
        public static TM_Xml_Database   tmDatabase;

        static MarkdownController()
        {             
             tmDatabase = TM_Xml_Database.Current;
        }

        public TeamMentor_Article getArticle(string articleId)
        {
            var article = tmDatabase.tmGuidanceItem(articleId.guid());            
            return article;
        }

        //view article
        public ActionResult Viewer(string articleId)
        {
            if (UserRole.ReadArticles.currentUserHasRole().isFalse())
                return View(@"~/MVC/Views/Loggin_Needed.cshtml");
            var article = getArticle(articleId);
            if (article.notNull())
            { 
                var markdownToRender = article.Content.Data_Json;
                ViewData["Content"] = markdownToRender.markdown_Transform();
                ViewData["ArticleId"] = articleId;            
            }
            return View(@"~/MVC/Views/MarkDown_Viewer.cshtml");
        }

        //show article editor        
        public ActionResult Editor(string articleId)
        {
            if (UserRole.EditArticles.currentUserHasRole().isFalse())
                return View(@"~/MVC/Views/Loggin_Needed.cshtml");

            var article = getArticle(articleId);
            if (article.notNull())
            {
                
                ViewData["Content"           ] = article.Content.Data_Json   ?? "";
                ViewData["Article_Title"     ] = article.Metadata.Title      ?? "";
                ViewData["Article_Technology"] = article.Metadata.Technology ?? "";
                ViewData["Article_Phase"     ] = article.Metadata.Phase      ?? "";
                ViewData["Article_Type"      ] = article.Metadata.Type       ?? "";
                ViewData["Article_Category"  ] = article.Metadata.Category   ?? "";
                ViewData["DirectLink"        ] = article.Metadata.DirectLink ?? "";                
                ViewData["Article_DataType"  ] = article.Content.DataType    ?? "";
            }
            else
            { 
                ViewData["Content"]            = "NO ARTICLE With GUID: {0}".format(articleId);
                ViewData["Article_DataType"  ] = "";
            }
            ViewData["ArticleId"] = articleId;
     
            return View(@"~/MVC/Views/MarkDown_Editor.cshtml");
        }

        //receive article content and update article
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)] 
        public ActionResult SaveContent(string articleId, string content, string category, string type, string phase, string technology, string title, string dataType, string directLink)
        {
            var article = getArticle(articleId);
            article.Content.Data.Value = content;
            article.Content.DataType   = dataType ?? article.Content.DataType;
            article.Metadata.Title = title ?? article.Metadata.Title;
            article.Metadata.Category = category ?? article.Metadata.Category;
            article.Metadata.Type = type ?? article.Metadata.Type;
            article.Metadata.Phase = phase ?? article.Metadata.Phase;
            article.Metadata.Technology = technology ?? article.Metadata.Technology;            
            article.Metadata.DirectLink   = directLink ?? article.Metadata.DirectLink;
            article.xmlDB_Save_Article(tmDatabase);

            TM_WebServices.guiObjectsCacheOk = false;

            if(TM_Xml_Database_Git.Current_Git.notNull())
                TM_Xml_Database_Git.Current_Git.triggerGitCommit();

            return Redirect("/article/{0}".format(articleId));

            //return Redirect("Viewer?articleId={0}".format(articleId));
        }
            
    }
}