using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FluentSharp.CoreLib;

namespace TeamMentor.UnitTests.TM_Website
{
    public static class Extra_ExtensionMethods_Stream
    {
        public static string readToEnd(this Stream stream)
        {
            return stream.notNull() 
                        ? new StreamReader(stream).ReadToEnd()
                        : "";
        }
    }
}
