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
			var tg = new TriarcGraph(4, 4, 3);
			var t = new Triarc(4,4,3, new int[2] { 5, 7 });
			t.Boundaries.Add(t.Boundary, t.Boundary);
			tg.StatesToGoTrough=t.MainFindTriarc();
			
			t.IsValid(1);


			Console.ReadKey();
		}
	}

	
	


}
