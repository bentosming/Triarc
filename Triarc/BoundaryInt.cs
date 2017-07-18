using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triarc
{
	public static class BoundaryInt
	{     /// <summary>
			/// Takes a boundary, that isn't represented properly and changes representation so that it is the greatest int value possible.
			/// </summary>
			/// <param name="l"></param>
			/// <returns></returns>
		public static int ToBoundary(this int l)
		{
			if (l < 0)
			{
				return ToBoundaryOnlyOneSide(l);
			}
			return ToBoundaryStandard(l);
		}

		private static int ToBoundaryOnlyOneSide(int l)
		{ throw new NotImplementedException(); }

		public static int ToBoundaryStandard(int l)
		{
			int highestValue = HighestSetBit(l);
			int highest = OrderOfHighestSetBit(l);
			int i = 0;
			int max = l;
			while (i <= highest && highest > 0)
			{
				int second = OrderOfHighestSetBit(l - highestValue);
				//	int secondValue = HighestSetBit(l - highestValue);
				l = ((l ^ highestValue) << (highest - second)) | (highestValue >> (second + 1));
				i += highest - second;
				if (l > max)
				{
					max = l;
				}
			}
			return max;

		}
		/// <summary>
		/// Takes non-negative int and returns value of highest set bit
		/// </summary>
		/// <param name="n"></param>
		/// <returns></returns>
		public static int HighestSetBit(int n)
		{
			n |= (n >> 1);
			n |= (n >> 2);
			n |= (n >> 4);
			n |= (n >> 8);
			n |= (n >> 16);
			return n - (n >> 1);
		}
		/// <summary>
		/// Takes positive int and returns order of highest bit set ( for 1 its 0).
		/// </summary>
		/// <param name="n"></param>
		/// <returns></returns>
		public static int OrderOfHighestSetBit(int n)
		{
			int result = 0;

			while (n != 1 && n != 0)
			{
				result++;
				n >>= 1;
			}
			return result;
		}

		public static int FaceToBoundary(int a)
		{
			int b = -1;
			for (int i = 0; i < a; i++)
			{
				b <<= 1;
				unchecked
				{
					b &= (int)0xFFFFFFFE;
				}
			}
			return b;
		}

	}
}
