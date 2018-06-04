using System;
using System.IO;

namespace SylphyHorn.Properties
{
	public static class Directories
	{
		private static readonly Lazy<DirectoryInfo> _localAppDataLazy = new Lazy<DirectoryInfo>(() => new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ProductInfo.Company, ProductInfo.Product)));

		internal static DirectoryInfo LocalAppData => _localAppDataLazy.Value;
	}
}
