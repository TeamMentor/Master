using System;
using System.Security.Permissions;
using System.Threading;
using System.Web.Script.Serialization;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
	public class REPL
	{
		//public static List<Thread> ExecutionThreads = new List<Thread>();		
		public static int MAX_EXECUTION_TIME = 20000;
		
		[Admin]	                    
		public static string executeSnippet(string snippet)
		{
			"[REPL] executing snippet with size: {0}".info(snippet.size());
			object executionResult = "";
			var compileError = "";
			Action<string> onCompileOk = (msg) => { };
			Action<string> onCompileFail = (msg) => { compileError = msg; };
			var result = snippet.fixCRLF().compileAndExecuteCodeSnippet(onCompileOk, onCompileFail);
			if (compileError.valid())
				executionResult = compileError;
			else
				executionResult = result.notNull() ? result : "";

			if (executionResult is string)
				return (string)executionResult;
			try
			{
				return new JavaScriptSerializer().Serialize(executionResult);
			}
			catch { }
						
			return executionResult.str();			
		}

		public static string executeSnippet_SeparateThread(string snippet)
		{			
			var executionResult = "";			
			var sync = new AutoResetEvent(false);
			var thread = O2Thread.mtaThread(
				()=>{
					    executionResult = executeSnippet(snippet);
					    sync.Set();
				});

			if (thread.Join(MAX_EXECUTION_TIME).isFalse())
			{
				"[REPL] Execution timeout reached".error();
				return "Error: Snippet execution timed out";
			}
			return executionResult;
		}
	}
}