using System;
using System.Collections.Generic;

namespace Peregrine.Web.Services
{
	class TupleEqualityComparer<T1, T2> : EqualityComparer<Tuple<T1, T2>>
	{
		readonly IEqualityComparer<T1> T1Comparer;
		readonly IEqualityComparer<T2> T2Comparer;

		public TupleEqualityComparer(IEqualityComparer<T1> t1Comparer = null, IEqualityComparer<T2> t2Comparer = null)
		{
			T1Comparer = t1Comparer ?? EqualityComparer<T1>.Default;
			T2Comparer = t2Comparer ?? EqualityComparer<T2>.Default;
		}

		public override bool Equals(Tuple<T1, T2> x, Tuple<T1, T2> y)
		{
			if(Object.ReferenceEquals(x, y))
				return true;

			// Object.ReferenceEquals() will be true if x and y are null. This catches the case where one is null.
			if(x == null || y == null)
				return false;

			return T1Comparer.Equals(x.Item1, y.Item1) && T2Comparer.Equals(x.Item2, y.Item2);
		}

		public override int GetHashCode(Tuple<T1, T2> obj)
		{
			if(obj == null)
				return 0;

			return T1Comparer.GetHashCode(obj.Item1) ^ T2Comparer.GetHashCode(obj.Item2);
		}
	}
}