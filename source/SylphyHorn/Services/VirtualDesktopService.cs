using System;
using System.Linq;
using System.Media;
using SylphyHorn.Serialization;
using WindowsDesktop;

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

		public static VirtualDesktop MoveToLeft(this IntPtr hWnd)
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
					VirtualDesktopHelper.MoveToDesktop(hWnd, left);
					return left;
				}
			}

			SystemSounds.Asterisk.Play();
			return null;
		}

		public static VirtualDesktop MoveToRight(this IntPtr hWnd)
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
					VirtualDesktopHelper.MoveToDesktop(hWnd, right);
					return right;
				}
			}

			SystemSounds.Asterisk.Play();
			return null;
		}

		public static VirtualDesktop MoveToNew(this IntPtr hWnd)
		{
			var newone = VirtualDesktop.Create();
			if (newone != null)
			{
				VirtualDesktopHelper.MoveToDesktop(hWnd, newone);
				return newone;
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

		public static void PinApp(this IntPtr hWnd)
		{
			VirtualDesktopHelper.PinApplication(hWnd);
		}

		public static void UnpinApp(this IntPtr hWnd)
		{
			VirtualDesktopHelper.UnpinApplication(hWnd);
		}

		public static void TogglePinApp(this IntPtr hWnd)
		{
			VirtualDesktopHelper.TogglePinApplication(hWnd);
		}
	}
}
