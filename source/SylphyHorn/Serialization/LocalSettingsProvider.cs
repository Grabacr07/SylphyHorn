using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml;

namespace SylphyHorn.Serialization
{
	public sealed class LocalSettingsProvider : DictionaryProvider, IDisposable
	{
		public static TimeSpan FileSystemHandlerThrottleDueTime { get; set; } = TimeSpan.FromMilliseconds(1500);

		public static LocalSettingsProvider Instance { get; } = new LocalSettingsProvider();

		private readonly FileSystemWatcher _watcher;
		private readonly FileInfo _targetFile;
		private readonly Subject<FileSystemEventArgs> _notifier;

		public bool Available { get; }

		public string FilePath => this._targetFile.FullName;

		public IObservable<WatcherChangeTypes> FileChanged => this._notifier.Select(x => x.ChangeType);

		private LocalSettingsProvider()
		{
			var path = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				"Packages",
				"46846grabacr.net.SylphyHorn_vwznf8jfphrrc",
				"LocalState",
				this.Filename);

			var file = new FileInfo(path);
			if (file.Directory == null || file.DirectoryName == null)
			{
				this.Available = false;
				return;
			}

			if (!file.Directory.Exists)
			{
				file.Directory.Create();
			}

			try
			{
				this._notifier = new Subject<FileSystemEventArgs>();
				this._notifier.Throttle(FileSystemHandlerThrottleDueTime)
					.SelectMany(_ => this.LoadAsync().ToObservable())
					.Subscribe(_ => this.OnReloaded());

				this._targetFile = file;
				this._watcher = new FileSystemWatcher(file.DirectoryName, file.Name)
				{
					NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite,
				};
				this._watcher.Changed += this.HandleFileChanged;
				this._watcher.Created += this.HandleFileChanged;
				this._watcher.Deleted += this.HandleFileChanged;
				this._watcher.Renamed += this.HandleFileChanged;
				this._watcher.EnableRaisingEvents = true;

				this.Available = true;
			}
			catch (Exception)
			{
				this.Available = false;
			}
		}


		protected override Task SaveAsyncCore(IDictionary<string, object> dic)
		{
			if (!this.Available || !this._targetFile.Exists)
			{
				return Task.FromResult<IDictionary<string, object>>(null);
			}

			return Task.Run(() =>
			{
				var serializer = new DataContractSerializer(dic.GetType(), this.KnownTypes);
				var settings = new XmlWriterSettings
				{
					Indent = true, // more readable!!!
				};

				using (var stream = this._targetFile.OpenWrite())
				using (var writer = XmlWriter.Create(stream, settings))
				{
					serializer.WriteObject(writer, dic);
				}
			});
		}

		protected override Task<IDictionary<string, object>> LoadAsyncCore()
		{
			if (!this.Available || !this._targetFile.Exists)
			{
				return Task.FromResult<IDictionary<string, object>>(null);
			}

			return Task.Run(() =>
			{
				if (!this._targetFile.Exists) return null;

				var serializer = new DataContractSerializer(typeof(IDictionary<string, object>), this.KnownTypes);

				using (var stream = this._targetFile.OpenRead())
				{
					return serializer.ReadObject(stream) as IDictionary<string, object>;
				}
			});
		}

		private void HandleFileChanged(object sender, FileSystemEventArgs args)
		{
			this._notifier.OnNext(args);
		}

		public void Dispose()
		{
			if (this._watcher != null)
			{
				this._watcher.EnableRaisingEvents = false;
				this._watcher.Changed -= this.HandleFileChanged;
				this._watcher.Created -= this.HandleFileChanged;
				this._watcher.Deleted -= this.HandleFileChanged;
				this._watcher.Renamed -= this.HandleFileChanged;
				this._watcher.Dispose();
			}
		}
	}
}
