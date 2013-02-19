using O2.FluentSharp;

namespace TeamMentor.CoreLib
{
    public class NGit
    {
        public bool isRepository(string path)
        {
            return path.isGitRepository();
        }
    }
}
