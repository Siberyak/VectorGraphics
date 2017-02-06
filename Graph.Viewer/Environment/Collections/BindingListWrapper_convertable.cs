using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace KG.SE2.Utils.Collections
{
	public class BindingListWrapper<TBase, TResult> : IBindingList<TResult>
	{
		protected class FakePropertyDescriptor : PropertyDescriptor
		{
			public FakePropertyDescriptor(string name) : base(name, new Attribute[0])
			{
			}

			#region Overrides of PropertyDescriptor

			public override bool CanResetValue(object component)
			{
				return false;
			}

			public override object GetValue(object component)
			{
				throw new NotImplementedException();
			}

			public override void ResetValue(object component)
			{
				throw new NotImplementedException();
			}

			public override void SetValue(object component, object value)
			{
				throw new NotImplementedException();
			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}

			public override Type ComponentType
			{
				get { return typeof(BindingListWrapper<TBase, TResult>); }
			}

			public override bool IsReadOnly
			{
				get { return true; }
			}

			public override Type PropertyType
			{
				get { return typeof(object); }
			}

			#endregion
		}


		protected Func<TBase, TResult> _baseToResult;
		protected Func<TResult, TBase> _resultToBase;
		private IBindingList<TBase> _innerList;
		protected IList<TBase> BaseList { get { return _innerList; } }


		public BindingListWrapper(IList<TBase> baseList, Func<TBase, TResult> baseToResult, Func<TResult, TBase> resultToChild)
		{
			_resultToBase = resultToChild;
			_baseToResult = baseToResult;
			UpdateInnerList(baseList);
		}

		public event BeforeRemoveEventHandler BeforeRemoveItem;

		protected void OnBeforeRemoveItem(int index)
		{
			if (BeforeRemoveItem == null)
				return;

			BeforeRemoveItem(this, new BeforeRemoveEventArgs(index));
		}

		public void SuspendEvents()
		{
			_innerList.SuspendEvents();
		}

		public void ResumeEvents(bool raiseReset)
		{
            _innerList.ResumeEvents(raiseReset);
		}

		public bool RaiseListChangedEvents
		{
			get { return _innerList.RaiseListChangedEvents; }
		}

		public void raiseReset()
		{
			OnListChanged(ListChangedType.Reset, -1, -1);
		}


        /// <summary>
        /// обновляет внутренний список и кидает событие ListChanged Reset
        /// </summary>
        /// <param name="list"></param>
		protected virtual void UpdateInnerList(IList<TBase> list)
		{
            if(ReferenceEquals(_innerList, list))
                return;

			OnInnerListChanging();

			_innerList = GetInnerListInstance(list);

			if(OnInnerListChanged())
                OnListChanged(ListChangedType.Reset, -1, -1);
		}

        /// <summary>
        /// если внутренний список есть - подписывается на события
        /// </summary>
        /// <returns>признак того, что надо кинуть ListChanged Reset</returns>
        protected virtual bool OnInnerListChanged()
	    {
	        _innerList.ListChanged += OnInnerListChanged;
	        _innerList.ToList().ForEach(x => SubscribeDeleted(x as IBindingListItem));
            return true;
	    }

        /// <summary>
        /// если внутренний список есть - отписывается от всех событий
        /// </summary>
	    protected virtual void OnInnerListChanging()
	    {
	        if (_innerList != null)
	        {
	            _innerList.ListChanged -= OnInnerListChanged;
	            _innerList.ToList().ForEach(x => UnsubscribeDeleted(x as IBindingListItem));
	        }
	    }

	    protected virtual TResult BaseToResult(TBase @base)
		{
			var result = _baseToResult(@base);
			ResubscribePropertyChanged(result as INotifyPropertyChanged);
			return result;

		}

		protected void ResubscribePropertyChanged(INotifyPropertyChanged npc)
		{
			if (npc == null)
				return;

			npc.PropertyChanged -= OnBOPropertyChanged;
			npc.PropertyChanged += OnBOPropertyChanged;
		}

		protected void OnBOPropertyChanged(object sender, PropertyChangedEventArgs e)
		{

			var contract = ResultToBase((TResult)sender);
			if (!BaseList_Contains(contract))
				((INotifyPropertyChanged)sender).PropertyChanged -= OnBOPropertyChanged;
			else
			{
				if (e.PropertyName == ">>")
					SuspendEvents();
				else if (e.PropertyName == "<<" && !RaiseListChangedEvents)
				{
					ResumeEvents(false);
				    var index = BaseList_IndexOf(contract);
				    OnListChanged(ListChangedType.ItemChanged, index, index);
				}
				else if (RaiseListChangedEvents)
					OnListChanged(ListChangedType.ItemChanged, BaseList_IndexOf(contract), new FakePropertyDescriptor(e.PropertyName));
			}
		}

		protected virtual TBase ResultToBase(TResult result)
		{
			return _resultToBase(result);
		}

		protected virtual IBindingList<TBase> GetInnerListInstance(IList<TBase> list)
		{
			return list as IBindingList<TBase> ?? new DataList<TBase>(list);
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
			switch (e.ListChangedType)
			{
				case ListChangedType.Reset:
				case ListChangedType.ItemAdded:
				case ListChangedType.ItemDeleted:
				case ListChangedType.ItemMoved:
					OnListChanged(e.ListChangedType, e.NewIndex, e.OldIndex);
					break;
				case ListChangedType.ItemChanged:
					OnListChanged(e.ListChangedType, e.NewIndex, null/*new FakePropertyDescriptor("")*/);
					break;
				case ListChangedType.PropertyDescriptorAdded:
					break;
				case ListChangedType.PropertyDescriptorDeleted:
					break;
				case ListChangedType.PropertyDescriptorChanged:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		#region BaseList Methods

		protected virtual IEnumerator<TBase> BaseList_GetEnumerator()
		{
			return BaseList.GetEnumerator();
		}

		protected virtual void BaseList_Add(TBase item)
		{
			SubscribeDeleted(item as IBindingListItem);
			_innerList.Add(item);
		}

		protected virtual object BaseList_AddNew()
		{
			var value = _innerList.AddNew();
			SubscribeDeleted(value as IBindingListItem);
			return value;
		}

		protected virtual void BaseList_Clear()
		{
			SuspendEvents();
			try
			{
				BaseList.ToList().ForEach(x => UnsubscribeDeleted(x as IBindingListItem));
				BaseList.Clear();
			}
			finally
			{
				ResumeEvents(true);
			}
		}

		protected virtual bool BaseList_Contains(TBase item)
		{
			return _innerList.Contains(item);
		}

		protected virtual void BaseList_CopyTo(TBase[] array, int arrayIndex)
		{
			_innerList.CopyTo(array, arrayIndex);
		}

		protected virtual bool BaseList_Remove(TBase item)
		{
			if (!BaseList_Contains(item))
			{
				OnListChanged(ListChangedType.Reset, -1, -1);
				return false;
			}

			UnsubscribeDeleted(item as IBindingListItem);
			OnBeforeRemoveItem(BaseList_IndexOf(item));
			return _innerList.Remove(item);
		}

		protected virtual void BaseList_RemoveAt(int index)
		{
			UnsubscribeDeleted(BaseList[index] as IBindingListItem);
			OnBeforeRemoveItem(index);
			BaseList.RemoveAt(index);
		}

		protected virtual int BaseListCount
		{
			get { return BaseList.Count; }
		}

		protected virtual bool BaseListIsReadOnly
		{
			get { return BaseList.IsReadOnly; }
		}

		protected virtual bool BaseListIsFixedSize
		{
			get { return _innerList.IsFixedSize; }
		}

		protected virtual int BaseList_IndexOf(TBase item)
		{
			return _innerList.IndexOf(item);
		}

		protected virtual void BaseList_Insert(int index, TBase item)
		{
			SubscribeDeleted(item as IBindingListItem);
			_innerList.Insert(index, item);
		}

		protected virtual void BaseList_MoveTo(int index, TBase item)
		{
			if (!_innerList.Contains(item))
				return;

			BaseList_Remove(item);
			BaseList_Insert(index, item);
		}

		protected virtual TBase BaseList_Get(int index)
		{
			return BaseList[index];
		}

		protected virtual void BaseList_Set(int index, TBase value)
		{
		    SuspendEvents();
			
            BaseList_RemoveAt(index);
			BaseList_Insert(index, value);
            
            ResumeEvents(false);
            OnListChanged(ListChangedType.ItemChanged, index, index);

			//UnsubscribeDeleted(BaseList[index] as IBindingListItem);
			//BaseList[index] = value;
			//SubscribeDeleted(BaseList[index] as IBindingListItem);
		}

		#endregion

		#region ResultList methods

		public virtual IEnumerator<TResult> GetEnumerator()
		{
			return new EnumeratorWithConvertation<TBase, TResult>(BaseList, BaseToResult);
		}

		public virtual void Add(TResult item)
		{
			var value = ResultToBase(item);
			BaseList_Add(value);
		}

		public virtual bool Contains(TResult item)
		{
			return BaseList_Contains(ResultToBase(item));
		}

		public virtual void CopyTo(TResult[] array, int arrayIndex)
		{
			BaseList.Select(BaseToResult).ToArray().CopyTo(array, arrayIndex);
		}

		public virtual void Insert(int index, TResult item)
		{
			var value = ResultToBase(item);
			BaseList_Insert(index, value);
		}

		public virtual void Clear()
		{
			BaseList_Clear();
		}

		public virtual int IndexOf(TResult item)
		{
			return BaseList_IndexOf(ResultToBase(item));
		}

		public virtual bool Remove(TResult item)
		{
			var value = ResultToBase(item);
			return BaseList_Remove(value);
		}

		public virtual void RemoveAt(int index)
		{
			BaseList_RemoveAt(index);
		}

		public virtual TResult this[int index]
		{
			get
			{
				return BaseToResult(BaseList_Get(index));
			}
			set
			{
				BaseList_Set(index, ResultToBase(value));
			}
		}

		public virtual bool IsReadOnly
		{
			get { return BaseListIsReadOnly; }
		}

		public virtual bool IsFixedSize
		{
			get { return BaseListIsFixedSize; }
		}

		public virtual object SyncRoot
		{
			get { return _innerList.SyncRoot; }
		}

		public virtual bool IsSynchronized
		{
			get { return _innerList.IsSynchronized; }
		}

		#endregion

		#region Implementation of IEnumerable

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region Implementation of ICollection

		void ICollection.CopyTo(Array array, int index)
		{
			BaseList.Select(BaseToResult).ToArray().CopyTo(array, index);
		}

		int ICollection<TResult>.Count
		{
			get { return BaseList.Count; }
		}

		bool ICollection<TResult>.IsReadOnly
		{
			get { return IsReadOnly; }
		}

		int ICollection.Count
		{
			get { return BaseList.Count; }
		}

		#endregion

		#region Implementation of IList

		int IList.Add(object value)
		{
			var item = ResultToBase((TResult)value);
			BaseList_Add(item);

			return BaseListCount - 1;
		}

		bool IList.Contains(object value)
		{
			return Contains((TResult)value);
		}

		void ICollection<TResult>.Clear()
		{
			Clear();
		}

		int IList.IndexOf(object value)
		{
			return IndexOf((TResult)value);
		}

		void IList.Insert(int index, object value)
		{
			Insert(index, (TResult)value);
		}

		void IList.Remove(object value)
		{
			Remove((TResult)value);
		}

		void IList.RemoveAt(int index)
		{
			RemoveAt(index);
		}

		object IList.this[int index]
		{
			get { return this[index]; }
			set { this[index] = (TResult)value; }
		}

		#endregion

		#region Implementation of IBindingList

		object IBindingList.AddNew()
		{
			return ResultList_AddNew();
		}

		protected virtual object ResultList_AddNew()
		{
			return BaseToResult((TBase)BaseList_AddNew());
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
			get { return false; }
		}

		public virtual bool AllowEdit
		{
			get { return true; }
		}

		public virtual bool AllowRemove
		{
			get { return true; }
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

		public virtual TBase this[TResult result]
		{
			get
			{
				var @base = ResultToBase(result);
				return BaseList.FirstOrDefault(x => Equals(x, @base));
			}
		}


		public void RiseItemChanged(ListChangedEventArgs e)
		{
			OnListChanged(e);
		}

		protected void OnListChanged(ListChangedEventArgs e)
		{
			if (!RaiseListChangedEvents || ListChanged == null)
				return;

			ListChanged(this, e);
		}

	}
}