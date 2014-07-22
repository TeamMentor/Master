using System;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.Cassini
{
    public static class IE_TeamMentor_ExtensionMethods_Content
    {
        public static IE_TeamMentor article(this IE_TeamMentor ieTeamMentor, TeamMentor_Article tmArticle)
        {
            return ieTeamMentor.article(tmArticle.Metadata.Id);
        }

        public static IE_TeamMentor article(this IE_TeamMentor ieTeamMentor, Guid id)
        {
            return ieTeamMentor.open("article/{0}".format(id));
        }
        public static IE_TeamMentor article_Html(this IE_TeamMentor ieTeamMentor, TeamMentor_Article tmArticle)
        {
            return (ieTeamMentor.notNull() && tmArticle.notNull()) 
                ? ieTeamMentor.article_Html(tmArticle.Metadata.Id)
                : ieTeamMentor;
        }

        public static IE_TeamMentor article_Html(this IE_TeamMentor ieTeamMentor, Guid id)
        {
            return ieTeamMentor.open("html/{0}".format(id));
        }

        public static IE_TeamMentor article_Raw(this IE_TeamMentor ieTeamMentor, TeamMentor_Article tmArticle)
        {
            return ieTeamMentor.article_Raw(tmArticle.Metadata.Id);
        }

        public static IE_TeamMentor article_Raw(this IE_TeamMentor ieTeamMentor, Guid id)
        {
            return ieTeamMentor.open("raw/{0}".format(id));
        }
        
    }
}