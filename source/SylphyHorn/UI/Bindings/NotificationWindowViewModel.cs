using MetroTrilithon.Mvvm;

namespace SylphyHorn.UI.Bindings
{
	public class NotificationWindowViewModel : WindowViewModel
	{
		#region Body 変更通知プロパティ

		private string _Body;

		public string Body
		{
			get { return this._Body; }
			set
			{
				if (this._Body != value)
				{
					this._Body = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


	}
}
