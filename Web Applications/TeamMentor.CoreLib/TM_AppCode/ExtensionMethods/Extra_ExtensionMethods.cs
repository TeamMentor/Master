using System;
using System.IO;
using O2.DotNetWrappers.ExtensionMethods;

namespace O2.FluentSharp
{
	public static class Extra_ExtensionMethods
	{
		public static bool canNotWriteToPath(this string path)
		{
			return path.canWriteToPath().isFalse();
		}

		public static bool canWriteToPath(this string path)
		{
			try
			{
				//var files = path.files();
				var tempFile = path.pathCombine("tempFile".add_RandomLetters());
				"test content".saveAs(tempFile);
				if (tempFile.fileExists())
				{
					File.Delete(tempFile);
					if (tempFile.fileExists().isFalse())
						return true;
				}
				"[in canWriteToPath] test failed for for path: {0}".error(path);
			}
			catch (Exception ex)
			{
				ex.log("[in canWriteToPath] for path: {0} : {1}", path, ex.Message);
			}
			return false;
		}
	}
}
