using SylphyHorn.UI.Bindings;

namespace SylphyHorn.UI.Pages
{
	internal sealed partial class MainPage
	{
		public MainPage()
		{
			this.InitializeComponent();
			this.DataContext = new MainPageViewModel();
		}
	}
}
