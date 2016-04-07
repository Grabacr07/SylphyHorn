<Query Kind="Program" />

void Main()
{
	// Test code for Setup.tt

	const string application = "SylphyHorn.exe";
	const string relativePath = @"..\..\SylphyHorn\bin\Release";
	var targets = Array.Empty<FileInfo>();
	var currentDir = new DirectoryInfo(Path.GetDirectoryName(Host.ResolvePath("Product.tt")));
	if (currentDir.Exists)
	{
		var releaseDir = new DirectoryInfo(Path.GetFullPath(Path.Combine(currentDir.FullName, relativePath)));
		if (releaseDir.Exists)
		{
			targets = releaseDir.GetFiles("*", SearchOption.AllDirectories)
				.Where(x => x.Extension == ".exe" || x.Extension == ".dll")
				.Where(x => !x.Name.Contains("vshost"))
				.ToArray();
		}
	}

	foreach (var file in targets)
	{
		if (string.Equals(file.Name, application, StringComparison.InvariantCultureIgnoreCase))
		{
		}
		else
		{
		}
	}
	
	targets.Dump();
}

// Define other methods and classes here

public class Host
{
	public static string ResolvePath(string s)
	{
		return @"E:\Repos\@Grabacr07\SylphyHorn\source\Setup\SylphyHorn.Installer.Gen\Product.tt";
	}
}
