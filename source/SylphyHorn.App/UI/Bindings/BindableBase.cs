using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MetroTrilithon.Mvvm;

namespace SylphyHorn.UI.Bindings
{
	public class BindableBase : Notifier
	{
		protected void SetProperty<T>(ref T property, T value, [CallerMemberName]string propertyName = null)
		{
			if (property.Equals(value)) return;

			property = value;
			this.RaisePropertyChanged(propertyName);
		}
	}
}
