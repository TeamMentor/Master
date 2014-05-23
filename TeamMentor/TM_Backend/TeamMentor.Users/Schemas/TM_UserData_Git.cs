using FluentSharp.CoreLib;
using FluentSharp.Git.APIs;

namespace TeamMentor.CoreLib
{
    public class TM_UserData_Git : TM_UserData
    {
        public string NGit_Author_Name { get; set; }
        public string NGit_Author_Email { get; set; }
  
        public API_NGit NGit { get; set; }

        public TM_UserData_Git() : this(false)
        {
            
        }
        public TM_UserData_Git(bool UsingFileStorage) : base(UsingFileStorage)
        {
            NGit_Author_Name = TMConsts.NGIT_DEFAULT_AUTHOR_NAME;
            NGit_Author_Email = TMConsts.NGIT_DEFAULT_AUTHOR_EMAIL;                
        }
        
    }
}
