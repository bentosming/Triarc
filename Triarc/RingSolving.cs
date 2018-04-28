using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Triarc
{
	class RingSolving
	{
		public List<long> ring { get; set; }
		public RingSolving(int a, int b, int[] facesSizes, int limit)
		{
			var solving = new TriarcSolving(CreateOuterBoundaryOfBiarc(a, b), facesSizes);
			solving.TransitionLimit = limit;
			this.ring = solving.SolveTriarc();
		}
		

		static string FacesToString(IList<int> facesSizes)
		{
			StringBuilder temp = new StringBuilder();
			foreach (var item in facesSizes)
			{
				temp.Append(item + " ");
			}
			return temp.ToString();
		}

		public static void RingSolvingStatistic(int[] facesSizes, int limit, TextWriter writer)
		{
			var facesToString = FacesToString(facesSizes);
			for (int i = 3; i < 30; i++)
			{
				for (int j = 3; j < 30; j++)
				{
					if (i==j && i+j<32)
					{
						var solving = new RingSolving(i, j, facesSizes, limit);
						if (						solving.ring != null)
						{
							writer.WriteLine("(" + i + "," + j + ") with facesSizes " + facesToString + "exists");
							Console.WriteLine("Exists");
						}
						else
						{
							writer.WriteLine("(" + i + "," + j + ") with facesSizes " + facesToString + "doesn't exist");
							Console.WriteLine("Doesn't exist");
						}
						writer.Flush();

					}
				}
			}
			writer.Flush();
			writer.Close();
		}

		public static long CreateOuterBoundaryOfBiarc(int a, int b)
		{
			//Starting from first main vertex of triarc and adding 0 or 1 for each vertex. 
			long Boundary = 0;
			for (int i = 0; i < a; i++)
			{
				Boundary <<= 2;
				Boundary |= 2;
			}
			Boundary <<= 1;
			for (int i = 0; i < b; i++)
			{
				Boundary <<= 2;
				Boundary |= 2;
			}
			Boundary <<= 1;
			return Boundary.BoundaryToStandardizedForm();
		}
	}
}
