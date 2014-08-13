using FluentSharp.CoreLib;
using FluentSharp.Git.APIs;
using TeamMentor.FileStorage;

namespace TeamMentor.CoreLib
{
    public class TM_UserData_Git 
    {
        public static TM_UserData_Git Current;

        public TM_FileStorage FileStorage   { get; set; }
        public TM_UserData    UserData      { get; set; }
        public string NGit_Author_Name      { get; set; }
        public string NGit_Author_Email     { get; set; }
  
        public API_NGit NGit                { get; set; }

        public TM_UserData_Git(TM_FileStorage tmFileStorage)
        {            
            Current             = this;
            FileStorage         = tmFileStorage;
            UserData            = tmFileStorage.userData();
            NGit_Author_Name    = TMConsts.NGIT_DEFAULT_AUTHOR_NAME;
            NGit_Author_Email   = TMConsts.NGIT_DEFAULT_AUTHOR_EMAIL;                   
        }
        
    }
}
