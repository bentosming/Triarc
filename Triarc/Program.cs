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
			var tg = new TriarcGraph(10,10,10);
			var t = new Triarc(10,10,10, new int[2] { 5, 7 });
			t.Boundaries.Add(t.Boundary, t.Boundary);
		tg.StatesToGoTrough=t.MainFindTriarc();
			var tr = new TriarcReconstruction(tg, 100000);
			tr.ReconstructTriarc("");
			
		


		//	Console.ReadKey();
		}
	}

	
	


}
