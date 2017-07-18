using Microsoft.VisualStudio.TestTools.UnitTesting;
using Triarc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triarc.Tests
{
	[TestClass()]
	public class TriarcTests
	{
		[TestMethod()]
		public void CreateBoundaryTest()
		{
			int b = Triarc.CreateBoundary(2, 2, 2).ToBoundary();
			Assert.IsTrue( b== 0x888);
			b = Triarc.CreateBoundary(4, 4, 3).ToBoundary();
			Assert.IsTrue(b == 0x2a2a28);
		}
	}
}