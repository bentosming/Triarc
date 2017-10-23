using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triarc
{
	interface IReconstructionGraph
	{
		string Name { get; set; }
		List<int> FaceSizes { get; set; }
		List<VertexStack> vertices { get; set; }

		int ActiveRootID { get; set; }

		int CountOfVertices { get; set; }

		int NumberOfVerticesInOuterBoundary { get; set; }
	}
}
