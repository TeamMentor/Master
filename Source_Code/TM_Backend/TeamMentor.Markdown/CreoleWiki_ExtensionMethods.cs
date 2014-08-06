using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Algorim.CreoleWiki;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;

namespace TeamMentor.Markdown
{
    public static class CreoleWiki_ExtensionMethods
    {
        public static int MAX_RENDER_WAIT = 2000;
        public static string wikiText_Transform(this string wikiText)
		{		
			var html = "";	
            try
            {                
                wikiText = wikiText.wikiCreole_Fix_WikiText_Bullets();
                
                var wikiParseThread = O2Thread.mtaThread(()=>
                    {
                         html = new CreoleParser().Parse(wikiText)              
                                                  .wikiCreole_Replaced_Html_Code_Tag_with_Pre();                         
                    });
                if(wikiParseThread.Join(MAX_RENDER_WAIT).isFalse())
                {
                    "[CreoleWiki_ExtensionMethods][wikiText_Transform] failed for WikiText: \n\n{0}\n\n".error(wikiParseThread);
                    wikiParseThread.Abort();
                }
            }
            catch(Exception ex)
            {
                ex.log("[wikiText_transform]");                
            }
            return html;
		}

        /// <summary>
        /// This is a hack to handle five scenarious that WikiCreole works differently from jsCreole:       
        /// 
        /// The fix is done using simple String replaces, since there are enough RegEx issues with WikiCreole already
        /// </summary>
        /// <param name="wikiText"></param>
        /// <returns></returns>
        public static string wikiCreole_Fix_WikiText_Bullets(this string wikiText)
        {           
            var lines = wikiText.split("\n");

            for(var i = 0; i < lines.size() ; i++)                              
            {
                var line = lines[i];                    
                if (line.starts("*" ).isTrue () &&                      // case one: * is on the beggining of the line 
                    line.starts("**").isFalse() &&                      //             is not bold
                    line.starts("* ").isFalse())                        //             is not followed by a space
                { 
                    var fix = "* " + line.removeFirstChar();
                    wikiText = wikiText.replace(line, fix);
                    continue;
                }

                if (line.starts("    *" ).isTrue () &&                   // case two: * is after 4 spaces
                    line.starts("    **").isFalse() &&
                    line.starts("    * ").isFalse())                    
                { 
                    var fix = "    * " + line.subString_After("    *");
                    wikiText = wikiText.replace(line, fix);                    
                    continue;
                }
                if (line.starts("#"  ).isTrue () &&                      // case three: #* is on the beggining of the line                     
                    line.starts("# ").isFalse())                    
                { 
                    var fix = "# " + line.subString_After("#");
                    wikiText = wikiText.replace(line, fix);                    
                    continue;
                }
                if (line.starts("    #" ).isTrue () &&                   // case four: # is after 4 spaces                    
                    line.starts("    # ").isFalse())                    
                { 
                    var fix = "    # " + line.subString_After("    #");
                    wikiText = wikiText.replace(line, fix);                    
                    continue;
                }
                if (line.starts("#*"  ).isTrue () &&                     // case five: #* is on the beggining of the line 
                    line.starts("#**" ).isFalse() &&
                    line.starts("#* " ).isFalse())                    
                { 
                    var fix = "#* " + line.subString_After("    #*");
                    wikiText = wikiText.replace(line, fix);                    
                    continue;
                }
                
            }
            return wikiText;
            //return size2.join("\n");
            var joinedLines = lines.join("".line());
            var size = joinedLines.split_onLines().size();
          //  return wikiText;
            return joinedLines;
        }

        public static string wikiCreole_Replaced_Html_Code_Tag_with_Pre(this string htmlCode)
        {
            return htmlCode.replace("<code>","<pre>")
                           .replace("</code>","</pre>");
        }
    }
}
