namespace TeamMentor.CoreLib
{
	public static class MarkdownDeep_ExtensionMethods
	{
        public static bool SafeMode { get; set; } //true;
        public static bool ExtraMode { get; set; } //true;

        static MarkdownDeep_ExtensionMethods()
        {
            SafeMode  = true;           // hard-coded to true for now
            ExtraMode = true;
        }
		public static string markdown_Transform(this string markdown)
		{						
		    var md = new MarkdownDeep.Markdown
		                 {
                             SafeMode       = SafeMode,       // was false in the MarkdownDeep demo app
                             ExtraMode      = ExtraMode,          // was true, was creating some probs with HTML conversion (too agreesive on spaces)
                             AutoHeadingIDs = true, 
                             MarkdownInHtml = true, 
                             NewWindowForExternalLinks = true,
                             
		                 };
            return md.Transform(markdown);            
		}
	}
}
