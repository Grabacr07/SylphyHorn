using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SylphyHorn.Views
{
	partial class MainWindow
	{
		public MainWindow()
		{
			this.InitializeComponent();
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			System.Windows.Application.Current.Shutdown();
		}
	}
}
