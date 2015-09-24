using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#if WINDOWS_UWP
using Windows.System;
#else
using VirtualKey = System.Windows.Forms.Keys;
using System.Windows.Input;
#endif

namespace SylphyHorn.Services
{
	public static class KeyHelper
	{
		public static bool IsModifyKey(this VirtualKey key)
		{
			return IsModifyKey((uint)key);
		}

#if !WINDOWS_UWP
		public static bool IsModifyKey(this Key key)
		{
			return IsModifyKey((uint)key.ToVirtualKey());
		}

		public static VirtualKey ToVirtualKey(this Key key)
		{
			return (VirtualKey)KeyInterop.VirtualKeyFromKey(key);
		}
#endif

		private static bool IsModifyKey(uint keyCode)
		{
			switch (keyCode)
			{
				case 164: // LMenu
				case 162: // LControlKey
				case 160: // LShiftKey
				case 091: // LWin
				case 165: // RMenu
				case 163: // RControlKey
				case 161: // RShiftKey
				case 092: // RWin
					return true;

				default:
					return false;
			}
		}
	}
}
