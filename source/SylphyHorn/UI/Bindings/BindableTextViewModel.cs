using System;
using Livet;

namespace SylphyHorn.UI.Bindings
{
	public class BindableTextViewModel : ViewModel
	{
		#region Text 変更通知プロパティ

		private string _Text;

		public string Text
		{
			get => this._Text;
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
			get => this._Uri;
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
