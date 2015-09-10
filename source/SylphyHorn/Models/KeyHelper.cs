using System.Windows.Input;

namespace SylphyHorn.Models
{
	public static class KeyHelper
	{
		public static bool IsModifyKey(this Key key)
		{
			// System.Windows.Forms.KeyEventArgs.Modifiers が LWin と RWin を含めてくれないので…
			// あと Left と Right 区別できたらいいなって…
			// _:(´ཀ`」 ∠):_

			return key == Key.LeftAlt
				|| key == Key.LeftCtrl
				|| key == Key.LeftShift
				|| key == Key.LWin
				|| key == Key.RightAlt
				|| key == Key.RightCtrl
				|| key == Key.RightShift
				|| key == Key.RWin;
		}
	}
}
