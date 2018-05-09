using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triarc
{
	class ArcGraph : ReconstructionGraph
	{
		public ArcGraph(long boundary, string name, List<int> faceSizes)
		{
			this.FaceSizes = faceSizes;
			Name = name;
			NumberOfVerticesInOuterBoundary = (int) boundary.OrderOfHighestSetBit()+1;
			CreateOuterBoundaryVertices(boundary);
			ActiveRootID = 0;
		}

		public void CreateOuterBoundaryVertices(long boundary)
		{

			VertexStack Outside = new VertexStack(-1);
			SingleVertex o = new SingleVertex();
			o.A = Outside;
			o.B = Outside;
			o.C = Outside;
			o.IsActive = false;
			Outside.Push(o);
			List<VertexStack> vrcholy = new List<VertexStack>();
			for (int i = 0; i < NumberOfVerticesInOuterBoundary; i++)
			{
				vrcholy.Add(new VertexStack(i));
				vrcholy[i].Push(new SingleVertex());
			}
			//praví sousedé + aktivní
			for (int i = 0; i < NumberOfVerticesInOuterBoundary; i++)
			{
				SingleVertex temp = vrcholy[i].Pop();
				temp.IsActive = true;
				temp.A = vrcholy[(i + 1) % NumberOfVerticesInOuterBoundary];
				vrcholy[i].Push(temp);
			}
			//leví sousedé
			for (int i = 1; i < NumberOfVerticesInOuterBoundary + 1; i++)
			{
				SingleVertex temp = vrcholy[i % NumberOfVerticesInOuterBoundary].Pop();
				temp.B = vrcholy[(i - 1) % NumberOfVerticesInOuterBoundary];
				vrcholy[i % NumberOfVerticesInOuterBoundary].Push(temp);
			}
			for (int i = 0; i < NumberOfVerticesInOuterBoundary; i++)
			{
				if ((boundary & ((long)1<<i))==0)
				{
					SingleVertex temp = vrcholy[i].Pop();
					temp.C = Outside;
					vrcholy[i].Push(temp);
				}
			}
			this.vertices = vrcholy;
			this.CountOfVertices = vertices.Count;

		}
	}
}
