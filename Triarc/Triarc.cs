using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Text;


namespace Triarc
{
	class Triarc
	{
		/// <summary>
		/// Determines whether a (x,y,z) triarc with given faceSizes exists (active boundary is never greater than 63)
		/// and exports graph to grafy folder.
		/// </summary>
		/// <param name="x">First number that determines triarc.</param>
		/// <param name="y">Second number that determines triarc.</param>
		/// <param name="z">Third number that determines triarc.</param>
		/// <param name="facesSizes">Allowed sizes of faces for triarc.</param>
		/// <returns>True if such triarc exist with activ boundary of maximum length 63.</returns>
		public static bool FindAndBuildTriarc(int x, int y, int z, List<int> facesSizes)
		{
			var triarcGraph = new TriarcGraph(4, 4, 3, facesSizes);
			var solving = new TriarcSolving(4, 4, 3, facesSizes.ToArray());

			var reconstruction = new TriarcReconstruction(triarcGraph, solving.SolveTriarc());

			return reconstruction.ReconstructTriarc();
		}

		/// <summary>
		/// Determines whether a (x,y,z) triarc with given faceSizes exists (active boundary is never greater than 63)
		/// and exports graph to grafy folder.
		/// </summary>
		/// <param name="x">First number that determines triarc.</param>
		/// <param name="y">Second number that determines triarc.</param>
		/// <param name="z">Third number that determines triarc.</param>
		/// <param name="facesSizes">Allowed sizes of faces for triarc.</param>
		/// <param name="limit">Number of states to find befor giving up.</param>
		/// <returns></returns>
		public static bool DoesTriarcExist(int x, int y, int z, IList<int> facesSizes, int limit)
		{
			var solving = new TriarcSolving(x, y, z, facesSizes.ToArray());
			solving.TransitionLimit = limit;
			return solving.SolveTriarc() != null;
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

		/// <summary>
		/// Goes trough all triarc with outter boarder of length 63 and less and determines, if they can be found within limit.
		/// </summary>
		/// <param name="textWriter">Results are written here.</param>
		/// <param name="facesSizes">Type of triarcs.</param>
		/// <param name="limit">Number of states to discover (per triarc) before giving up on a triarc.</param>
		public static void DoTriarcsExist(TextWriter textWriter, IList<int> facesSizes, int limit)
		{
			var facesToString = FacesToString(facesSizes);
			for (int x = 2; x < 30; x++)
			{
				for (int y = 2; y < 30; y++)
				{
					if (x >= y && x + y < 32)

						for (int z = 2; z < 30; z++)
						{
							if (y >= z && x + y + z < 32)
							{
								Console.Write("(" + x + "," + y + "," + z + ") with facesSizes " + facesToString);
								if (DoesTriarcExist(x, y, z, facesSizes, limit))
								{
									textWriter.WriteLine("(" + x + "," + y + "," + z + ") with facesSizes " + facesToString + "exists");
									Console.WriteLine("Exists");
								}
								else
								{
									textWriter.WriteLine("(" + x + "," + y + "," + z + ") with facesSizes " + facesToString + "doesn't exists");
									Console.WriteLine("Doesn't exists");
								}

							}
						}


				}
			}
		}




	}


	
}