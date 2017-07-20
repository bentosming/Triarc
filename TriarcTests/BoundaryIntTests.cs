using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Triarc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triarc.Tests
{
	[TestClass()]
	public class BoundaryIntTests
	{
		[TestMethod()]
		public void HighestSetBitTest()
		{
			Assert.IsTrue(BoundaryInt.HighestSetBit(0) == 0);
			Assert.IsTrue(BoundaryInt.HighestSetBit(1) == 1);
			Assert.IsTrue(BoundaryInt.HighestSetBit(2) == 2);
			Assert.IsTrue(BoundaryInt.HighestSetBit(11) == 8);
			Assert.IsTrue(BoundaryInt.HighestSetBit(672) == 512);
			Assert.IsTrue(BoundaryInt.HighestSetBit(0xfffffff) == 0x8000000);
		}
		[TestMethod()]
		public void OrderOfHighestSetBitTest()
		{
			Assert.IsTrue(BoundaryInt.OrderOfHighestSetBit(1) == 0);
			Assert.IsTrue(BoundaryInt.OrderOfHighestSetBit(2) == 1);
			Assert.IsTrue(BoundaryInt.OrderOfHighestSetBit(11) == 3);
			Assert.IsTrue(BoundaryInt.OrderOfHighestSetBit(672) == 9);
			Assert.IsTrue(BoundaryInt.OrderOfHighestSetBit(0xfffffff) == 27);
		}

		[TestMethod()]
		public void ToBoundaryStandardTest()
		{
			Assert.IsTrue(BoundaryInt.ToBoundaryStandard(1) == 1);
			Assert.IsTrue(BoundaryInt.ToBoundaryStandard(2) == 2);
			Assert.IsTrue(BoundaryInt.ToBoundaryStandard(0) == 0);
			Assert.IsTrue(BoundaryInt.ToBoundaryStandard(74) == 84);
			Assert.IsTrue(BoundaryInt.ToBoundaryStandard(855) == 1002);
			Assert.IsTrue(BoundaryInt.ToBoundaryStandard(0x72) == 0x74);

		}



		[TestMethod()]
		public void FaceToBoundaryTest()
		{
			Assert.IsTrue(BoundaryInt.FaceToBoundary(0) == -1);
			Assert.IsTrue(BoundaryInt.FaceToBoundary(1) == -2);
			Assert.IsTrue(BoundaryInt.FaceToBoundary(2) == -4);
			Assert.IsTrue(BoundaryInt.FaceToBoundary(5) == -32);

		}

		public void FaceToBoundaryTest1()
		{

		}
	}
}