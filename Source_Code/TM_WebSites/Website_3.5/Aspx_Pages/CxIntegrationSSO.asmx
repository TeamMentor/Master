<%@ WebService Language="C#" Class="CheckmarxService.CxTMArticlesResolver" %>
using System.Web;
using System.Web.Services;
using FluentSharp.CoreLib;
using FluentSharp.Web;
using TeamMentor.CoreLib;
using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using HtmlAgilityPack;
namespace CheckmarxService
{
    [WebService(Namespace = "http://teammentor.net")]
    public class CxTMArticlesResolver : WebService
    {
        readonly string _baseArticleUrl = string.Empty;
        readonly string _baseUrl = string.Empty;
        private readonly List<string> _dataMapping;
        private readonly CultureInfo _myCIintl;
        private readonly TM_Xml_Database _xmlDatabase;
        private readonly TM_UserData userData;
        private readonly TM_Authentication _authentication;
        private const string SsoUser = "Checkmarx_User";
        private const string  HtmlTemplateFile = @"\Html_Pages\Gui\Pages\article_Html.html";
        public HttpContextBase context = HttpContextFactory.Current;
        #region Enum
        //Checkmarx supported languages.
        enum SupportedLanguages
        {
            Unknown = 0,
            ASPNET = 1,
            Java = 2,
            CPP = 4,
            JavaScript = 8,
            Apex = 16,
            VbNet = 32,
            VbScript = 64,
            ASP = 128,
            VB6 = 256,
            PHP = 512,
            Ruby = 1024,
            Perl = 2048,
            Objc = 4096,
            PLSQL = 8192,
            Python = 16384
        };
        #endregion
        public CxTMArticlesResolver()
        {
            //Creating a culture info for English
            _myCIintl = new CultureInfo("en-US");

            var context = HttpContext.Current;
            if (context!=null)
            {
                var request = context.Request;
                if (request!=null)
                {
                    _baseUrl = request.Url.GetLeftPart(UriPartial.Authority);
                    _baseArticleUrl = _baseUrl + "/html/";
                }
                if (_dataMapping == null)
                {
                    var result = File.ReadAllLines(Server.MapPath("TM_Mappings_CWE.csv"));
                    if (result != null)
                    {
                        _dataMapping = result.ToList();
                        //Cleaning up headers
                        _dataMapping.RemoveAt(0);
                    }
                }
            }
             _xmlDatabase = TM_Xml_Database.Current;
             userData = TM_UserData.Current;
            _authentication = new TM_Authentication(null);
        }
        [WebMethod]
        public string GetHTMLDescription(int cwe_id, int dev_language_id, string query_name, string project_name, string logged_in_user_name, int localization_id, Guid authentication_token)
        {
            //Validating Token
            if (!authentication_token.notNull())
            {
                return "Authentication token is a required value.";
            }
            authentication_token = Guid.Parse(authentication_token.ToString().ToLower());
            //Language ID needs to be supported by Cx
            if (!Enum.IsDefined(typeof(SupportedLanguages), dev_language_id))
            {
                throw new Exception(String.Format("Language {0} not supported.", dev_language_id));
            }
            //LCIDS http://msdn.microsoft.com/nb-no/goglobal/bb964664.aspx, at this time just supporting English.
            if (_myCIintl.LCID.CompareTo(localization_id)!=0)
            {
                //At this time, if the localization request is different from English (1033) we do retun an empty string.
                return string.Empty;
            }

            var technology = Enum.Parse(typeof(SupportedLanguages), dev_language_id.ToString());
            var articleGuid = FindArticleUrl(cwe_id, technology.ToString());

            //There is not mapping article for the CWE and Language
            if (String.IsNullOrEmpty(articleGuid))
                return "";
          
            //Token verification
            if (AuthenticationTokenIsValid(authentication_token))
            {
                //Pulling session id
                var sessionId = SsoUser.tmUser().login();
                _authentication.sessionID = sessionId;
                
                var htmlResponse = _xmlDatabase.getGuidanceItemHtml(sessionId, articleGuid.guid());
                //Safe check, if the article does not exist, htmlResponse becomes null
                if (htmlResponse.notNull())
                {
                    var article = _xmlDatabase.getGuidanceItem(articleGuid.guid());
                    //Safe check, it could be the case that the article GUID does not exist.
                    if (article.notNull())
                    {
                        var htmlTemplate = context.Server.MapPath(HtmlTemplateFile).fileContents();
                        var htmlContent = htmlTemplate.replace("#ARTICLE_TITLE", article.Metadata.Title)
                                                      .replace("#ARTICLE_HTML", htmlResponse);
                        return HttpUtility.HtmlDecode(GetAbsoluteUrls(htmlContent));
                    }
                    else
                    {
                        String.Format("{0} not found!", articleGuid).error();    
                    }
                }
                return "";
            }
            return "Authentication token is not valid";
        }
        #region Private methods
        private  string FindArticleUrl(int cweId, string technology)
        {
            foreach (var row in _dataMapping)
            {
                var data = row.Split(',');
                if (data.Length > 0)
                {
                    var cwe = data[1].Trim();
                    var temptechnology = data[4].Trim();

                    if (cwe.Contains(String.Format("CWE ID {0}",cweId))  && temptechnology.Equals(technology))
                    {
                        return data[2].Trim();
                    }
                }
            }
            return "";
        }
        private bool  AuthenticationTokenIsValid (Guid token)
        {
            //Safe check for Secret data.
            if (userData.notNull() && userData.SecretData.notNull())
            {
                var ssoKey = userData.SecretData.Rijndael_Key;
                if (ssoKey.isNull())
                {
                    throw new ArgumentException("SSO Key not found. Please verify Secret data");
                }
                //Computing expected token.
                var expectedToken = (SsoUser.Trim() + ssoKey.Trim()).md5Hash();
                return expectedToken.guid().Equals(token);
            }
            return false;
        }
        /// <summary>
        /// This function gets the raw HTML returned from the service and using HtmlAgilityPack,return AbsoluteUris for images and hyperlinks.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private string GetAbsoluteUrls(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            
            //Finding images
            foreach (var img in doc.DocumentNode.Descendants("img"))
            {
                if (img != null)
                    img.Attributes["src"].Value = new Uri(new Uri(_baseUrl), 
                                                    img.Attributes["src"].Value).AbsoluteUri;
            }
            //Finding styles
            foreach (var link in doc.DocumentNode.Descendants("link"))
            {
                if (link.Attributes != null)
                    link.Attributes["href"].Value = new Uri(new Uri(_baseUrl), link.Attributes["href"].Value).AbsoluteUri;
            }
            //Finding hyperlinks
            foreach (var link in doc.DocumentNode.Descendants("a"))
            {
                //Ignoring Absolute Urls and mailto
                if (link != null && (!link.Attributes["href"].Value.Contains("http")
                                     && !link.Attributes["href"].Value.Contains("mailto")))
                {
                    //Article link needs to be a GUID
                    if (link.Attributes["href"].Value.isGuid()) 
                    {
                        link.Attributes["href"].Value = new Uri(new Uri(_baseUrl + "/html/"),
                                                        link.Attributes["href"].Value).AbsoluteUri;
                    }
                }
            }
            return doc.DocumentNode.InnerHtml;
        }
        #endregion
    }
}