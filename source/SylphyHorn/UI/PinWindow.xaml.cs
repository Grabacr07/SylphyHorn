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

		protected override bool TryGetTargetRect(out RECT rect, out RECT? capableRect)
		{
			capableRect = null;
			if (User32.IsZoomed(this._target))
			{
				return TryGetWorkAreaFromHwnd(this._target, out rect);
			}
			else
			{
				if (!TryGetWindowRectFromHwnd(this._target, out rect)) return false;
				if (!this.Placement.HasFlag(WindowPlacement.OutsideY)) return true;
				if (!TryGetWorkAreaFromHwnd(this._target, out var temp)) return false;

				capableRect = temp;
				return true;
			}
		}
	}
}
