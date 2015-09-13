using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MetroTrilithon.Lifetime;
using MetroTrilithon.Mvvm;
using SylphyHorn.Models;

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

		public SettingsWindowViewModel()
		{
			this.Title = "Settings";

			Disposable.Create(Providers.Local.Save).AddTo(this);
		}
	}
}
