using System;
using System.Linq;
using System.Media;
using WindowsDesktop;
using SylphyHorn.Serialization;
using VDMHelperCLR.Common;

namespace SylphyHorn.Services
{
	internal static class VirtualDesktopService
	{
		public static VirtualDesktop GetLeft()
		{
			var current = VirtualDesktop.Current;
			var desktops = VirtualDesktop.GetDesktops();

			return desktops.Length >= 2 && current.Id == desktops.First().Id
				? Settings.General.LoopDesktop ? desktops.Last() : null
				: current.GetLeft();
		}

		public static VirtualDesktop GetRight()
		{
			var current = VirtualDesktop.Current;
			var desktops = VirtualDesktop.GetDesktops();

			return desktops.Length >= 2 && current.Id == desktops.Last().Id
				? Settings.General.LoopDesktop ? desktops.First() : null
				: current.GetRight();
		}

		public static VirtualDesktop MoveToLeft(this IntPtr hWnd, IVdmHelper helper)
		{
			var current = VirtualDesktop.FromHwnd(hWnd);
			if (current != null)
			{
				var left = current.GetLeft();
				if (left == null)
				{
					if (Settings.General.LoopDesktop)
					{
						var desktops = VirtualDesktop.GetDesktops();
						if (desktops.Length >= 2) left = desktops.Last();
					}
				}
				if (left != null)
				{
					if (VirtualDesktopHelper.MoveToDesktop(hWnd, left)
						|| helper.MoveWindowToDesktop(hWnd, left.Id))
					{
						return left;
					}
				}
			}

			SystemSounds.Asterisk.Play();
			return null;
		}

		public static VirtualDesktop MoveToRight(this IntPtr hWnd, IVdmHelper helper)
		{
			var current = VirtualDesktop.FromHwnd(hWnd);
			if (current != null)
			{
				var right = current.GetRight();
				if (right == null)
				{
					if (Settings.General.LoopDesktop)
					{
						var desktops = VirtualDesktop.GetDesktops();
						if (desktops.Length >= 2) right = desktops.First();
					}
				}
				if (right != null)
				{
					if (VirtualDesktopHelper.MoveToDesktop(hWnd, right)
						|| helper.MoveWindowToDesktop(hWnd, right.Id))
					{
						return right;
					}
				}
			}

			SystemSounds.Asterisk.Play();
			return null;
		}

		public static VirtualDesktop MoveToNew(this IntPtr hWnd, IVdmHelper helper)
		{
			var newone = VirtualDesktop.Create();
			if (newone != null)
			{
				if (VirtualDesktopHelper.MoveToDesktop(hWnd, newone)
					|| helper.MoveWindowToDesktop(hWnd, newone.Id))
				{
					return newone;
				}
			}

			SystemSounds.Asterisk.Play();
			return null;
		}

		public static void Pin(this IntPtr hWnd)
		{
			VirtualDesktopHelper.PinWindow(hWnd);
		}

		public static void Unpin(this IntPtr hWnd)
		{
			VirtualDesktopHelper.UnpinWindow(hWnd);
		}

		public static void TogglePin(this IntPtr hWnd)
		{
			VirtualDesktopHelper.TogglePinWindow(hWnd);
		}
	}
}
