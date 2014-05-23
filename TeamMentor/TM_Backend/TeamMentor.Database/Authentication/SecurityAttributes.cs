using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamMentor.CoreLib
{
    //TODO: Add security properties that were being enforced by PostSharp
    //demands
    public class AdminAttribute : Attribute
    {
    }
    public class ReadArticlesAttribute : Attribute
    {
    }
    public class ReadArticlesTitlesAttribute : Attribute
    {
    }
    public class EditArticlesAttribute : Attribute
    {
    }
    public class ManageUsersAttribute : Attribute
    {
    }

    //asserts
    public class Assert_AdminAttribute : Attribute
    {
    }
    public class Assert_EditorAttribute : Attribute
    {
    }
    public class Assert_ReaderAttribute : Attribute
    {
    }
}
