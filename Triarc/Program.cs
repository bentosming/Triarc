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

			var t = new Triarc(2,2,4, new int[2] { 5, 7 });
			t.Boundaries.Add(t.Boundary, t.Boundary);
			t.MainFindTriarc();
			
			t.IsValid(1);


			Console.ReadKey();
		}
	}

	
	


}
