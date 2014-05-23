namespace FluentSharp.CoreLib
{
    public static class Extra_String_ExtensionMethods
    {
        public static string random_Email(this string emailName)
        {
            if (emailName.notValid())
                emailName = 10.randomLetters();
            return "{0}@{1}.com".format(emailName, 10.randomLetters());
        }


    }
}