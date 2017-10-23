using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Triarc
{
	class TriarcGraph :IReconstructionGraph
	{
		
		public string Name { get;  set; }

		/// <summary>
		/// Sizes of faces aloved in triarc, sorted from least to greatest.
		/// </summary>
		public List<int> FaceSizes { get; set; } 

		public List<VertexStack> vertices;

		public int ActiveRootID { get; set; }

		public int CountOfVertices;

		int NumberOfVerticesInOuterBoundary;

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
			SingleVertex o = new TriarcGraph.SingleVertex();
			o.A = Outside;
			o.B = Outside;
			o.C = Outside;
			o.IsActive = false;
			Outside.Push(o);
			List<VertexStack> vrcholy = new List<VertexStack>();
			for (int i = 0; i < NumberOfVerticesInOuterBoundary; i++)
			{
				vrcholy.Add(new VertexStack(i));
				vrcholy[i].Push(new TriarcGraph.SingleVertex());
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

		/// <summary>
		/// Finds all vertices marked as active, that create a cycle with ActiveRootID vertex.
		/// </summary>
		/// <returns>vertices marked as active, that create a cycle with ActiveRootID vertex</returns>
		public List<VertexStack> ActiveVertices()
		{
			List<VertexStack> active = new List<VertexStack>();
			active.Add(vertices[ActiveRootID]);
			int i = 0;
			if (active.First().HasAllThreeNeighbours() && active.First().A.IsActive && active.First().B.IsActive && active.First().C.IsActive)
			{
				//nejprve přidej řetízek "in" vrcholů
				VertexStack temp = active.First().C;
				while (!temp.HasAllThreeNeighbours())
				{
					i++;
					active.Add(temp);
					temp = active[i].Succescor(active[i - 1]);
				}
				active.Add(temp);
				i++;
				if (temp.A.IsActive &&temp.B.IsActive && temp.C.IsActive) //nastavení prvního out vrcholu, který nesouseí s in vrcholy
				{
					if (! active.Contains(temp.A))
					{
						temp = temp.A;
					}
					else if (! active.Contains(temp.B))
					{
						temp = temp.B;
					}
					else
					{
						temp = temp.C;
					}
				}

				while (temp!=active[0])
				{
					i++;
					active.Add(temp);
					//temp = následník, který je aktivní a ještě není v active
					if (temp.A.IsActive && !active.Contains(temp.A))
					{
						temp = temp.A;
						continue;
					}
					if (temp.B.IsActive && !active.Contains(temp.B))
					{
						temp = temp.B;
						continue;
					}
					if (temp.C.HasAllThreeNeighbours()&& temp.C.IsActive && !active.Contains(temp.C))
					{
						temp = temp.C;
						continue;
					}
					//pokud žádné z předchozích nevyšlo, musel mít jen takové aktivní sousedy, které už jsou v active, tedy jeden z nich je první soused
					temp = active[0];
				}
				
				return active;
			}
			else
			{
				
				VertexStack temp = active[i].Succescor(active[i]);
				while (temp != active[0])
				{
					i++;
					active.Add(temp);
					temp = active[i].Succescor(active[i - 1]);
				}
				return active;
			}
		}

		/// <summary>
		/// Writes a BAT code that turns a "name".gv file into .png.
		/// </summary>
		/// <param name="textWriter">Probably a writer that represents a .BAT file</param>
		/// <param name="name"></param>

		public void BATWrite(TextWriter textWriter, string name)
		{
			textWriter.WriteLine("cd \"C:\\Users\\Zuzana\\Documents\\Visual Studio 2015\\Projects\\Triarc\\Triarc\\bin\\Debug\"");
			textWriter.WriteLine("  \"C:\\Program Files (x86)\\Graphviz2.38\\bin\\neato.exe\" -o \"grafy\\" + name + ".png\" -Tpng   \"grafy\\" + name + ".gv\"");
		}
		
		/// <summary>
		/// Writes a WolframAlpha readable representation of triarc. 
		/// That is edge is represented by VertexNumber -> VertexNumber and separated by columns.
		/// </summary>
		/// <param name="textWriter">TextWriter to write to.</param>
		public void WAWrite(TextWriter textWriter)
		{
		   foreach (var vertex in vertices)
			{
				if (vertex.ID < NumberOfVerticesInOuterBoundary)
				{
					//Adding loop to mark outer boundary
					textWriter.Write(vertex.ID + " -> " + vertex.ID + ", ");

					if (vertex.ID < vertex.A.ID)
					{
						
					}															 
					if (vertex.ID < vertex.B.ID)							 
					{															 
						textWriter.Write(vertex.ID + " -> " + vertex.B.ID + ", ");
					}															 
				}																 
				else															 
				{																 
					if (vertex.ID < vertex.A.ID)							 
					{															 
						textWriter.Write(vertex.ID + " -> " + vertex.A.ID + ", ");
					}
					if (vertex.ID < vertex.B.ID)
					{
						textWriter.Write(vertex.ID + " -> " + vertex.B.ID + ", ");
					}

				}
				if (vertex.C != null && vertex.ID < vertex.C.ID)
				{
					textWriter.Write(vertex.ID + " -> " + vertex.C.ID + ", ");
				}


			}
		}

		/// <summary>
		/// Writes a GraphViz readable representation of triarc,
		/// </summary>
		/// <param name="tw">TextWriter to write to.</param>
		public void GWWrite(TextWriter tw)
		{
			double diameter = Math.Sqrt(CountOfVertices);
			tw.WriteLine("graph G {");
			foreach (var item in vertices)
			{
				//outer boundary has fixed position on a circle and the circle is in bold
				if (item.ID < NumberOfVerticesInOuterBoundary)
				{
					double sin = (Math.Sin(((double)360 / NumberOfVerticesInOuterBoundary * item.ID) * Math.PI / 180) * diameter);
					double cos = (Math.Cos(((double)360 / NumberOfVerticesInOuterBoundary * item.ID) * Math.PI / 180) * diameter);
					string tempSIN = string.Format("{0:0.00}", sin); //(int)sin + "." + (int)((sin - (int)sin) * 100);
					string SIN = tempSIN.Substring(0, tempSIN.Length - 3) + "." + tempSIN[tempSIN.Length - 2] + tempSIN[tempSIN.Length - 1];
					string tempCOS = string.Format("{0:0.00}", cos); //(int)cos + "." + (int)((cos - (int)cos) * 100);
					string COS = tempCOS.Substring(0, tempCOS.Length - 3) + "." + tempCOS[tempCOS.Length - 2] + tempCOS[tempCOS.Length - 1];

					tw.WriteLine(item.ID + " [ pos = \" " + COS + "," + SIN + "!\" ];");


					if (item.ID < item.A.ID)
					{
						tw.WriteLine(item.ID + " -- " + item.A.ID + "[style=bold];");
					}
					if (item.ID < item.B.ID)
					{
						tw.WriteLine(item.ID + " -- " + item.B.ID + "[style=bold];");
					}
				}
				//other vertices don't have fixed position.
				else
				{
					if (item.ID < item.A.ID)
					{
						tw.WriteLine(item.ID + " -- " + item.A.ID + ";");
					}
					if (item.ID < item.B.ID)
					{
						tw.WriteLine(item.ID + " -- " + item.B.ID + ";");
					}

				}
				if (item.C != null && item.ID < item.C.ID)
				{
					tw.WriteLine(item.ID + " -- " + item.C.ID + ";");
				}


			}
			tw.WriteLine("}");
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
				return "ID " + ID + neighbours;
			}
		}

	}








	class StringStatesWithHasSetAndRedundants : IStates<string>, IStates
	{
		int numberOfDifferent = 0;
		HashSet<string> states = new HashSet<string>();

		public bool Add(IList<TriarcGraph.VertexStack> list)
		{
			return Add(VerticesToState(list));
		}

		public bool Add(string s)
		{
			if (states.Contains(s))
			{
				return false;
			}
			for (int i = 0; i < s.Length; i++)
			{
				states.Add(s);
				s = s.Substring(1, s.Length - 1) + s[0];
			}
			StringBuilder rSB = new StringBuilder();
			for (int i = s.Length - 1; i >= 0; i--)
			{
				rSB.Append(s[i]);
			}
			string r = rSB.ToString();
			for (int i = 0; i < r.Length; i++)
			{
				states.Add(r);
				r = r.Substring(1, r.Length - 1) + r[0];
			}
			numberOfDifferent++;
			return true;
		}

		public int Count()
		{
			return numberOfDifferent;
		}
		public string VerticesToString(IList<TriarcGraph.VertexStack> list)
		{
			return VerticesToState(list);
		}

		public string VerticesToState(IList<TriarcGraph.VertexStack> list)
		{

			Func<TriarcGraph.VertexStack, char> toState = x =>
			{
				if (x.HasAllThreeNeighbours())
				{
					return 'O';
				}
				return 'I';
			};
			StringBuilder sb = new StringBuilder(); ;
			foreach (var item in list)
			{
				sb.Append(toState(item));
			}
			return sb.ToString();
		}
	}


}



