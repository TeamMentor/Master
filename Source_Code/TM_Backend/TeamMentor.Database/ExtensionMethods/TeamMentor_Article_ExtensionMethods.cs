using FluentSharp.CoreLib;
using Microsoft.Security.Application;

namespace TeamMentor.CoreLib
{
    public static class TeamMentor_Article_ExtensionMethods
    {
        public static TeamMentor_Article setHashes(this TeamMentor_Article article)
        { 
            article.Metadata_Hash = article.Metadata.serialize(false).hash();
            article.Content_Hash  = article.Content.serialize(false).hash();
            return article;
        }


/*        public static guidanceItem transform_into_guidanceItem(this TeamMentor_Article article)
        {
            if (article.isNull())
                return null;
            return new guidanceItem()
                    {
                        id          =   article.Metadata.Id.str(),
                        id_original =   article.Metadata.Id_History,
                        title       =   article.Metadata.Title,

                        Technology  =   article.Metadata.Technology,
                        phase       =   article.Metadata.Phase,
                        Category    =   article.Metadata.Category,
                        Rule_Type   =   article.Metadata.Type,

                        content     =   article.Content.Data.Value
                    };
        }*/

        public static TeamMentor_Article teamMentor_Article(this string pathToXmlFile)
        { 
            var article = pathToXmlFile.load<TeamMentor_Article>().htmlEncode(); 
            return article;
        }

        //this causes  double encoding problems with some properties (like the Title on Html Editor) , but removing it opens up more XSS on other viewers (like the Table)
        public static TeamMentor_Article htmlEncode(this TeamMentor_Article article)
        {
            if (article.isNull())
                return null;
            var metaData = article.Metadata;
            foreach(var prop in metaData.type().properties())
            {
                if (prop.PropertyType == typeof(string))
                {
                    var value = (string)metaData.prop(prop.Name);
                    metaData.prop(prop.Name, Encoder.HtmlEncode(value)); 
                }
            }
            if (TMConfig.Current.TMSecurity.Sanitize_HtmlContent)
            {
                article.Content.Data.Value = Sanitizer.GetSafeHtmlFragment(article.Content.Data.Value);
                article.Content.Sanitized = true;
            }

            return article;
        }

        //fix double encoding caused by JSON?CDATA/XML transfer of XML data
        public static string fixXmlDoubleEncodingIssue(this string htmlContent)
        { 
            return htmlContent.replace("&amp;", "&"); 
        }
    }
}