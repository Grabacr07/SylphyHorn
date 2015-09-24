using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;

namespace SylphyHorn.Services
{
	public class EngineDownloader
	{
		public static string PackageUri { get; } = @"https://github.com/Grabacr07/SylphyHorn/releases/download/v2.0/SylphyHornEngineInstall.exe";

		public async void Launch()
		{
			try
			{
				var uri = new Uri(PackageUri);
				var succeed = await Launcher.LaunchUriAsync(uri);
				if (succeed)
				{

				}
			}
			catch (Exception ex)
			{
				// ToDo: error action
				System.Diagnostics.Debug.WriteLine(ex);
			}
		}
	}
}
