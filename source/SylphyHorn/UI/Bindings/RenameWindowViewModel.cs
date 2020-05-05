using MetroTrilithon.Mvvm;
using SylphyHorn.Services;

namespace SylphyHorn.UI.Bindings
{
	public class RenameWindowViewModel : WindowViewModel
	{
		#region Index

		private int _Index;

		public int Index
		{
			get { return this._Index; }
			set
			{
				if (this._Index != value)
				{
					this._Index = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Name

		public string Name
		{
			get => SettingsHelper.GetDesktopName(this._Index);
			set
			{
				SettingsHelper.SetDesktopName(this._Index, value);
				this.Close();
			}
		}

		#endregion
	}
}
