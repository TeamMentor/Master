namespace TeamMentor.UnitTests.Cassini
{
    public static class IE_TeamMentor_ExtensionMethods_Pages
    {
        public static IE_TeamMentor page_Admin(this IE_TeamMentor ieTeamMentor)
        {
            return ieTeamMentor.open("/Admin");
        }
        public static IE_TeamMentor page_Home(this IE_TeamMentor ieTeamMentor)
        {
            return ieTeamMentor.open("/TeamMentor");
        }    
        public static IE_TeamMentor page_Login(this IE_TeamMentor ieTeamMentor)
        {
            return ieTeamMentor.open("/Login");
        }        
        public static IE_TeamMentor page_TBot(this IE_TeamMentor ieTeamMentor)
        {
            return ieTeamMentor.open("/Tbot");
        }        
    }
}