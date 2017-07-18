using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;

namespace Triarc
{
	class Program
	{
		static void Main(string[] args)
		{
			int b = Triarc.CreateBoundary(2, 2, 2).ToBoundary();
		}
	}

	static class BoundaryLong
	{
		/// <summary>
		/// Takes a boundary, that isn't represented properly and changes representation so that it is the greatest long value possible.
		/// </summary>
		/// <param name="l"></param>
		/// <returns></returns>
		public static long ToBoundary(this long l)
		{ throw new NotImplementedException(); }
	}

	


}
