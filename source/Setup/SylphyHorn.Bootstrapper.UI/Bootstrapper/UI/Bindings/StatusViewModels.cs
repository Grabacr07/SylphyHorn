using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Livet;
using SylphyHorn.Properties;
using WixToolset.Bootstrapper;

namespace SylphyHorn.Bootstrapper.UI.Bindings
{
	// 現状、メッセージ表示程度しか要件がないため、Install/Uninstall/Upgrade いずれの場合も
	// ConfirmationViewModel とその DataTemplate だけで UI を実装できる。
	//
	// 今後、Install/Uninstall/Upgrade でそれぞれ専用の UI を表示しなければならなくなった場合、
	// ConfirmationViewModel ではなく専用の ViewModel を作成して MainWindowViewModel.Content に設定し、
	// それぞれの ViewModel に関連付けられた DataTemplate で UI を実装するのが望ましい。
	//
	// ProgressViewModel や、CompletionViewModel 等も同じ。

	public class PreparationViewModel : ViewModel
	{
	}

	public class ConfirmationViewModel : ViewModel
	{
		public Action ClickAction { get; set; }

		#region Header notification property

		private string _Header;

		public string Header
		{
			get { return this._Header; }
			set
			{
				if (this._Header != value)
				{
					this._Header = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Message notification property

		private string _Message;

		public string Message
		{
			get { return this._Message; }
			set
			{
				if (this._Message != value)
				{
					this._Message = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Button notification property

		private string _Button;

		public string Button
		{
			get { return this._Button; }
			set
			{
				if (this._Button != value)
				{
					this._Button = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public ConfirmationViewModel(Operation operation)
		{
			switch (operation)
			{
				case Operation.Install:
				case Operation.Upgrade:
					this.Header = Resources.InstallMode_Header;
					this.Message = Resources.InstallMode_Message;
					this.Button = Resources.Install;
					break;

				case Operation.Uninstall:
					this.Header = Resources.UninstallMode_Header;
					this.Message = Resources.UninstallMode_Message;
					this.Button = Resources.Uninstall;
					break;
			}
		}

		public void Click()
		{
			this.ClickAction?.Invoke();
		}
	}

	public class ProgressViewModel : ViewModel
	{
		#region Message notification property

		private string _Message;

		public string Message
		{
			get { return this._Message; }
			set
			{
				if (this._Message != value)
				{
					this._Message = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public ProgressViewModel(Operation operation)
		{
			switch (operation)
			{
				case Operation.Install:
				case Operation.Upgrade:
					this.Message = Resources.Progress_Install;
					break;

				case Operation.Uninstall:
					this.Message = Resources.Progress_Uninstall;
					break;
			}
		}
	}

	public class CompletionViewModel : ViewModel
	{
		public Action ClickAction { get; set; }

		#region Header notification property

		private string _Header;

		public string Header
		{
			get { return this._Header; }
			set
			{
				if (this._Header != value)
				{
					this._Header = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Message notification property

		private string _Message;

		public string Message
		{
			get { return this._Message; }
			set
			{
				if (this._Message != value)
				{
					this._Message = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Button notification property

		private string _Button;

		public string Button
		{
			get { return this._Button; }
			set
			{
				if (this._Button != value)
				{
					this._Button = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public CompletionViewModel(Operation operation, InstallResult result)
		{
			if (result.IsSucceeded)
			{
				switch (operation)
				{
					case Operation.Install:
					case Operation.Upgrade:
						this.Header = Resources.InstallCompleted;
						this.Message = Resources.InstallCompleted_Message;
						this.Button = Resources.Launch;
						break;

					case Operation.Uninstall:
						this.Header = Resources.UninstallCompleted;
						this.Message = Resources.UninstallCompleted_Message;
						this.Button = Resources.Close;
						break;
				}
			}
			else
			{
				switch (operation)
				{
					case Operation.Install:
					case Operation.Upgrade:
						this.Header = Resources.InstallFailed;
						this.Message = Resources.InstallFailed_Message;
						this.Button = Resources.Close;
						break;

					case Operation.Uninstall:
						this.Header = Resources.UninstallFailed;
						this.Message = Resources.UninstallFailed_Message;
						this.Button = Resources.Close;
						break;
				}
			}
		}

		public void Click()
		{
			this.ClickAction?.Invoke();
		}
	}

	public class CancellationViewModel : ViewModel
	{
		public Action CancelCancellationAction { get; set; }

		public Action PerformCancellationAction { get; set; }

		public ViewModel Fallback { get; }

		public bool CanClick { get; private set; } = true;

		public CancellationViewModel(ViewModel fallback)
		{
			this.Fallback = fallback;
		}

		public void CancelCancellation()
		{
			this.CanClick = false;
			this.CancelCancellationAction?.Invoke();
		}

		public void PerformCancellation()
		{
			this.CanClick = false;
			this.PerformCancellationAction?.Invoke();
		}
	}

	public class ErrorViewModel : ViewModel
	{
		#region Message notification property

		private string _Message;

		public string Message
		{
			get { return this._Message; }
			set
			{
				if (this._Message != value)
				{
					this._Message = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion
	}
}
