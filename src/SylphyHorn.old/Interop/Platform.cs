
namespace SylphyHorn.Interop
{
	public static class Platform
	{
		public static bool IsUwp
#if APPX
			=> true;
#else
			=> false;
#endif
	}
}
