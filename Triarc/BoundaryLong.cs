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
		/// <summary>
		/// Number of bits in long
		/// </summary>
		const int LengthOfStruct = 64;

		/// <summary>
		/// Takes a long, which represents a boundary and transforms it into its unique form.
		/// Unique form of boundary is the one, that represents the same boundary and has the greatest numerical value. Unique form of one-faced n-long boundary is all bits 
		/// </summary>
		/// <param name="l"></param>
		/// <returns></returns>
		public static long BoundaryToStandardizedForm(this long boundaryToBeStandardized)
		{

			if (boundaryToBeStandardized < 0)
			{
				return ToStandardizedFormOnlyOneFace(boundaryToBeStandardized);
			}
			return ToStandardizedFormMoreFaces(boundaryToBeStandardized);
		}

		/// <summary>
		/// SetBits array allows direct acces to n-th bit.
		/// </summary>
		static long[] SetBits = new long[LengthOfStruct]
		{
		0x1, 0x2, 0x4, 0x8 ,
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

		/// <summary>
		/// Knowing the project never generates a one-faced boundary representation that isn't standardized, this returns its parameter.
		/// In the case that previous doesn't hold, some adequate control has to be added.
		/// </summary>
		/// <param name="oneFacedBoundaryToBeStandardized"></param>
		/// <returns></returns>
		private static long ToStandardizedFormOnlyOneFace(long oneFacedBoundaryToBeStandardized)
		{
			return oneFacedBoundaryToBeStandardized;
		}

		/// <summary>
		/// Takes a more-than-one-faced boundary and returns its standardized form which is the one from those that represents the same boundary that has the greates numeric value.
		/// For zero returns zero.
		/// For other non supported input returns eighter nonsense or crushes.
		/// </summary>
		/// <param name="moreThanOneFacedBoundaryToBeStandardized"></param>
		/// <returns></returns>
		public static long ToStandardizedFormMoreFaces(long moreThanOneFacedBoundaryToBeStandardized)
		{
			//not exactly a "moreThanOneFacedBoundary" but still useful
			if (moreThanOneFacedBoundaryToBeStandardized == 0)
			{
				return 0;
			}

			int orderOfHighestBitSet = OrderOfHighestSetBit(moreThanOneFacedBoundaryToBeStandardized);
			long onlyHighestBitSet = SetBits[orderOfHighestBitSet];

			//sums lengths of rotations done, so that it can determine that the boundary has been fully rotated
			int togetherRotatedBy = 0;
			
			//Since a maximal value of all acceptable is searched for, it will be represented by maximalValueFound and inicializated by the first acceptable.
			long maximalValueFound = moreThanOneFacedBoundaryToBeStandardized;

			while (togetherRotatedBy <= orderOfHighestBitSet && orderOfHighestBitSet > 0)
			{
				int orderOfSecondHighestBitSet = OrderOfHighestSetBit(moreThanOneFacedBoundaryToBeStandardized - onlyHighestBitSet);
				
				//unsets highest bit set, shifts left, so that the second highest bit set is on the position of first and sets bit that responds to the original highest
				moreThanOneFacedBoundaryToBeStandardized = ((moreThanOneFacedBoundaryToBeStandardized ^ onlyHighestBitSet) << (orderOfHighestBitSet - orderOfSecondHighestBitSet)) | (onlyHighestBitSet >> (orderOfSecondHighestBitSet + 1));

				togetherRotatedBy += orderOfHighestBitSet - orderOfSecondHighestBitSet;

				if (moreThanOneFacedBoundaryToBeStandardized > maximalValueFound)
				{
					maximalValueFound = moreThanOneFacedBoundaryToBeStandardized;
				}
			}

			//To really find all boundary representations, it is neccesary to "read it backwards" (equivalent to clockwise and anticlock wise reading graphical representation).
		
			//reversing 
			long reversedBoundaryToBeStandardized = 0;
			for (int j = 0; j <= orderOfHighestBitSet; j++)
			{
				reversedBoundaryToBeStandardized <<= 1;
				reversedBoundaryToBeStandardized |= (moreThanOneFacedBoundaryToBeStandardized & 1);
				moreThanOneFacedBoundaryToBeStandardized >>= 1;
			}
			int reversedHighestBit = reversedBoundaryToBeStandardized.OrderOfHighestSetBit();
			reversedBoundaryToBeStandardized <<= (orderOfHighestBitSet - reversedHighestBit);

			//rotating reversed just as above the original
			moreThanOneFacedBoundaryToBeStandardized = reversedBoundaryToBeStandardized;
			orderOfHighestBitSet = OrderOfHighestSetBit(moreThanOneFacedBoundaryToBeStandardized);
			onlyHighestBitSet = SetBits[orderOfHighestBitSet];
			togetherRotatedBy = 0;
			while (togetherRotatedBy <= orderOfHighestBitSet && orderOfHighestBitSet > 0)
			{
				int orderOfSecondHighestBitSet = OrderOfHighestSetBit(moreThanOneFacedBoundaryToBeStandardized - onlyHighestBitSet);
				moreThanOneFacedBoundaryToBeStandardized = ((moreThanOneFacedBoundaryToBeStandardized ^ onlyHighestBitSet) << (orderOfHighestBitSet - orderOfSecondHighestBitSet)) | (onlyHighestBitSet >> (orderOfSecondHighestBitSet + 1));
				togetherRotatedBy += orderOfHighestBitSet - orderOfSecondHighestBitSet;
				if (moreThanOneFacedBoundaryToBeStandardized > maximalValueFound)
				{
					maximalValueFound = moreThanOneFacedBoundaryToBeStandardized;
				}
			}

			return maximalValueFound;
		}


		/// <summary>
		/// Takes non-negative long and returns value of highest bit set.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static long HighestSetBit(this long value)
		{
			//sets all bits lower that the highest and then unsets all but the highests
			value |= (value >> 1);
			value |= (value >> 2);
			value |= (value >> 4);
			value |= (value >> 8);
			value |= (value >> 16);
			value |= (value >> 32);
			return value - (value >> 1);
		}

		/// <summary>
		/// Takes positive int and returns order of highest bit set. Lowest bit's order is 0 and highest's is 63.
		/// For zero returns -1.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static int OrderOfHighestSetBit(this long value)
		{
			if (value < 0)
			{
				return LengthOfStruct - 1;
			}
			if (value == 0)
			{
				return -1;
			}
			int result = 0;

			//Counted lineraly to LengthOfStruct by shifting by one until only last bit is set.
			while (value != 1 && value != 0)
			{
				result++;
				value >>= 1;
			}

			return result;
		}

		/// <summary>
		/// Takes length of single-faced boundary and returns its standardized representation.
		/// </summary>
		/// <param name="lengthOfSingleFacedBoundary"></param>
		/// <returns>Standardized boundary form of face</returns>
		public static long FaceToBoundary(int lengthOfSingleFacedBoundary)
		{
			//initializes to all bits set and then add zeros to low bits (as many as lenght of face)
			int representation = -1;
			representation <<= lengthOfSingleFacedBoundary;

			return representation;
		}
		
	}
}
