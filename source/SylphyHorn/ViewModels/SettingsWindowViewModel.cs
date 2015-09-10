using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;
using MetroTrilithon.Lifetime;
using MetroTrilithon.Mvvm;
using SylphyHorn.Models;

namespace SylphyHorn.ViewModels
{
	public class SettingsWindowViewModel : WindowViewModel
	{
		public SettingsWindowViewModel()
		{
			this.Title = "Settings";

			Disposable.Create(Providers.Local.Save).AddTo(this);
		}
	}
}
