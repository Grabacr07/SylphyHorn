using System;
using MetroRadiance.Interop;
using MetroRadiance.Interop.Win32;
using SylphyHorn.UI.Bindings;

namespace SylphyHorn.UI
{
	partial class SwitchWindow
	{
		private readonly IntPtr _target;

		public SwitchWindow(IntPtr target, WindowPlacement placement)
			: base(placement)
		{
			this._target = target;
			this.InitializeComponent();
		}

		protected override bool TryGetTargetRect(out RECT rect)
		{
			return TryGetWorkAreaFromHmonitor(this._target, out rect);
		}
	}
}
