using System;

namespace ValveQuery
{
	internal static class Extensions
	{
		internal static void Fire<T>(this EventHandler<T> handler, object sender, T eventArgs) where T : EventArgs
		{
			handler?.Invoke(sender, eventArgs);
		}
	}
}
