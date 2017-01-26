﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Livet;
using Livet.Messaging.IO;
using MetroTrilithon.Lifetime;
using MetroTrilithon.Mvvm;
using SylphyHorn.Properties;
using SylphyHorn.Serialization;
using SylphyHorn.Services;

namespace SylphyHorn.UI.Bindings
{
	public class SettingsWindowViewModel : WindowViewModel
	{
		private static bool _restartRequired;
		private static readonly string _defaultCulture = Settings.General.Culture;

		private readonly HookService _hookService;
		private readonly Startup _startup;

		public IReadOnlyCollection<DisplayViewModel<string>> Cultures { get; }

        public IReadOnlyCollection<DisplayViewModel<string>> Themes { get; }

        public IReadOnlyCollection<DisplayViewModel<string>> Accents { get; }

		public IReadOnlyCollection<BindableTextViewModel> Libraries { get; }

		public bool RestartRequired => _restartRequired;

		#region HasStartupLink notification property

		private bool _HasStartupLink;

		public bool HasStartupLink
		{
			get { return this._HasStartupLink; }
			set
			{
				if (this._HasStartupLink != value)
				{
					if (value)
					{
						this._startup.Create();
					}
					else
					{
						this._startup.Remove();
					}

					this._HasStartupLink = this._startup.IsExists;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Culture notification property

		public string Culture
		{
			get { return Settings.General.Culture; }
			set
			{
				if (Settings.General.Culture != value)
				{
					Settings.General.Culture.Value = value;
					_restartRequired = value != _defaultCulture;

					this.RaisePropertyChanged();
					this.RaisePropertyChanged(nameof(this.RestartRequired));
				}
			}
		}

        #endregion

        #region Colors notification properties

        public string Theme
        {
            get { return Settings.General.Theme; }
            set
            {
                if (Settings.General.Theme != value)
                {
                    Settings.General.Theme.Value = value;
                    //_restartRequired = value != _defaultCulture;

                    this.RaisePropertyChanged();
                    //this.RaisePropertyChanged(nameof(this.RestartRequired));
                }
            }
        }

        public string Accent
        {
            get { return Settings.General.Accent; }
            set
            {
                if (Settings.General.Accent != value)
                {
                    Settings.General.Accent.Value = value;
                    //_restartRequired = value != _defaultCulture;

                    this.RaisePropertyChanged();
                    //this.RaisePropertyChanged(nameof(this.RestartRequired));
                }
            }
        }

        #endregion

        #region Backgrounds notification property

        private WallpaperFile[] _Backgrounds;

		public WallpaperFile[] Backgrounds
		{
			get { return this._Backgrounds; }
			set
			{
				if (this._Backgrounds != value)
				{
					this._Backgrounds = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public SettingsWindowViewModel(HookService hookService)
		{
			this._hookService = hookService;
			this._startup = new Startup();

			this.Cultures = new[] { new DisplayViewModel<string> { Display = "(auto)", } }
				.Concat(ResourceService.Current.SupportedCultures
					.Select(x => new DisplayViewModel<string> { Display = x.NativeName, Value = x.Name, })
					.OrderBy(x => x.Display))
				.ToList();

		    this.Themes = new DisplayViewModel<string>[] { }
		        .Concat(ResourceService.Current.SupportedThemes
		            .Select(x => new DisplayViewModel<string> { Display = x.ToString(), Value = x.Specified?.ToString() })
		            .OrderBy(x => x.Display))
		        .ToList();

            this.Accents = new DisplayViewModel<string>[] { }
                .Concat(ResourceService.Current.SupportedAccents
                    .Select(x => new DisplayViewModel<string> { Display = x.ToString(), Value = x.Specified?.ToString() })
                    .OrderBy(x => x.Display))
                .ToList();

            this.Libraries = ProductInfo.Libraries.Aggregate(
				new List<BindableTextViewModel>(),
				(list, lib) =>
				{
					list.Add(new BindableTextViewModel { Text = list.Count == 0 ? "Build with " : ", ", });
					list.Add(new HyperlinkViewModel { Text = lib.Name.Replace(' ', Convert.ToChar(160)), Uri = lib.Url, });
					return list;
				},
				list =>
				{
					list.Add(new BindableTextViewModel() { Text = ".", });
					return list;
				});

			this._HasStartupLink = this._startup.IsExists;

			Settings.General.DesktopBackgroundFolderPath
				.Subscribe(path => this.Backgrounds = WallpaperService.Instance.GetWallpaperFiles(path))
				.AddTo(this);

			Disposable.Create(() => LocalSettingsProvider.Instance.SaveAsync().Wait())
				.AddTo(this);
		}

		protected override void InitializeCore()
		{
			base.InitializeCore();
			this._hookService.Suspend()
				.AddTo(this);
		}

		public void OpenBackgroundPathDialog()
		{
			var message = new FolderSelectionMessage("Window.OpenBackgroundImagesDialog.Open")
			{
				Title = Resources.Settings_Background_SelectionDialog,
				SelectedPath = Settings.General.DesktopBackgroundFolderPath,
				DialogPreference = FolderSelectionDialogPreference.CommonItemDialog,
			};
			this.Messenger.Raise(message);

			if (Directory.Exists(message.Response))
			{
				Settings.General.DesktopBackgroundFolderPath.Value = message.Response;
			}
		}
	}

	public class BindableTextViewModel : ViewModel
	{
		#region Text 変更通知プロパティ

		private string _Text;

		public string Text
		{
			get { return this._Text; }
			set
			{
				if (this._Text != value)
				{
					this._Text = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion
	}

	public class HyperlinkViewModel : BindableTextViewModel
	{
		#region Uri 変更通知プロパティ

		private Uri _Uri;

		public Uri Uri
		{
			get { return this._Uri; }
			set
			{
				if (this._Uri != value)
				{
					this._Uri = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion
	}
}
