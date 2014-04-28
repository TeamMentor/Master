using System.Text;

namespace FluentSharp.CoreLib
{
    public static class Extra_StringBuilder_ExtensionMethods
    {
        public static StringBuilder appendLine(this StringBuilder stringBuilder, string line)
        {
            if (stringBuilder.notNull() && line.notNull())
                stringBuilder.AppendLine(line);
            return stringBuilder;
        }
        public static StringBuilder appendLines(this StringBuilder stringBuilder, params string[] lines)
        {
            if (stringBuilder.notNull() && lines.notNull())
                foreach(var line in lines)
                    stringBuilder.appendLine(line);
            return stringBuilder;
        }
    }
}