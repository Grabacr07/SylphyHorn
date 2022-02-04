using System;
using System.IO;

namespace SylphyHorn.Properties
{
	public static class Directories
	{
		private static readonly Lazy<DirectoryInfo> _localAppDataLazy = new Lazy<DirectoryInfo>(
			() => new DirectoryInfo(
				Path.Combine(
					Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
#if APPX
					@"Packages\46846grabacr.net.SylphyHorn_vwznf8jfphrrc\LocalCache\Local",
#endif
					ProductInfo.Company,
					ProductInfo.Product)
			));

		internal static DirectoryInfo LocalAppData => _localAppDataLazy.Value;
	}
}
