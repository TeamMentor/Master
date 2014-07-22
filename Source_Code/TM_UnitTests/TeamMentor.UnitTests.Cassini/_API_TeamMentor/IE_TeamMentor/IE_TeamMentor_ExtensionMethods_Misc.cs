using FluentSharp.Watin;

namespace TeamMentor.UnitTests.Cassini
{
    public static class IE_TeamMentor_ExtensionMethods_Misc
    {
        //ie helpers
        public static IE_TeamMentor refresh(this IE_TeamMentor ieTeamMentor)
        {
            ieTeamMentor.ie.refresh();
            return ieTeamMentor;
        }

        public static string html(this IE_TeamMentor ieTeamMentor)
        {
            return ieTeamMentor.ie.html();            
        }

    }
}