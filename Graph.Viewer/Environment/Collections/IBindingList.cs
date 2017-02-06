using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace KG.SE2.Utils.Collections
{
	public class BeforeRemoveEventArgs : EventArgs
	{
		public BeforeRemoveEventArgs(int index)
		{
			Index = index;
		}

		public int Index { get; private set; }
	}

	public delegate void BeforeRemoveEventHandler(object sender, BeforeRemoveEventArgs e);

	public interface IBindingList<TItem> : IBindingList, IList<TItem>
	{
		event BeforeRemoveEventHandler BeforeRemoveItem;
		bool RaiseListChangedEvents { get; }
		void SuspendEvents();
		void ResumeEvents(bool raiseReset = true);
	}

	public static class EnumerableExtender
	{
		public static IBindingList<T> ToBindingList<T>(this IEnumerable<T> collection)
		{
			if (collection == null)
				return new DataList<T>();

			if (collection is IBindingList<T>)
				return (IBindingList<T>) collection;

			if (collection is List<T>)
				return new DataList<T>((IList<T>) collection);

			return new DataList<T>(collection.ToList());
		}
	}
}