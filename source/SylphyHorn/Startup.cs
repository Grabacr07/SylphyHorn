using System;
using System.IO;
using System.Reflection;
using MetroTrilithon.Desktop;
using MetroTrilithon.Threading.Tasks;
using SylphyHorn.Interop;

namespace SylphyHorn
{
	public class Startup
	{
		private readonly string _path;

		public bool IsExists => File.Exists(this._path);

		public Startup()
			: this(GetExecutingAssemblyFileNameWithoutExtension())
		{
		}

		public Startup(string name)
		{
			this._path = GetStartupFilePath(name);
		}

		public void Create()
		{
			ShellLink.Create(this._path);
		}

		public void Remove()
		{
			if (this.IsExists)
			{
				File.Delete(this._path);
			}
		}

		private static string GetStartupFilePath(string name)
		{
			var dir = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
			return Path.Combine(dir, name + ".lnk");
		}

		private static string GetExecutingAssemblyFileNameWithoutExtension()
		{
			return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);
		}
	}
}
