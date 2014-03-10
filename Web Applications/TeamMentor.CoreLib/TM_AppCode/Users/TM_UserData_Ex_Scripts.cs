using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public static class TM_UserData_Ex_Scripts
    {
        public static string firstScript_Invoke(this TM_UserData userData)
        {
            var scriptFile = userData.firstScript_FileLocation();
            if (scriptFile.fileExists())                
            {
                var assembly = scriptFile.fileContents().compileCodeSnippet();
                if (assembly.isNull())
                    "[secretDataScript_Invoke] couldn't compile script file: {0}".format(scriptFile);
                else
                {
                    var result = assembly.firstMethod().invoke().str();
                    "[secretDataScript_Invoke] execution result: {0}".info(result);
                    return result;
                }
            }
            return null;            
        }
        public static string firstScript_FileLocation(this TM_UserData userData)
        {
            return userData.Path_UserData.pathCombine(userData.FirstScriptToInvoke);
        }

        
    }
}
