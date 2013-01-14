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
			GoogleAnalytics.Current.LogEntry("WebService", service);
		}

		public override void OnExit(MethodExecutionArgs args)
		{
			//var message = "< Leaving {0}.{1}.".format(args.Method.DeclaringType.Name, args.Method.Name);						
			//"[{0}] {1}".info(Category, message);
		}
	}
}
