using System;

namespace FluentSharp.CoreLib
{
    public static class Extra_Double_ExtensionMethods
    {
        public static bool isDouble(this string value)
        {
            double dummyDouble;
            return Double.TryParse(value, out dummyDouble);
        }
    
    }
}