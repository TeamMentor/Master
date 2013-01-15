using System;
using O2.DotNetWrappers.ExtensionMethods;
using PostSharp.Aspects;

namespace TeamMentor.CoreLib
{
	[Serializable]
	public sealed class LogUrlAttribute : OnMethodBoundaryAspect
	{
		public string Category { get; set; }

		public LogUrlAttribute(string category)
		{
			Category = category;
		}
		
		public override void OnEntry(MethodExecutionArgs args)
		{										
			var url = HttpContextFactory.Request.Url;
			var title = "[TM_Log] : {0}".format(Category);
			title.ga_LogEntry(url.str());			

			base.OnEntry(args);
		}
	}
	[Serializable]
	public sealed class LogAttribute : OnMethodBoundaryAspect
	{
		public string Category { get; set; }

		public LogAttribute(string category)
		{
			Category = category;
		}

		public override void OnEntry(MethodExecutionArgs args)
		{ 			
			"[TM_Log]".ga_LogEntry(Category);			

			base.OnEntry(args);
		}
	}


	public static class PostSharp_ExtensionMethods
	{
		public static bool has_Arguments(this MethodInterceptionArgs args)
		{
			return args.Arguments.empty().isFalse();
		}

		public static object first_Argument(this MethodInterceptionArgs args)
		{
			//return args.Arguments.first();  //returns null;
			if (args.has_Arguments())
				return args.Arguments[0];
			return null;
		}

		public static T argument<T>(this MethodInterceptionArgs args)
		{
			var firstArgument = args.first_Argument();
			if (firstArgument is T)
				return (T) firstArgument;
			return default(T);
		}
	}

}
