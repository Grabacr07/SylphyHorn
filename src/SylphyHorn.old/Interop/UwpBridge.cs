using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;

namespace SylphyHorn.Interop
{
	public static class UwpBridge
	{
		public static Task<T> ToTask<T>(this IAsyncOperation<T> operation)
		{
			var source = new TaskCompletionSource<T>();

			operation.Completed = (asyncInfo, asyncStatus) =>
			{
				switch (asyncStatus)
				{
					case AsyncStatus.Completed:
						source.SetResult(asyncInfo.GetResults());
						break;

					case AsyncStatus.Canceled:
						source.SetCanceled();
						break;

					case AsyncStatus.Error:
						source.SetException(asyncInfo.ErrorCode);
						break;
				}
			};

			return source.Task;
		}
	}
}
