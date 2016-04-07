using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SylphyHorn.Bootstrapper
{
	public static class Launcher
	{
		private static FileInfo _target;
		private static readonly string _targetPath = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
			"grabacr.net",
			"SylphyHorn Engine",
			"SylphyHorn.exe");

		public static FileInfo Target => _target ?? (_target = new FileInfo(_targetPath));

		public static void Launch()
		{
			if (Target.Exists)
			{
				Process.Start(Target.FullName);
			}
		}
	}
}
