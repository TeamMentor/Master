using System;
using System.Collections.Generic;
using FluentSharp.CoreLib;
using TeamMentor.Markdown;

namespace TeamMentor.CoreLib
{
    public static class TM_Xml_Database_ExtensionMethods_Articles
    {
        //move to  extension methods
        [ReadArticles]
        public static TeamMentor_Article getGuidanceItem(this TM_Xml_Database tmDatabase, Guid guidanceItemId)
        {
            UserRole.ReadArticles.demand();
            if (tmDatabase.Cached_GuidanceItems.hasKey(guidanceItemId).isFalse())
                return null;

            return tmDatabase.Cached_GuidanceItems[guidanceItemId];
        }

        [ReadArticles]
        public static string getGuidanceItemHtml(this TM_Xml_Database tmDatabase, Guid sessionId, Guid guidanceItemId)
        {
            UserRole.ReadArticles.demand();
            if (tmDatabase.Cached_GuidanceItems.hasKey(guidanceItemId).isFalse())
                return null;

            var article = tmDatabase.Cached_GuidanceItems[guidanceItemId];


            sessionId.session_TmUser().logUserActivity("View Article Html", article.Metadata.Title);

            var articleContent = article.Content.Data.Value;

            if (articleContent.inValid())
                return "";

            switch (article.Content.DataType.lower())
            {
                case "raw":
                    return articleContent;
                case "html":
                    {
                        if (TMConfig.Current.TMSecurity.Sanitize_HtmlContent && article.Content.Sanitized.isFalse())
                            return articleContent.sanitizeHtmlContent();

                        return articleContent.fixXmlDoubleEncodingIssue();
                    }
                case "safehtml":
                    {
                        return articleContent.sanitizeHtmlContent();
                    }
                case "wikitext":
                    return articleContent.wikiText_Transform();
                    //return "<div id ='tm_datatype_wikitext'>{0}</div>".format(articleContent.htmlEncode());
                case "markdown":
                    return articleContent.markdown_Transform();
                default:
                    return articleContent;
            }
        }

        [ReadArticles]
        public static List<string> getGuidanceItemsHtml(this TM_Xml_Database tmDatabase, Guid sessionId, List<Guid> guidanceItemsIds)
        {
            UserRole.ReadArticles.demand();
            var data = new List<string>();
            if (guidanceItemsIds.notNull())
                foreach (var guidanceItemId in guidanceItemsIds)
                {
                    data.add(tmDatabase.getGuidanceItemHtml(sessionId, guidanceItemId));
                }
            return data;
        }
    }
}