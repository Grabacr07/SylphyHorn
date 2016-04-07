<Query Kind="Program">
  <Namespace>System.Runtime.InteropServices</Namespace>
</Query>

void Main()
{
	// Test code for Bundle.tt

	const string relativePath = @"..\SylphyHorn.Bootstrapper.UI\bin\Release";
	var targets = Array.Empty<FileInfo>();
	var currentDir = new DirectoryInfo(Path.GetDirectoryName(Host.ResolvePath("Bundle.tt")));
	var appRootDir = Path.GetFullPath(Path.Combine(currentDir.FullName, relativePath));
	if (currentDir.Exists)
	{
		var releaseDir = new DirectoryInfo(appRootDir);
		if (releaseDir.Exists)
		{
			targets = releaseDir.GetFiles("*", SearchOption.AllDirectories)
				.Where(x => x.Extension == ".dll")
				.Where(x => !x.Name.Contains("vshost"))
				.ToArray();
		}
	}

	

	foreach (var file in targets)
	{
		var sb = new StringBuilder(260);
		if (PathRelativePathTo(sb, appRootDir, FileAttributes.Directory, file.FullName, FileAttributes.Normal))
		{
			var path = sb.ToString(2, sb.Length - 2);
			path.Dump();
		}
	}
}

// Define other methods and classes here

public class Host
{
	public static string ResolvePath(string s)
	{
		return @"D:\Repos\@Grabacr07\SylphyHorn\source\Setup\SylphyHorn.Installer.Gen\Bundle.tt";
	}
}


[DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
private static extern bool PathRelativePathTo(
	 [Out] StringBuilder pszPath,
	 [In] string pszFrom,
	 [In] System.IO.FileAttributes dwAttrFrom,
	 [In] string pszTo,
	 [In] System.IO.FileAttributes dwAttrTo);
