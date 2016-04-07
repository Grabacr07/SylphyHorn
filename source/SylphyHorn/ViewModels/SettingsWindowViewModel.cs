using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Livet;
using MetroTrilithon.Mvvm;
using SylphyHorn.Properties;
using SylphyHorn.Services;

namespace SylphyHorn.ViewModels
{
	public class SettingsWindowViewModel : WindowViewModel
	{
		#region HasStartupLink property

		private bool _HasStartupLink;

		public bool HasStartupLink
		{
			get { return this._HasStartupLink; }
			set
			{
				if (this._HasStartupLink != value)
				{
					this._HasStartupLink = value;

					if (value)
					{
						ShellLinkHelper.CreateStartup();
					}
					else
					{
						ShellLinkHelper.RemoveStartup();
					}

					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public IReadOnlyCollection<BindableTextViewModel> Libraries { get; }

		public SettingsWindowViewModel()
		{
			this.Title = "Settings";

			this.Libraries = ProductInfo.Libraries.Aggregate(
				new List<BindableTextViewModel>(),
				(list, lib) =>
				{
					list.Add(new BindableTextViewModel { Text = list.Count == 0 ? "Build with " : ", " });
					list.Add(new HyperlinkViewModel { Text = lib.Name.Replace(' ', Convert.ToChar(160)), Uri = lib.Url });
					return list;
				});
			this._HasStartupLink = ShellLinkHelper.ExistsStartup();

			// ToDo: 設定ダイアログがタコってるのであとで見直す
			//       基本的には設定ダイアログは提供しない、ものの、起動オプションで表示できるようにはしたい
			//       設定の保存は UWP 側でのみやる想定だけど、ダイアログには保存ボタンを置きたい
			//       スタートアップへの登録と解除も設定ダイアログで提供したい

			//Disposable.Create(Providers.Local.Save).AddTo(this);
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
