using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triarc
{
	public static class BoundaryLong
	{
		const int LengthOfStruct = 64;
		/// <summary>
		/// Takes a boundary, that isn't represented properly and changes representation so that it is the greatest int value possible.
		/// </summary>
		/// <param name="l"></param>
		/// <returns></returns>
		public static long ToBoundary(this long l)
		{

			if (l < 0)
			{
				return ToBoundaryOnlyOneSide(l);
			}
			return ToBoundaryStandard(l);
		}
		static long[] SetBits = new long[LengthOfStruct]
		{ 0x1, 0x2, 0x4, 0x8 ,
		0x10, 0x20, 0x40, 0x80,
		0x100, 0x200, 0x400, 0x800,
		0x1000, 0x2000, 0x4000, 0x8000 ,
		0x00010000, 0x00020000, 0x00040000, 0x00080000,
		0x00100000, 0x00200000, 0x00400000, 0x00800000,
		0x01000000, 0x02000000, 0x04000000, 0x08000000,
		0x10000000, 0x20000000, 0x40000000, 0x80000000,
		0x100000000, 0x200000000, 0x400000000, 0x800000000 ,
		0x1000000000, 0x2000000000, 0x4000000000, 0x8000000000,
		0x10000000000, 0x20000000000, 0x40000000000, 0x80000000000,
		0x100000000000, 0x200000000000, 0x400000000000, 0x800000000000 ,
		0x0001000000000000, 0x0002000000000000, 0x0004000000000000, 0x0008000000000000,
		0x0010000000000000, 0x0020000000000000, 0x0040000000000000, 0x0080000000000000,
		0x0100000000000000, 0x0200000000000000, 0x0400000000000000, 0x0800000000000000,
		0x1000000000000000, 0x2000000000000000, 0x4000000000000000, long.MinValue};

		private static long ToBoundaryOnlyOneSide(long a)
		{ throw new NotImplementedException(); }

	


		public static long ToBoundaryStandard(long a)
		{
			if (a == 0)
			{
				return 0;
			}
			int highest = OrderOfHighestSetBit(a);
			long highestValue = SetBits[highest];
			int i = 0;
			long max = a;
			while (i <= highest && highest > 0)
			{
				int second = OrderOfHighestSetBit(a - highestValue);
				//	int secondValue = HighestSetBit(l - highestValue);
				a = ((a ^ highestValue) << (highest - second)) | (highestValue >> (second + 1));
				i += highest - second;
				if (a > max)
				{
					max = a;
				}
			}
			//ještě verze "čtení z druhé strany"
			long aReversed = 0;
			for (int j = 0; j <= highest; j++)
			{
				aReversed <<= 1;
				aReversed |= (a & 1);
				a >>= 1;
			}
			int reversedHighestBit = aReversed.OrderOfHighestSetBit();
			aReversed <<= (highest - reversedHighestBit);
			a = aReversed;
			highest = OrderOfHighestSetBit(a);
			highestValue = SetBits[highest];
			i = 0;
			while (i <= highest && highest > 0)
			{
				int second = OrderOfHighestSetBit(a - highestValue);
				//	int secondValue = HighestSetBit(l - highestValue);
				a = ((a ^ highestValue) << (highest - second)) | (highestValue >> (second + 1));
				i += highest - second;
				if (a > max)
				{
					max = a;
				}
			}
			return max;
		}

		/// <summary>
		/// rotates l so that second highest set bit is first
		/// </summary>
		/// <param name="l"></param>
		/// <param name="highest"></param>
		/// <param name="highestValue"></param>
		/// <returns></returns>
		public static long RotateRight(this long l, int highest, long highestValue)
		{
			int second = OrderOfHighestSetBit(l - highestValue);
			//	int secondValue = HighestSetBit(l - highestValue);
			return ((l ^ highestValue) << (highest - second)) | (highestValue >> (second + 1));
		}

		/// <summary>
		/// Takes non-negative int and returns value of highest set bit
		/// </summary>
		/// <param name="n"></param>
		/// <returns></returns>
		public static long HighestSetBit(this long n)
		{
			n |= (n >> 1);
			n |= (n >> 2);
			n |= (n >> 4);
			n |= (n >> 8);
			n |= (n >> 16);
			n |= (n >> 32);
			return n - (n >> 1);
		}
		/// <summary>
		/// Takes positive int and returns order of highest bit set ( for 1 its 0).
		/// </summary>
		/// <param name="n"></param>
		/// <returns></returns>
		public static int OrderOfHighestSetBit(this long n)
		{
			if (n < 0)
			{
				return LengthOfStruct - 1;
			}
			if (n == 0)
			{
				return -1;
			}
			int result = 0;

			while (n != 1 && n != 0)
			{
				result++;
				n >>= 1;
			}
			return result;
		}

		public static long FaceToBoundary(int a)
		{
			int b = -1;
			for (int i = 0; i < a; i++)
			{
				b <<= 1;

			}
			return b;
		}


		public static void ToBoundaryStandardPerformaceTest()
		{

			BoundaryInt.ToBoundaryStandard(1);
			var stopWatch = new Stopwatch();
			stopWatch.Start();
			for (int i = 1; i < 0xFFFFFFF; i++)
			{
				BoundaryInt.ToBoundaryStandard(i);

			}
			stopWatch.Stop();
			Console.WriteLine("Time elapsed when using ToBoundaryStandard  " + stopWatch.Elapsed);

			BoundaryInt.ToBoundaryStandard2(1);
			stopWatch = new Stopwatch();
			stopWatch.Start();
			for (int i = 1; i < 0xFFFFFFF; i++)
			{
				BoundaryInt.ToBoundaryStandard2(i);
			}
			stopWatch.Stop();
			Console.WriteLine("Time elapsed when using ToBoundaryStandard2  " + stopWatch.Elapsed);
		}
	}
}
