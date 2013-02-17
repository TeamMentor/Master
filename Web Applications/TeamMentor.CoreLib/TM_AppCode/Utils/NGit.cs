using O2.FluentSharp;

namespace TeamMentor.CoreLib.TM_AppCode.Utils
{
    public class NGit
    {
        public bool isRepository(string path)
        {
            return path.isGitRepository();
        }
    }
}
