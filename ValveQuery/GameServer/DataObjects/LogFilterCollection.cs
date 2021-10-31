using System;
using System.Collections;
using System.Collections.Generic;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Represents collection of logfilter.
	/// </summary>
	public class LogFilterCollection : IEnumerable<LogFilter>, IEnumerable
	{
		private List<LogFilter> Filters { get; } = new List<LogFilter>();

		/// <summary>
		/// used to set lock on add/remove of filter.
		/// </summary>
		protected internal static object LockObj = new object();

		/// <summary>
		/// Enables all filters.
		/// </summary>
		public void EnableAll()
		{
			Filters.ForEach(delegate (LogFilter x)
			{
				x.Enabled = true;
			});
		}

		/// <summary>
		/// Enables filter of specific type.
		/// </summary>
		/// <param name="type">Filter type.</param>
		public void EnableAll(Type type)
		{
			Filters.ForEach(x =>
			{
				if (x.GetType() == type)
				{
					x.Enabled = true;
				}
			});
		}

		/// <summary>
		/// Disables all filters.
		/// </summary>
		public void DisableAll()
		{
			Filters.ForEach(x =>
			{
				x.Enabled = false;
			});
		}

		/// <summary>
		/// Disables filter of specific type.
		/// </summary>
		/// <param name="type">Filter type.</param>
		public void DisableAll(Type type)
		{
			Filters.ForEach(x =>
			{
				if (x.GetType() == type)
				{
					x.Enabled = false;
				}
			});
		}

		/// <summary>
		/// Adds a filter to the end of the collection.
		/// </summary>
		/// <param name="filter"></param>
		public void Add(LogFilter filter)
		{
			lock (LockObj)
			{
				Filters.Add(filter);
			}
		}

		/// <summary>
		/// Removes specified filter from the collection.
		/// </summary>
		/// <param name="filter"></param>
		public void Remove(LogFilter filter)
		{
			lock (LockObj)
			{
				Filters.Remove(filter);
			}
		}

		/// <summary>
		/// Removes all filters from the collection.
		/// </summary>
		public void Clear() => Filters.Clear();

		/// <summary>
		/// Returns an enumerator that iterates through the Filter collection.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<LogFilter> GetEnumerator()
		{
			foreach (LogFilter filter in Filters)
			{
				yield return filter;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
