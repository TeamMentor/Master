using System;
using FluentSharp.CoreLib;
using FluentSharp.Git;
using FluentSharp.Git.APIs;

namespace TeamMentor.CoreLib
{
    public static class TM_NGit_Ex
    {
        public static TMUser setGitUser(this TMUser currentUser)             
        {
            if(currentUser.notNull())
            {
                if (TM_UserData_Git.Current.notNull())
                {
                    TM_UserData_Git.Current.NGit_Author_Name = currentUser.UserName;
                    TM_UserData_Git.Current.NGit_Author_Email = currentUser.EMail;
                }
            }
            return currentUser;
        }
        public static API_NGit setDefaultAuthor(this API_NGit nGit)
        {
            if (TM_UserData_Git.Current.notNull())
                try
                {
                    var userData    = TM_UserData_Git.Current;
                    var name        = userData.NGit_Author_Name.valid() ? userData.NGit_Author_Name : "tm-bot";
                    var email       = userData.NGit_Author_Email.valid() ? userData.NGit_Author_Email : "tm-bot@teammentor.net";
                    nGit.Author     = name.personIdent(email);
                    nGit.Committer  = "tm-bot ".personIdent("tm-bot@teammentor.net");
                }
                catch(Exception ex)
                {
                    ex.log();
                }
            return nGit;
        }
    }
}