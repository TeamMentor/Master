using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamMentor.CoreLib
{    
    //demand clues
    public class AdminAttribute              : Attribute { }
    public class ManageUsersAttribute        : Attribute { }
    public class EditArticlesAttribute       : Attribute { }    
    public class ReadArticlesAttribute       : Attribute { }
    public class ReadArticlesTitlesAttribute : Attribute { }    
    public class ViewLibraryAttribute        : Attribute { }
    public class NoneAttribute               : Attribute { }

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
