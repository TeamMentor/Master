using FluentSharp.CoreLib;
using TeamMentor.CoreLib;

namespace FluentSharp
{
    [Admin]    
    public class O2_Script_Library
    {
        public static string ping()
        {
            return @"return ""pong"";";
        }

        public static string list()
        {
            var methodNames = typeof (O2_Script_Library).methods().names().@join(",");
            return @"return ""{0}"";".format(methodNames);
        }

        public static string openLshowLogViewer_if_LocalHostogViewer()
        {
            return  @"
return ""LogViewer"".popupWindow().add_LogViewer();
//O2Ref:FluentSharp.BCL.dll";    
        }

        public static string script_REPL()
        {
            return @"
HttpContext.Current.script_Me().waitForClose();
return ""done"";
//using System.Web;
//O2Ref:FluentSharp.BCL.dll
//O2Ref:FluentSharp.REPL.exe
//O2Ref:O2_Platform_External_SharpDevelop.dll";
        }

        public static string script_Database()
        {
            return @"
TM_Xml_Database.Current.script_Me().waitForClose();
return ""done"";
//using TeamMentor.CoreLib;
//O2Ref:TeamMentor.Corelib.dll
//O2Ref:FluentSharp.BCL.dll
//O2Ref:FluentSharp.REPL.exe
//O2Ref:O2_Platform_External_SharpDevelop.dll";
        }
        
        public static string load_NGit_Dlls()
        {
            return @"var a = typeof(Sharpen.URLEncoder);
var b = typeof(NGit.Repository);
return a.notNull() && b.notNull();

//O2Ref:NGit.dll
//O2Ref:Sharpen.dll";
        }
    }
}
