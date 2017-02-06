using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace KG.SE2.Utils.Collections
{
	public class ReadonlyBindingListWrapper : IBindingList
	{
		private IBindingList _innerList;

		public ReadonlyBindingListWrapper(IBindingList baseList)
		{
			UpdateInnerList(baseList);
		}

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

		public void ResumeEvents()
		{
			RaiseListChangedEvents = _raiseListChangedEventsInfo.Pop();
			if (RaiseListChangedEvents)
				raiseReset();
		}

		public void raiseReset()
		{
			OnListChanged(ListChangedType.Reset, -1, -1);
		}


		public void UpdateInnerList(IBindingList list)
		{
			if(_innerList == list)
				return;

			if (_innerList != null)
			{
				_innerList.ListChanged -= OnInnerListChanged;
				_innerList.Cast<object>().ToList().ForEach(x => UnsubscribeDeleted(x as IBindingListItem));
			}

			_innerList = list;

			_innerList.ListChanged += OnInnerListChanged;
			_innerList.Cast<object>().ToList().ForEach(x => SubscribeDeleted(x as IBindingListItem));

			OnListChanged(ListChangedType.Reset, -1, -1);
		}
		
		protected void UnsubscribeDeleted(IBindingListItem item)
		{
			if (item == null)
				return;

			item.Deleted -= ItemDeletd;
		}

		protected void SubscribeDeleted(IBindingListItem item)
		{
			if (item == null)
				return;

			item.Deleted += ItemDeletd;
		}

		protected virtual void ItemDeletd(object sender, EventArgs e)
		{
			((IBindingListItem)sender).Deleted -= ItemDeletd;
			_innerList.Remove(sender);
		}

		protected virtual void OnInnerListChanged(object sender, ListChangedEventArgs e)
		{
			OnListChanged(e);
		}

		#region Implementation of IEnumerable

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _innerList.GetEnumerator();
		}

		#endregion

		#region Implementation of ICollection

		void ICollection.CopyTo(Array array, int index)
		{
			_innerList.CopyTo(array, index);
		}

		int ICollection.Count
		{
			get { return _innerList.Count; }
		}

		bool IList.IsReadOnly
		{
			get { return _innerList.IsReadOnly; }
		}

		#endregion

		#region Implementation of ICollection

		public object SyncRoot
		{
			get { return _innerList.SyncRoot; }
		}

		public bool IsSynchronized
		{
			get { return _innerList.IsSynchronized; }
		}

		#endregion

		#region Implementation of IList

		public bool IsFixedSize
		{
			get { return _innerList.IsFixedSize; }
		}

		#endregion



		#region Implementation of IList

		int IList.Add(object value)
		{
			throw new NotSupportedException();
			//return _innerList.Add(value);
		}

		bool IList.Contains(object value)
		{
			return _innerList.Contains(value);
		}

		void IList.Clear()
		{
			throw new NotSupportedException();
			//_innerList.Clear();
		}

		int IList.IndexOf(object value)
		{
			return _innerList.IndexOf(value);
		}

		void IList.Insert(int index, object value)
		{
			throw new NotSupportedException();
			//_innerList.Insert(index, value);
		}

		void IList.Remove(object value)
		{
			throw new NotSupportedException();
			//_innerList.Remove(value);
		}

		void IList.RemoveAt(int index)
		{
			throw new NotSupportedException();
			//_innerList.RemoveAt(index);
		}

		object IList.this[int index]
		{
			get { return _innerList[index]; }
			set
			{
				throw new NotSupportedException();
				//_innerList[index] = value;
			}
		}

		#endregion

		#region Implementation of IBindingList

		object IBindingList.AddNew()
		{
			throw new NotSupportedException();
			//return _innerList.AddNew();
		}

		public virtual void AddIndex(PropertyDescriptor property)
		{
			_innerList.AddIndex(property);
		}

		public virtual void ApplySort(PropertyDescriptor property, ListSortDirection direction)
		{
			_innerList.ApplySort(property, direction);
		}

		public virtual int Find(PropertyDescriptor property, object key)
		{
			return _innerList.Find(property, key);
		}

		public virtual void RemoveIndex(PropertyDescriptor property)
		{
			_innerList.RemoveIndex(property);
		}

		public virtual void RemoveSort()
		{
			_innerList.RemoveSort();
		}

		public virtual bool AllowNew
		{
			get
			{
				return false;
				//return _innerList.AllowNew;
			}
		}

		public virtual bool AllowEdit
		{
			get { return _innerList.AllowEdit; }
		}

		public virtual bool AllowRemove
		{
			get
			{
				return false;
				//return _innerList.AllowRemove;
			}
		}

		public virtual bool SupportsChangeNotification
		{
			get { return _innerList.SupportsChangeNotification; }
		}

		public virtual bool SupportsSearching
		{
			get { return _innerList.SupportsSearching; }
		}

		public virtual bool SupportsSorting
		{
			get { return _innerList.SupportsSorting; }
		}

		public virtual bool IsSorted
		{
			get { return _innerList.IsSorted; }
		}

		public virtual PropertyDescriptor SortProperty
		{
			get { return _innerList.SortProperty; }
		}

		public virtual ListSortDirection SortDirection
		{
			get { return _innerList.SortDirection; }
		}

		public event ListChangedEventHandler ListChanged;

		#endregion

		protected virtual void OnListChanged(ListChangedType changedType, int newIndex, PropertyDescriptor pd)
		{
			OnListChanged(new ListChangedEventArgs(changedType, newIndex, pd));
		}

		protected virtual void OnListChanged(ListChangedType changedType, int newIndex, int oldIndex)
		{
			OnListChanged(new ListChangedEventArgs(changedType, newIndex, oldIndex));
		}

		public void RiseItemChanged(ListChangedEventArgs e)
		{
			OnListChanged(e);
		}

		void OnListChanged(ListChangedEventArgs e)
		{
			if (!RaiseListChangedEvents || ListChanged == null)
				return;

			ListChanged(this, e);
		}

	}
}