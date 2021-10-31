using System;
using System.Threading;

namespace ValveQuery
{
	/// <summary>
	/// Provides clean up code.
	/// </summary>
	public class QueryBase : IDisposable
	{
		/// <summary>
		/// To check whether dispose method was called before.
		/// </summary>
		protected bool IsDisposed { get; set; }

		/// <summary>
		/// Disposes all the resources used by this instance.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
		}

		/// <summary>
		/// Throw <see cref="T:System.ObjectDisposedException" /> if this instance is already disposed.
		/// </summary>
		protected void ThrowIfDisposed()
		{
			if (IsDisposed)
			{
				throw new ObjectDisposedException(GetType().FullName);
			}
		}

		internal T Invoke<T>(Func<T> method, int attempts, AttemptCallback attemptcallback, bool throwExceptions) where T : class
		{
			int AttemptCounter = 0;
			while (true)
			{
				try
				{
					AttemptCounter++;
					if (attemptcallback != null)
					{
						ThreadPool.QueueUserWorkItem(delegate
						{
							attemptcallback(AttemptCounter);
						});
					}
					return method();
				}
				catch (Exception)
				{
					if (AttemptCounter >= attempts)
					{
						if (throwExceptions)
						{
							throw;
						}
						return null;
					}
				}
			}
		}

		/// <summary>
		/// Disposes all the resources used by this instance.
		/// </summary>
		public void Dispose()
		{
			if (IsDisposed)
			{
				return;
			}

			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Destructor
		/// </summary>
		~QueryBase()
		{
			Dispose(false);
		}
	}
}
