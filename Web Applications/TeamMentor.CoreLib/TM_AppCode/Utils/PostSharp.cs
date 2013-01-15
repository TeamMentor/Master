using System;
using O2.DotNetWrappers.ExtensionMethods;
using PostSharp.Aspects;

namespace TeamMentor.CoreLib
{

	[Serializable]
	public sealed class TraceAttribute : OnMethodBoundaryAspect
	{
		private readonly string category;

		public TraceAttribute(string category)
		{
			this.category = category;
		}

		public string Category { get { return category; } }

		public override void OnEntry(MethodExecutionArgs args)
		{ 
			var message = "> Entering {0}.{1}.".format(args.Method.DeclaringType.Name, args.Method.Name);
			var url = HttpContextFactory.Request.Url;			
			"[{0}] {1} for: {2} ".info(Category, message, url);
			var service = url.str().split("/").last();
			"WebService".logActivity(service);
			//GoogleAnalytics.Current.LogEntry("WebService", );
		}

		public override void OnExit(MethodExecutionArgs args)
		{
			//var message = "< Leaving {0}.{1}.".format(args.Method.DeclaringType.Name, args.Method.Name);						
			//"[{0}] {1}".info(Category, message);
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
