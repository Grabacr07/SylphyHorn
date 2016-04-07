using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using SylphyHorn.Serialization;
using SylphyHorn.Services;
using SylphyHorn.UI.Pages;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SylphyHorn
{
	/// <summary>
	/// 既定の Application クラスを補完するアプリケーション固有の動作を提供します。
	/// </summary>
	sealed partial class App
	{
		/// <summary>
		/// 単一アプリケーション オブジェクトを初期化します。これは、実行される作成したコードの最初の行であるため、main() または WinMain() と論理的に等価です。
		/// </summary>
		public App()
		{
			WindowsAppInitializer.InitializeAsync(WindowsCollectors.Metadata | WindowsCollectors.Session);

			this.InitializeComponent();

			this.Suspending += this.HandleSuspending;
			this.UnhandledException += this.HandleUnhandledException;
		}


		/// <summary>
		/// アプリケーションがエンド ユーザーによって正常に起動されたときに呼び出されます。他のエントリ
		/// ポイントは、アプリケーションが特定のファイルを開くために起動されたときなどに使用されます。
		/// </summary>
		/// <param name="e">起動の要求とプロセスの詳細を表示します。</param>
		protected override async void OnLaunched(LaunchActivatedEventArgs e)
		{
#if DEBUG
			if (System.Diagnostics.Debugger.IsAttached)
			{
				this.DebugSettings.EnableFrameRateCounter = true;
			}
#endif

			var rootFrame = Window.Current.Content as Frame;
			if (rootFrame == null)
			{
				// ウィンドウにコンテンツが表示されていない場合
				// アプリケーションの初期化を実行

				// ナビゲーション コンテキストとして動作するフレームを作成し、最初のページに移動
				rootFrame = new Frame();
				rootFrame.NavigationFailed += this.HandleNavigationFailed;

				//if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
				//{
				//	// ToDo: 初期化・状態復元
				//}

				Window.Current.Content = rootFrame;
			}

			if (!LocalStorageProvider.Instance.IsLoaded)
			{
				await LocalStorageProvider.Instance.LoadAsync();
			}

			// デスクトップ クライアントに UI が起動したことを通知する
			// (= キー入力の一時停止をオンにして設定に書き込む)
			Settings.General.SuspendKeyDetection.Value = true;
			await LocalStorageProvider.Instance.SaveAsync();

			if (rootFrame.Content == null)
			{
				// ナビゲーション スタックが復元されない場合は、最初のページに移動
				rootFrame.Navigate(typeof(MainPage), e.Arguments);
			}

			Window.Current.Activate();
		}

		/// <summary>
		/// アプリケーションの実行が中断されたときに呼び出されます。アプリケーションが終了されるか、メモリの内容がそのままで再開されるかにかかわらず、アプリケーションの状態が保存されます。
		/// </summary>
		/// <param name="sender">中断要求の送信元。</param>
		/// <param name="e">中断要求の詳細。</param>
		private async void HandleSuspending(object sender, SuspendingEventArgs e)
		{
			using (e.GetDeferral())
			{
				Settings.General.SuspendKeyDetection.Value = false;
				await LocalStorageProvider.Instance.SaveAsync();
			}
		}

		/// <summary>
		/// 特定のページへの移動が失敗したときに呼び出されます。
		/// </summary>
		/// <param name="sender">移動に失敗したフレーム。</param>
		/// <param name="e">ナビゲーション エラーの詳細。</param>
		private void HandleNavigationFailed(object sender, NavigationFailedEventArgs e)
		{
			throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
		}

		private void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			// ToDo: 不慮の事故で死んだとき何かしたい
		}
	}
}
