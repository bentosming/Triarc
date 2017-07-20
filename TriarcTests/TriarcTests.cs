using Microsoft.VisualStudio.TestTools.UnitTesting;
using Triarc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;

namespace Triarc.Tests
{
	[TestClass()]
	public class TriarcTests
	{
		[TestMethod()]
		public void CreateBoundaryTest()
		{
			long b = Triarc.CreateBoundary(2, 2, 2);
			Assert.IsTrue(b == 0x888);
			b = Triarc.CreateBoundary(4, 4, 3);
			Assert.IsTrue(b == 0x2a2a28);
		}

		[TestMethod()]
		public void IsValidTest()
		{
			var t = new Triarc(4, 4, 3, new int[2] { 5, 7 });

			Assert.IsTrue(t.IsValid(BoundaryInt.FaceToBoundary(5)));
			Assert.IsTrue(t.IsValid(BoundaryInt.FaceToBoundary(7)));
			Assert.IsFalse(t.IsValid(BoundaryInt.FaceToBoundary(8)));

			Assert.IsTrue(t.IsValid(Triarc.CreateBoundary(2, 3, 4)));
			//1 1010 1000 1000 0001
			Assert.IsTrue(t.IsValid(0x1a881));
			//1000 0000 1010 1000 1000 0001
			Assert.IsFalse(t.IsValid(0x80a881));
		}

		[TestMethod()]
		public void SubstituteAndStartTaskForTwoBitsSetTest()
		{
			var t = new Triarc(4, 4, 3, new int[2] { 5, 7 });
			t.SubstituteAndStartTaskForTwoBitsSet(6,2,1);
			Assert.IsTrue(t.Boundaries.Contains(0x38, 6)); //přidání 5 mezi 101
			Assert.IsTrue(t.Boundaries.Contains(0xF8, 6)); //přidání 7 mezi 101
			Assert.IsTrue(t.Boundaries.Contains(0x0C, 6)); //přidání 5 mezi 11
			Assert.IsTrue(t.Boundaries.Contains(0x3C, 6)); //přidání 7 mezi 11
			//Assert.IsTrue(t.Boundaries.Count == 4);
			t.SubstituteAndStartTaskForTwoBitsSet(0x220,9,5);//10 0010 0000
			Assert.IsTrue(t.Boundaries.Contains(BoundaryInt.FaceToBoundary(5), 0x220) || t.Boundaries.Contains(BoundaryInt.FaceToBoundary(7), 0x220));
		}
	}
}