using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Triarc
{
	class TriarcGraph : ReconstructionGraph
	{

		/// <summary>
		/// Creates a graph representation of (x,y,z) triarc's outer boundary
		/// </summary>
		/// <param name="x">First number that determines triarc</param>
		/// <param name="y">Second number that determines triarc</param>
		/// <param name="z">Third number that determines triarc</param>
		/// <param name="faceSizes">Values that satisfy neccesary conditions. Must be sorted and have the greatest value last.</param>
		public TriarcGraph(int x, int y, int z, List<int> faceSizes)
		{
			this.FaceSizes = faceSizes;
			Name = "(" + x + "," + y + "," + z + ")";
			NumberOfVerticesInOuterBoundary = (x + y + z) * 2;
			CreateOuterBoundaryVertices(x, y, z);
			ActiveRootID = 0;

		}

		/// <summary>
		/// Creates and registeres vertices to represent outer boundary of (x,y,z) triarc.
		/// </summary>
		/// <param name="x">First number that determines triarc</param>
		/// <param name="y">Second number that determines triarc</param>
		/// <param name="z">Third number that determines triarc</param>
		void CreateOuterBoundaryVertices(int x, int y, int z)
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
				if (i % 2 == 1 || i == 0 || i == 2 * x || i == 2 * (x + y))
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


	/// <summary>
	/// Represents a vertex with up to three neighbours. 
	/// </summary>
	public class SingleVertex
	{
		public VertexStack A, B;
		private VertexStack c;
		public VertexStack C
		{
			get
			{
				return c;
			}
			set
			{

				//Prevents multiple edges and loops
				if ((B != null && A != null && value.ID != A.ID && value.ID != B.ID) || value.ID == -1)
				{
					c = value;
				}
				else
				{
					if (value.ID == -1)
					{
						throw new ArgumentException();
					}
				}
			}
		}

		/// <summary>
		/// Indicates if this vertex is on the boundary of triarc at the moment.
		/// </summary>
		public bool IsActive { get; set; }

		/// <summary>
		/// Finds next active vertex stack. (Each vertex has up to two active neighbours, so return value is uniquely defined.)
		/// </summary>
		/// <param name="predecessor">One of active neighbours, the other than will be returned</param>
		/// <returns>Active neighbour that isn't predecessor.</returns>
		public VertexStack Succescor(VertexStack predecessor)
		{

			if (A != null && A != predecessor && A.IsActive)
			{
				return A;
			}
			if (B != null && B != predecessor && B.IsActive)
			{
				return B;
			}
			if (C != null && C != predecessor && C.IsActive)
			{

				return C;
			}
			return null;
		}

		/// <summary>
		/// Clones this object.
		/// </summary>
		/// <returns>Clone of this object.</returns>
		public SingleVertex Clone()
		{
			var clone = new SingleVertex();
			clone.A = this.A;
			clone.B = this.B;
			clone.c = this.c;
			clone.IsActive = this.IsActive;
			return clone;
		}

		/// <summary>
		/// Returns "A + "A.ID"B + "B.ID"C + "C.ID" 
		/// </summary>
		/// <returns> "A + "A.ID"B + "B.ID"C + "C.ID" </returns>
		public override string ToString()
		{
			string neighbours = "";
			if (A != null)
			{
				neighbours += "    A " + A.ID;
			}
			if (B != null)
			{
				neighbours += "    B " + B.ID;
			}
			if (C != null)
			{
				neighbours += "    C " + C.ID;
			}
			return neighbours;
		}


	}

	/// <summary>
	/// Represents a vertex with stack, where previous versions can be kept.
	/// </summary>
	public class VertexStack
	{
		Stack<SingleVertex> stack;
		public int ID { get; set; }

		/// <summary>
		/// Creates empty VertexStack
		/// </summary>
		/// <param name="id">Vertex ID</param>
		public VertexStack(int id)
		{
			stack = new Stack<SingleVertex>();
			ID = id;
		}
		public VertexStack A
		{
			get
			{
				return stack.Peek().A;
			}
		}
		public VertexStack B
		{
			get
			{
				return stack.Peek().B;
			}
		}
		public VertexStack C
		{
			get
			{
				return stack.Peek().C;
			}
		}

		public IList<VertexStack> ABC
		{ get
			{
				return new List<VertexStack>() { this.A, this.B, this.C };
			} }

		/// <summary>
		/// Indicates if this vertex is on the boundary of triarc at the moment.
		/// </summary>
		public bool IsActive
		{
			get
			{
				return stack.Peek().IsActive;
			}
		}


		public List<VertexStack> ActiveNeighbours(int notThis = -5) //-5 is invalid
		{
			
				return this.ABC.Where(x => x!=null && x.IsActive &&  x.ID!=notThis).ToList();
			
		}

		/// <summary>
		/// Finds next active vertex stack. (Each vertex has up to two active neighbours, so return value is uniquely defined.)
		/// </summary>
		/// <param name="predecessor">One of active neighbours, the other than will be returned</param>
		/// <returns>Active neighbour that isn't predecessor.</returns>
		public VertexStack Succescor(VertexStack predecessor)
		{
			return stack.Peek().Succescor(predecessor);
		}
		public bool HasAllThreeNeighbours()
		{
			return A != null && B != null && C != null;
		}
		/// <summary>
		/// Inserts a vertex on top of stack.
		/// </summary>
		/// <param name="vertex"></param>
		public void Push(SingleVertex vertex)
		{
			stack.Push(vertex);
		}


		/// <summary>
		/// Removes and returns the vertex at the top of stack.
		/// </summary>
		/// <returns>Vertex at the top of stack </returns>
		public SingleVertex Pop()
		{
			return stack.Pop();
		}

		/// <summary>
		/// Returns the vertex at the top of stack without removing it.
		/// </summary>
		/// <returns>Vertex at the top of stack.</returns>
		public SingleVertex Peek()
		{
			return stack.Peek();
		}

		public override string ToString()
		{
			string neighbours = "";
			if (A != null)
			{
				neighbours += "    A " + A.ID;
			}
			if (B != null)
			{
				neighbours += "    B " + B.ID;
			}
			if (C != null)
			{
				neighbours += "    C " + C.ID;
			}
			return "ID " + ID + neighbours + " level " + level + " angle " + angle * (180.0 / Math.PI);
		}

		public int level;
		public double angle;
	}






	
}



