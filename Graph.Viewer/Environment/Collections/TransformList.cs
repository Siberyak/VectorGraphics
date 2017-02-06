using System;
using System.Collections.Generic;

namespace KG.SE2.Utils.Collections
{
	public abstract class TransformList<TBase, TResult> : BindingListWrapper<TBase, TResult>, IBindingList<TBase>
	{
		protected TransformList(Func<TBase, TResult> fromBaseToResult, Func<TResult, TBase> fromResultToBase)
			: this(new DataList<TBase>(), fromBaseToResult, fromResultToBase)
		{
		}

		protected TransformList(IList<TBase> baseList, Func<TBase, TResult> fromBaseToResult, Func<TResult, TBase> fromResultToBase)
			: base(baseList, fromBaseToResult, fromResultToBase)
		{
		}

		public bool IsEmpty { get { return BaseListCount == 0; } }

		#region Implementation of IEnumerable<out TBase>

		IEnumerator<TBase> IEnumerable<TBase>.GetEnumerator()
		{
			return BaseList_GetEnumerator();
		}

		#endregion

		#region Implementation of ICollection<TBase>

		void ICollection<TBase>.Add(TBase item)
		{
			BaseList_Add(item);
		}

		void ICollection<TBase>.Clear()
		{
			BaseList_Clear();
		}

		bool ICollection<TBase>.Contains(TBase item)
		{
			return BaseList_Contains(item);
		}

		void ICollection<TBase>.CopyTo(TBase[] array, int arrayIndex)
		{
			BaseList_CopyTo(array, arrayIndex);
		}

		bool ICollection<TBase>.Remove(TBase item)
		{
			return BaseList_Remove(item);
		}

		int ICollection<TBase>.Count
		{
			get { return BaseListCount; }
		}

		bool ICollection<TBase>.IsReadOnly
		{
			get { return BaseListIsReadOnly; }
		}

		#endregion

		#region Implementation of IList<TBase>

		int IList<TBase>.IndexOf(TBase item)
		{
			return BaseList_IndexOf(item);
		}

		void IList<TBase>.Insert(int index, TBase item)
		{
			BaseList_Insert(index, item);
		}

		TBase IList<TBase>.this[int index]
		{
			get { return BaseList_Get(index); }
			set { BaseList_Set(index,value); }
		}

		#endregion
	}
}