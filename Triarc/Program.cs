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

			var tg = new TriarcGraph(4,4,3, new List<int>{ 5, 7 });
			var t = new TriarcFinding(4,4,3, new int[2] { 5, 7 });
			
			//		t.BoundaryTransitions.Add(t.StartingBoundary, t.StartingBoundary);
		var tr = new TriarcReconstruction(tg, 100000);
			tr.SequenceOfStatesLeadingToResult = t.FindTriarc();

			tr.ReconstructTriarc("");
			
		


		//	Console.ReadKey();
		}
	}

	
	


}
