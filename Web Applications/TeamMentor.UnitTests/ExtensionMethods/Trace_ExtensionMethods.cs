using System.Diagnostics;
using O2.DotNetWrappers.ExtensionMethods;
namespace O2.FluentSharp
{
	public static class Trace_ExtensionMethods
	{
		public static string writeLine_Trace(this string message, params object[] parameters)
		{
			return message.trace_WriteLine(parameters);
		}
		public static string trace_WriteLine(this string message, params object[] parameters)
		{
			if (parameters.empty())
				Trace.WriteLine(message);
			else
				Trace.WriteLine(message.format(parameters));
			return message;
		}
	}
}
