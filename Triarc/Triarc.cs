using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triarc
{
	public class Triarc
	{
		ImmutableDictionary<int,int> Boundaries = ImmutableDictionary.Create<int, int>();
		List<int> Faces;
		List<int> FacesSizes;
		int BiggestFace;
		string Name;
		int Boundary;
		public Triarc(int a, int b, int c, List<int> facesSizes)
		{
			this.Name = "(" + a + "," + b + "," + c + ",)";
			this.FacesSizes = facesSizes; 
			Boundary = CreateBoundary(a, b, c);
			BiggestFace = facesSizes.Last();
			Faces = new List<int>();
			foreach (var item in facesSizes)
			{
				Faces.Add(BoundaryInt.FaceToBoundary(item));
			}
		}
		public static int CreateBoundary(int a, int b, int c)
		{
			int Boundary = 0;
			for (int i = 2; i < (a + b + c) * 2; i++)
			{
				Boundary <<= 1;
				if ((i & 1) == 0 && i != a + a && i != a + a + b + b && i != 0)
				{
					Boundary |= 1;
				}

			}
			//první dva jsou 00 - je třeba je napsat na konec
			Boundary <<= 2;
			return Boundary;
		}
		public void FindTriarc(int b)
		{

		}
		/// <summary>
		/// pokud je za sebou počet nul delší než nejdelší strana, je nevalidní
		/// </summary>
		/// <param name="b"></param>
		/// <returns></returns>
		bool IsValid(int b)
		{
			while (b!=0)
			{

			}
		}
	}
}
