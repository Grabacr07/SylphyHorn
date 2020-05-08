using MetroTrilithon.Mvvm;
using SylphyHorn.Services;

namespace SylphyHorn.UI.Bindings
{
	public class RenameWindowViewModel : WindowViewModel
	{
		#region Number

		private int _Number;

		public int Number
		{
			get { return this._Number; }
			set
			{
				if (this._Number != value)
				{
					this._Number = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Name

		public string Name
		{
			get => SettingsHelper.GetDesktopName(this._Number);
			set => SettingsHelper.SetDesktopName(this._Number, value);
		}

		#endregion
	}
}
