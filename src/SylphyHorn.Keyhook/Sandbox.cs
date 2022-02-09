using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WindowsHook;

namespace SylphyHorn;

public static class Sandbox
{
	[Conditional("DEBUG")]
	public static void Method()
	{
		var sequence = Sequence.FromString("Control+Alt+U,Control+Alt+U,U");
		var assignment = new Dictionary<Sequence, Action>
		{
			{ sequence, () => Debug.WriteLine("Sequence") },
		};

		Hook.GlobalEvents().OnSequence(assignment);
	}
}
