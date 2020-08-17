using System;
using MetroRadiance.Interop;
using MetroRadiance.Interop.Win32;
using SylphyHorn.UI.Bindings;

namespace SylphyHorn.UI
{
	partial class PinWindow
	{
		private readonly IntPtr _target;

		public PinWindow(IntPtr target, WindowPlacement placement)
			: base(placement)
		{
			this._target = target;
			this.InitializeComponent();
		}

		protected override bool TryGetTargetRect(out RECT rect)
		{
			return User32.IsZoomed(this._target)
				? TryGetWorkAreaFromHwnd(this._target, out rect)
				: TryGetWindowRectFromHwnd(this._target, out rect);
		}
	}
}
