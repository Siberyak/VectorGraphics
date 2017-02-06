using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace KG.SE2.Utils.Collections
{
	public abstract class LinksListBase<TLink, TOwner, TRelated> : TransformList<TLink, TRelated>, ILinksList<TLink, TOwner, TRelated>
		where TLink : ILink<TOwner, TRelated>
	{
		private readonly Func<TOwner, TRelated, TLink> _createNewLink;
		private readonly Func<TLink, bool> _deleteLink;

		protected override TLink ResultToBase(TRelated related)
		{
			return Contains(related)
			       	? this[related]
			       	: _createNewLink(Owner, related);
		}

		protected LinksListBase(TOwner owner, Func<TOwner, TRelated, TLink> createNewLink, Func<TLink, bool> deleteLink)
			: base(link => link.Related, null)
		{
			Owner = owner;
			_createNewLink = createNewLink;
			_deleteLink = deleteLink;
		}

		protected LinksListBase(TOwner owner, IList<TLink> baseList, Func<TOwner, TRelated, TLink> createNewLink, Func<TLink, bool> deleteLink)
			: base(baseList, link => link.Related, null)
		{
			Owner = owner;
			_createNewLink = createNewLink;
			_deleteLink = deleteLink;
		}

		#region Implementation of ILinksList<TLink,TOwner,TRelated>

		public TOwner Owner { get; private set; }

		#endregion

		public override void Add(TRelated item)
		{
			var link = _createNewLink(Owner, item);
            if(Equals(link, default(TLink)))
                return;

			if (!BaseList_Contains(link))
				BaseList_Add(link);
			else
				OnListChanged(ListChangedType.ItemAdded, BaseListCount - 1, -1);
		}

		protected override object ResultList_AddNew()
		{
			throw new NotSupportedException();
		}

		public override bool AllowNew
		{
			get { return false; }
		}

		public override void Clear()
		{
			BaseList.ToList().ForEach(x => RemoveLink(x));
		}

		public override void Insert(int index, TRelated item)
		{
			var link = _createNewLink(Owner, item);

            if (Equals(link, default(TLink)))
                return;

			if (!BaseList_Contains(link))
				BaseList_Insert(index, link);
			else
				BaseList_MoveTo(index, link);
		}

		public override int IndexOf(TRelated item)
		{
			return BaseList.IndexOf(this[item]);
		}

		public override TLink this[TRelated result]
		{
			get { return BaseList.FirstOrDefault(x => Equals(x.Related, result)); }
		}

		public override bool Remove(TRelated item)
		{
			return RemoveLink(this[item]);
		}

		public override void RemoveAt(int index)
		{
			RemoveLink(BaseList[index]);
		}

		bool RemoveLink(TLink link)
		{
			if (Equals(link, default(TLink)))
				return false;

			var ret = _deleteLink(link);

			if (ret)
				BaseList_Remove(link);

			return ret;
		}

		public override bool Contains(TRelated item)
		{
			return BaseList.Any(x => Equals(x.Related, item));
		}
	}
}