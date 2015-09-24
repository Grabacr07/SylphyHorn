using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Livet;
using MetroTrilithon.Mvvm;
using MetroTrilithon.Threading.Tasks;
using SylphyHorn.Properties;

namespace SylphyHorn.Bootstrapper.UI.Bindings
{
	public class MainWindowViewModel : WindowViewModel
	{
		private readonly Installer _installer;

		#region Content notification property

		private ViewModel _Content = new PreparationViewModel();

		public ViewModel Content
		{
			get { return this._Content; }
			set
			{
				if (this._Content != value)
				{
					this._Content = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public MainWindowViewModel(Installer installer)
		{
			this._installer = installer;
			this.Title = Resources.WindowTitle;

			this._installer
				.Subscribe(nameof(Installer.Status), () => this.ChangeStatus())
				.AddTo(this);
		}

		protected override void InitializeCore()
		{
			base.InitializeCore();
			this.Detect().Forget();
		}

		protected override void CloseCanceledCallbackCore()
		{
			base.CloseCanceledCallbackCore();

			var current = this.Content;
			var cancel = new CancellationViewModel(current);
			cancel.CancelCancellationAction = () => this.Content = cancel.Fallback;
			cancel.PerformCancellationAction = () => this._installer.Cancel();

			this.Content = cancel;
		}

		private async Task Detect()
		{
			await Task.Delay(1500); // いわゆる演出
			this._installer.Detect();
		}

		private void ChangeStatus()
		{
			switch (this._installer.Status)
			{
				case Status.Preparing:
					this.CanClose = true;
					this.Content = new PreparationViewModel();
					break;

				case Status.Detected:
					this.CanClose = true;
					this.Content = new ConfirmationViewModel(this._installer.Operation)
					{
						ClickAction = () => this._installer.Start(),
					};
					break;

				case Status.Planning:
				case Status.Applying:
					this.CanClose = false;
					if (this.Content is ProgressViewModel) break;
					this.Content = new ProgressViewModel(this._installer.Operation);
					break;

				case Status.Completed:
					this.CanClose = true;
					if (this._installer.Result != null && this._installer.Result.Value.IsSucceeded)
					{
						var content = new CompletionViewModel(this._installer.Operation, this._installer.Result.Value);
						switch (this._installer.Operation)
						{
							case Operation.Install:
							case Operation.Upgrade:
								content.ClickAction = () =>
								{
									Launcher.Launch();
									this.Close();
								};
								break;
							case Operation.Uninstall:
								content.ClickAction = () => this.Close();
								break;
						}
						this.Content = content;
						break;
					}
					goto default;

				default:
					this.CanClose = true;
					this.Content = new ErrorViewModel();
					break;
			}
		}
	}
}
