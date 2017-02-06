using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace KG.SE2.Utils.Collections
{
	public class PredicatedList<T> : IBindingList<T>
	{
		private readonly IBindingList<T> _original;
		private readonly Func<T, bool> _predicate;
		private readonly BindingList<T> _items = new BindingList<T>();

		public PredicatedList(IBindingList<T> original, Func<T, bool> predicate)
		{
			_original = original;
			_predicate = predicate;
			_original.ListChanged += OnOriginalListChanged;
			_original.BeforeRemoveItem += OnOriginalBeforeRemoveItem;
			_items.ListChanged += DelegateListChanged;
			AddOriginalItems();
		}

		private void OnOriginalBeforeRemoveItem(object sender, BeforeRemoveEventArgs e)
		{
			RemoveItem(e.Index);
		}

		private void DelegateListChanged(object sender, ListChangedEventArgs e)
		{
			if (e.ListChangedType == ListChangedType.ItemChanged && e.PropertyDescriptor != null)
			{
				var item = _items[e.NewIndex];
				if (!_predicate(item))
				{
					OnBeforeRemoveItem(e.NewIndex);
					_items.Remove(item);
					return;
				}
			}

			if (ListChanged == null)
				return;

			ListChanged(this, e);
		}

		private void AddOriginalItems()
		{
			foreach (var item in _original)
				AddItem(item);
		}

		private void AddItem(T item, int originalIndex = -1)
		{
			if(!_predicate(item))
				return;

			if(originalIndex == -1 || originalIndex == _original.Count())
				_items.Add(item);
			else
				_items.Insert(CalculateIndex(originalIndex), item);
		}

		private void RemoveItem(int index)
		{
			if(!_items.Contains(_original.ElementAt(index)))
				return;

			index = CalculateIndex(index);
			_items.RemoveAt(index);
		}

		private void MoveItem(int newIndex, int oldIndex)
		{
			if (!Contains(_original.ElementAt(newIndex)))
				return;

			newIndex = CalculateIndex(newIndex);
			oldIndex = CalculateIndex(oldIndex);

			if(newIndex == oldIndex)
				return;

			var item =_items[oldIndex];
			_items.RemoveAt(oldIndex);
			_items.Insert(newIndex, item);
		}

		private int CalculateIndex(int originalIndex)
		{
			var originals = _original.Take(originalIndex).ToArray();

			var index = originals.Sum(x => _items.Contains(x) ? 1 : 0);
			return index;
		}


		private void OnOriginalListChanged(object sender, ListChangedEventArgs e)
		{
			switch (e.ListChangedType)
			{
				case ListChangedType.Reset:
					SuspendEvents();
					_items.Clear();
					AddOriginalItems();
					ResumeEvents(true);
					break;
				case ListChangedType.ItemAdded:
					{
						var item = _original.ElementAt(e.NewIndex);
						AddItem(item, e.NewIndex);
						break;
					}
				case ListChangedType.ItemDeleted:
						break;
				case ListChangedType.ItemMoved:
					{
						MoveItem(e.NewIndex, e.OldIndex);
						break;
					}
				case ListChangedType.ItemChanged:
					{
						var item = _original.ElementAt(e.NewIndex);
						if (e.PropertyDescriptor == null || _items.Contains(item))
							break;

						AddItem(item, e.NewIndex);
						break;
					}
				case ListChangedType.PropertyDescriptorAdded:
					break;
				case ListChangedType.PropertyDescriptorDeleted:
					break;
				case ListChangedType.PropertyDescriptorChanged:
					break;
			}
		}

		public event BeforeRemoveEventHandler BeforeRemoveItem;

		protected void OnBeforeRemoveItem(int index)
		{
			if (BeforeRemoveItem == null)
				return;

			BeforeRemoveItem(this, new BeforeRemoveEventArgs(index));
		}



		#region Implementation of IEnumerable

		public IEnumerator<T> GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region Implementation of ICollection

		public void CopyTo(Array array, int index)
		{
			((ICollection)_items).CopyTo(array, index);
		}

		public bool Remove(T item)
		{
			return _original.Remove(item);
		}

		int ICollection<T>.Count
		{
			get { return _items.Count; }
		}

		bool ICollection<T>.IsReadOnly
		{
			get { return true; }
		}

		int ICollection.Count
		{
			get { return _items.Count; }
		}

		public object SyncRoot
		{
			get { return ((ICollection)_items).SyncRoot; }
		}

		public bool IsSynchronized
		{
			get { return ((ICollection)_items).IsSynchronized; }
		}

		#endregion

		#region Implementation of IList

		public int Add(object value)
		{
			return _original.Add(value);
		}

		public bool Contains(object value)
		{
			return ((IList)_items).Contains(value);
		}

		public void Add(T item)
		{
			_original.Add(item);
		}

		void ICollection<T>.Clear()
		{
			throw new NotSupportedException();
		}

		public bool Contains(T item)
		{
			return ((ICollection<T>)_items).Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			_items.CopyTo(array, arrayIndex);
		}

		void IList.Clear()
		{
			throw new NotSupportedException();
		}

		public int IndexOf(object value)
		{
			return ((IList)_items).IndexOf(value);
		}

		public void Insert(int index, object value)
		{
			_original.Insert(index, value);
		}

		public void Remove(object value)
		{
			throw new NotSupportedException();
		}

		public int IndexOf(T item)
		{
			return ((IList<T>)_items).IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			_original.Insert(index, item);
		}

		void IList<T>.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		public T this[int index]
		{
			get { return _items[index]; }
			set
			{
				throw new NotSupportedException();
			}
		}

		void IList.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		object IList.this[int index]
		{
			get { return this[index]; }
			set
			{
				throw new NotSupportedException();
			}
		}

		bool IList.IsReadOnly
		{
			get { return false; }
		}

		public bool IsFixedSize
		{
			get { return false; }
		}

		#endregion

		#region Implementation of IBindingList

		public object AddNew()
		{
			throw new NotSupportedException();
		}

		public void AddIndex(PropertyDescriptor property)
		{
			((IBindingList) _items).AddIndex(property);
		}

		public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
		{
			((IBindingList)_items).ApplySort(property, direction);
		}

		public int Find(PropertyDescriptor property, object key)
		{
			return ((IBindingList)_items).Find(property, key);
		}

		public void RemoveIndex(PropertyDescriptor property)
		{
			((IBindingList)_items).RemoveIndex(property);
		}

		public void RemoveSort()
		{
			((IBindingList)_items).RemoveSort();
		}

		public bool AllowNew
		{
			get { return false; }
		}

		public bool AllowEdit
		{
			get { return _original.AllowEdit; }
		}

		public bool AllowRemove
		{
			get { return false; }
		}

		public bool SupportsChangeNotification
		{
			get { return ((IBindingList)_items).SupportsChangeNotification; }
		}

		public bool SupportsSearching
		{
			get { return ((IBindingList)_items).SupportsSearching; }
		}

		public bool SupportsSorting
		{
			get { return ((IBindingList)_items).SupportsSorting; }
		}

		public bool IsSorted
		{
			get { return ((IBindingList)_items).IsSorted; }
		}

		public PropertyDescriptor SortProperty
		{
			get { return ((IBindingList)_items).SortProperty; }
		}

		public ListSortDirection SortDirection
		{
			get { return ((IBindingList)_items).SortDirection; }
		}

		public event ListChangedEventHandler ListChanged;
		//{
		//    add { _items.ListChanged += value; }
		//    remove { _items.ListChanged -= value;}
		//}

		private void OnListChanged(ListChangedType type, int newIndex, int oldIndex = -1)
		{
			if (ListChanged == null)
				return;

			ListChanged(this, new ListChangedEventArgs(type, newIndex, oldIndex));
		}

		#endregion

		#region Implementation of IBindingList<T>

		private bool _raiseListChangedEvents = true;
		public bool RaiseListChangedEvents
		{
			get { return _raiseListChangedEvents; }
			private set { _raiseListChangedEvents = value; }
		}

		readonly Stack<bool> _raiseListChangedEventsInfo = new Stack<bool>();
		public void SuspendEvents()
		{
			_raiseListChangedEventsInfo.Push(RaiseListChangedEvents);
			RaiseListChangedEvents = false;
		}

        public void ResumeEvents(bool raiseReset)
		{
			RaiseListChangedEvents = _raiseListChangedEventsInfo.Pop();
            if (RaiseListChangedEvents && raiseReset)
				OnListChanged(ListChangedType.Reset, -1);
		}

		#endregion
	}
}