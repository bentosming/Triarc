using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
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

		private static int ToBoundaryOnlyOneSide(int a)
		{ throw new NotImplementedException(); }

		//Efficiency lost, 2x aproximately
		public static IEnumerable<int> AllRepresentations(this int b)
		{
			int highestValue = HighestSetBit(b);
			int highest = OrderOfHighestSetBit(b);
			int i = 0;
			while (i <= highest && highest > 0)
			{
				yield return b;
				int second = OrderOfHighestSetBit(b - highestValue);
				//	int secondValue = HighestSetBit(l - highestValue);
				b = ((b ^ highestValue) << (highest - second)) | (highestValue >> (second + 1));
				i += highest - second;
			}
			yield return b;
		}
		//May seem more elegant, but it not efficient enough
		public static int ToBoundaryStandard2(int a)
		{
			var E = a.AllRepresentations();
			int max = E.First();

			foreach (var item in E)
			{
				if (item>max)
				{
					max = item;
				}
			}
			return max;
		}
		public static int ToBoundaryStandard(int a)
		{
			int highestValue = HighestSetBit(a);
			int highest = OrderOfHighestSetBit(a);
			int i = 0;
			int max = a;
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
		public static int RotateRight(this int l, int highest,int highestValue)
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
		public static int HighestSetBit(this int n)
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
		public static int OrderOfHighestSetBit(this int n)
		{
			if (n==0)
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

		public static int FaceToBoundary(int a)
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
