using System;
using System.IO;
using System.Reflection;
using MetroTrilithon.Desktop;
using MetroTrilithon.Threading.Tasks;
using SylphyHorn.Interop;
using Windows.ApplicationModel;

namespace SylphyHorn
{
	public class Startup
	{
		private readonly string _path;
		private readonly StartupTask _task;

		public bool IsExists => Platform.IsUwp
			? this._task.State == StartupTaskState.Enabled || this._task.State == StartupTaskState.DisabledByUser
			: File.Exists(this._path);

		public Startup()
			: this(GetExecutingAssemblyFileNameWithoutExtension())
		{
		}

		public Startup(string name)
		{
			if (Platform.IsUwp)
			{
				this._task = GetStartupTask();
			}
			else
			{
				this._path = GetStartupFilePath(name);
			}
		}

		public void Create()
		{
			if (Platform.IsUwp)
			{
				if (this._task.State == StartupTaskState.Disabled)
				{
					this._task.RequestEnableAsync().ToTask().Forget();
				}
			}
			else
			{
				ShellLink.Create(this._path);
			}
		}

		public void Remove()
		{
			if (Platform.IsUwp)
			{
				if (this._task.State == StartupTaskState.Enabled || this._task.State == StartupTaskState.DisabledByUser)
				{
					this._task.Disable();
				}
			}
			else
			{
				if (this.IsExists)
				{
					File.Delete(this._path);
				}
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

		private static StartupTask GetStartupTask()
		{
			return StartupTask.GetAsync("SylphyHornEngineStartupTask").ToTask().Result;
		}
	}
}
