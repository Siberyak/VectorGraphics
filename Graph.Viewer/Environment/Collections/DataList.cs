using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace KG.SE2.Utils.Collections
{
	public class DataList<T> : BindingList<T>, IBindingList<T>
	{
		public DataList()
		{
		}

		public DataList(IList<T> list) : base(list ?? new List<T>())
		{
		}

		public event BeforeRemoveEventHandler BeforeRemoveItem;

		protected void OnBeforeRemoveItem(int index)
		{
			if (BeforeRemoveItem == null)
				return;

			BeforeRemoveItem(this, new BeforeRemoveEventArgs(index));
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
				OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
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
            Remove((T) sender);
        }

	    protected override object AddNewCore()
	    {
	        return base.AddNewCore();
	    }

	    public override void CancelNew(int itemIndex)
	    {
	        base.CancelNew(itemIndex);
	    }

	    public override void EndNew(int itemIndex)
	    {
	        base.EndNew(itemIndex);
	    }

	    protected override void SetItem(int index, T item)
	    {
			if(ReferenceEquals(this[index], item))
				return;

	        SuspendEvents();
	    	
            Remove(this[index]);
			InsertItem(index, item);
            
            ResumeEvents(false);
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index, index));

			//UnsubscribeDeleted(this[index] as IBindingListItem);
			//base.SetItem(index, item);
			//SubscribeDeleted(this[index] as IBindingListItem);
	    }

	    protected override void RemoveItem(int index)
	    {
            UnsubscribeDeleted(this[index] as IBindingListItem);
			OnBeforeRemoveItem(index);
            base.RemoveItem(index);
	    }

	    protected override void InsertItem(int index, T item)
	    {
            SubscribeDeleted(item as IBindingListItem);
            base.InsertItem(index, item);
	    }

	    protected override void ClearItems()
	    {
			SuspendEvents();
	        foreach (var item in Items)
                UnsubscribeDeleted(item as IBindingListItem);

            base.ClearItems();
			ResumeEvents(true);
	    }
	}


    public class DataListWithKeys<T, TKey> : DataList<T>
    {
        private Func<T, TKey> GetKey { get; set; }
        readonly Dictionary<TKey, T> _dict = new Dictionary<TKey, T>();

        public DataListWithKeys(Func<T, TKey> getKey)
        {
            GetKey = getKey;
        }

        public DataListWithKeys(IList<T> list, Func<T, TKey> getKey)
            : base(list)
        {
            GetKey = getKey;
            foreach (var item in list)
            {
                _dict.Add(GetKey(item), item);
            }
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            _dict.Clear();
        }

        protected override void InsertItem(int index, T item)
        {
            _dict.Add(GetKey(item), item);
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            _dict.Remove(GetKey(this[index]));
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, T item)
        {
            _dict.Remove(GetKey(this[index]));
            _dict.Add(GetKey(item), item);
            base.SetItem(index, item);
        }

        public override void CancelNew(int itemIndex)
        {
            base.CancelNew(itemIndex);
        }

        protected override object AddNewCore()
        {
            return base.AddNewCore();
        }

        public override void EndNew(int itemIndex)
        {
            base.EndNew(itemIndex);
        }

        protected override void OnAddingNew(AddingNewEventArgs e)
        {
            base.OnAddingNew(e);
        }

        public bool ContainsKey(TKey key)
        {
            return _dict.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            if (ContainsKey(key))
            {
                Remove(_dict[key]);
                return true;
            }

            return false;
        }

        public T this[TKey key]
        {
            get { return ContainsKey(key) ? _dict[key] : default(T); }
        }

        public ICollection<TKey> Keys
        {
            get { return _dict.Keys; }
        }
    }
}