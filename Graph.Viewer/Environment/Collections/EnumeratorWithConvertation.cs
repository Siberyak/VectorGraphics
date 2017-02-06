using System;
using System.Collections;
using System.Collections.Generic;

namespace KG.SE2.Utils.Collections
{
	internal class EnumeratorWithConvertation<TBase, TResult> : IEnumerator<TResult>
	{
		private readonly IEnumerator<TBase> _enumerator;
		private readonly Func<TBase, TResult> _baseToResult;
		public EnumeratorWithConvertation(IEnumerable<TBase> list, Func<TBase, TResult> baseToResult)
		{
			_baseToResult = baseToResult;
			_enumerator = list.GetEnumerator();
		}

		#region Implementation of IDisposable

		public void Dispose()
		{
			_enumerator.Dispose();
		}

		#endregion

		#region Implementation of IEnumerator

		public bool MoveNext()
		{
			return _enumerator.MoveNext();
		}

		public void Reset()
		{
			_enumerator.Reset();
		}

		public TResult Current
		{
			get { return _baseToResult(_enumerator.Current); }
		}

		object IEnumerator.Current
		{
			get { return Current; }
		}

		#endregion
	}
}