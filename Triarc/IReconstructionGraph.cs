using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Triarc
{
	abstract class ReconstructionGraph
	{

		public string Name { get; set; }

		/// <summary>
		/// Sizes of faces aloved in triarc, sorted from least to greatest.
		/// </summary>
		public List<int> FaceSizes { get; set; }

		public List<VertexStack> vertices;

		public int ActiveRootID { get; set; }

		public int CountOfVertices;

		public int NumberOfVerticesInOuterBoundary;

		public List<List<int>> Faces { get; set; }


		/// <summary>
		/// Finds all vertices marked as active, that create a cycle with ActiveRootID vertex.
		/// </summary>
		/// <returns>vertices marked as active, that create a cycle with ActiveRootID vertex</returns>
		public List<VertexStack> ActiveVerticesOld()
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
				if (temp.A.IsActive && temp.B.IsActive && temp.C.IsActive) //nastavení prvního out vrcholu, který nesouseí s in vrcholy
				{
					if (!active.Contains(temp.A))
					{
						temp = temp.A;
					}
					else if (!active.Contains(temp.B))
					{
						temp = temp.B;
					}
					else
					{
						temp = temp.C;
					}
				}

				while (temp != active[0])
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
					if (temp.C.HasAllThreeNeighbours() && temp.C.IsActive && !active.Contains(temp.C))
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

		public List<VertexStack> ActiveVertices()
		{
			var allActive = this.vertices.Where(x => x.IsActive).ToList();
			
			Random rnd = new Random();
			if (allActive.Count==2)
			{
				throw new NotImplementedException();
			}
			while (true)
			{
				var result = tryActive(rnd, allActive.First());
				if (result.Count == allActive.Count && allActive.Aggregate(true, (containsAll, next) => containsAll && result.Contains(next)))
				{
					return result;
				}
			}
		}

		List<VertexStack> tryActive(Random rnd, VertexStack start)
		{
			var result = new List<VertexStack>() { start };
			result.Add(chooseRnd(start.ActiveNeighbours(), rnd));
			var next = chooseRnd(result[1].ActiveNeighbours(start.ID), rnd);
			while (next!=start)
			{
				var current = next;
				var last = result.Last();
				result.Add(next);
				next = chooseRnd(current.ActiveNeighbours(last.ID), rnd);
			}
			return result;
		}

		VertexStack chooseRnd(List<VertexStack> list, Random rnd)
		{
			return list[rnd.Next(list.Count)];
		}

		void EvaluateVertexPositions()
		{
			var vertices = this.vertices.OrderBy(x => x.ID).ToList();
			for (int i = 0; i < vertices.Count; i++)
			{
				var item = vertices[i];
				if (item.ID < NumberOfVerticesInOuterBoundary)
				{
					item.level = 0;
					item.angle = ((double)360 / NumberOfVerticesInOuterBoundary * item.ID) * Math.PI / 180;
					continue;
				}
				var ABC = item.ABC.OrderBy(x => x.ID).Where(x => x.ID < item.ID).ToList();
				if (ABC.Count == 0)
				{
					throw new NotImplementedException("tohle se nemělo stát - vrchol by vždy měl mít souseda s nižším id");
				}
				if (ABC.Count == 1)
				{
					item.angle = ABC.FirstOrDefault().angle;
					item.level = ABC.FirstOrDefault().level + 1;
				}
				if (ABC.Count == 2)
				{
					item.level = (ABC[0].level < ABC[1].level ? ABC[1].level : ABC[0].level)+1;
					item.angle = GetAngeFrom2(Math.Min(ABC[0].angle, ABC[1].angle), Math.Max(ABC[0].angle, ABC[1].angle));
				}
				if (ABC.Count==3)
				{
					item.level = Math.Min(Math.Min(ABC[0].level, ABC[1].level), ABC[1].level)+1;
					item.angle = GetAngeFrom2(Math.Min(ABC[0].angle, ABC[1].angle), Math.Max(ABC[0].angle, ABC[1].angle));
				}
			}
		}

		double GetAngeFrom2(double a, double b)
		{
			var adeg = a * (180.0 / Math.PI);
			var bdeg = b * (180.0 / Math.PI);
			//	return Math.Abs((b-a)/2) < Math.Abs((a-b)/2 + Math.PI) ? Math.Abs(( b-a)/2 + 360) % 360 : Math.Abs((b - a)/2 + 360) % 360;
			//	return (b - a) / 2 < (a+2 *Math.PI - b) / 2 ? (b - a) / 2 : (a - b) / 2 + 2*Math.PI;
			//var result = b - a < Math.PI ? (b + a) / 2 : ((a + 2 * Math.PI + b) / 2 > 0 ? (a + 2 * Math.PI + b) / 2 : (a + 2 * Math.PI + b) / 2 + 2 * Math.PI);
			var result = bdeg - adeg <= 180 ? (bdeg + adeg) / 2 : ((adeg + bdeg - 360) / 2 >= 0 ? (adeg + bdeg - 360) / 2 : (adeg + bdeg - 360) / 2 + 360);
		//	var resultDeg = result * (180.0 / Math.PI);
			return result*Math.PI/180;
		}

		/// <summary>
		/// Writes a GraphViz readable representation of triarc,
		/// </summary>
		/// <param name="tw">TextWriter to write to.</param>
		public void GWWrite(TextWriter tw)
		{
			EvaluateVertexPositions();
			int diameter = vertices.Select(x => x.level).OrderBy(x => -x).FirstOrDefault()+1;
			tw.WriteLine("graph G {");
			

			foreach (var item in vertices)
			{

				//outer boundary has fixed position on a circle and the circle is in bold
					if (item.ID < NumberOfVerticesInOuterBoundary)
					{
				//tw.WriteLine("//"+item.ToString());
					double sin = (Math.Sin(item.angle) * (diameter-item.level));
					double cos = (Math.Cos(item.angle) * (diameter-item.level));
					string tempSIN = string.Format("{0:0.00}", sin); //(int)sin + "." + (int)((sin - (int)sin) * 100);
					string SIN = tempSIN.Substring(0, tempSIN.Length - 3) + "." + tempSIN[tempSIN.Length - 2] + tempSIN[tempSIN.Length - 1];
					string tempCOS = string.Format("{0:0.00}", cos); //(int)cos + "." + (int)((cos - (int)cos) * 100);
					string COS = tempCOS.Substring(0, tempCOS.Length - 3) + "." + tempCOS[tempCOS.Length - 2] + tempCOS[tempCOS.Length - 1];

				

					tw.WriteLine(item.ID + " [ pos = \" " + COS + "," + SIN + "!\" ];");
				


					if (item.ID < item.A.ID && item.A.ID<this.CountOfVertices)
					{
						tw.WriteLine(item.ID + " -- " + item.A.ID + "[style=bold];");
					}
					if (item.ID < item.B.ID && item.B.ID < this.CountOfVertices)
					{
						tw.WriteLine(item.ID + " -- " + item.B.ID + "[style=bold];");
					}
				}
			//other vertices don't have fixed position.
				else
				{
					if (item.ID < item.A.ID && item.A.ID < this.CountOfVertices)
					{
						tw.WriteLine(item.ID + " -- " + item.A.ID + ";");
					}
					if (item.ID < item.B.ID && item.B.ID < this.CountOfVertices)
					{
						tw.WriteLine(item.ID + " -- " + item.B.ID + ";");
					}
			
				}
				if (item.C != null && item.ID < item.C.ID && item.C.ID < this.CountOfVertices)
				{
					tw.WriteLine(item.ID + " -- " + item.C.ID + ";");
				}


			}
	//		for (int i = 0; i < Faces.Count; i++)
	//		{
	//			foreach (var vertex in Faces[i])
	//			{
	//				tw.WriteLine(i + this.CountOfVertices + " -- " + vertex + ";");
	//			}
	//		}
			tw.WriteLine("}");
	//		foreach (var item in Faces)
	//		{
	//			foreach (var it in item)
	//			{
	//				tw.Write(it + "  ");
	//			}
	//
	//			tw.WriteLine("// ");
	//		}
		}

		public void SageMathScriptForTuttesEmbeddingWrite(TextWriter tw)
		{
			tw.WriteLine("G = Graph()");
			foreach (var item in vertices)
			{
				if (item.ID < item.A.ID)
				{
					tw.WriteLine("G.add_edge(" + item.ID + ", " + item.A.ID + ")");
				}
				if (item.ID < item.B.ID)
				{
					tw.WriteLine("G.add_edge(" + item.ID + ", " + item.B.ID + ")");
				}
				if (item.C != null && item.ID < item.C.ID)
				{
					tw.WriteLine("G.add_edge(" + item.ID + ", " + item.C.ID + ")");
				}
			}
			tw.WriteLine("outerSize=" + this.NumberOfVerticesInOuterBoundary);
			tw.WriteLine("countOfVertices=" + this.CountOfVertices);
			tw.WriteLine("#Now adding vertices for each face, so that the graph is 3-connected.");
			for (int i = 0; i < Faces.Count; i++)
			{
				foreach (var vertex in Faces[i])
				{
					tw.WriteLine("G.add_edge(" + (i + this.CountOfVertices) + ", " + vertex + ")");
				}
			}

			tw.WriteLine(@"def scaling(x,i,j,n):");
			tw.WriteLine(@"    return x*(n-j)");
			tw.WriteLine();
			tw.WriteLine(@"A = G.adjacency_matrix();");
			tw.WriteLine();
			tw.WriteLine(@"n = G.num_verts()");
			tw.WriteLine(@"Munscaled = matrix(QQ, n,n, lambda i,j:-A[i,j]/G.degree(i) if i>=outerSize else 0) + identity_matrix(n)");
			tw.WriteLine(@"M=matrix(QQ,n,n,lambda i,j: scaling(Munscaled[i,j],i,j,n) if i>j & i<countOfVertices else Munscaled[i,j])");
			tw.WriteLine(@"jednicky = matrix(QQ,n,1,lambda i,j: -1)");
			tw.WriteLine(@"soucty = M*jednicky");
			tw.WriteLine(@"for i in range (n-outerSize):");
			tw.WriteLine(@"    M[i+outerSize,i+outerSize]+=soucty[i+outerSize,0]");
			tw.WriteLine();
			tw.WriteLine(@"vy = matrix(QQ,n,1, lambda i,j:round(sin(i/outerSize*2*pi)*1000) if i<outerSize else 0)");
			tw.WriteLine(@"vx = matrix(QQ,n,1, lambda i,j:round(cos(i/outerSize*2*pi)*1000) if i<outerSize else 0)");
			tw.WriteLine(@"resX=M.inverse() * vx");
			tw.WriteLine(@"resY=M.inverse()*vy");
			tw.WriteLine(@"i=0");
			tw.WriteLine(@"d=[[resX[i],resY[i]] for i in range(n)]");
			tw.WriteLine();
			tw.WriteLine(@"G.graphplot(save_pos=True)");
			tw.WriteLine(@"dd=G.get_pos()");
			tw.WriteLine(@"");
			tw.WriteLine(@"for i in range(n):");
			tw.WriteLine(@"    dd[i]=[resX[i,0],resY[i,0]]");
			tw.WriteLine();
			tw.WriteLine(@"G.set_pos(dd)");
			tw.WriteLine(@"#removing vertices that were used to make graph 3-connected");
			tw.WriteLine(@"ran = range(countOfVertices, n)");
			tw.WriteLine(@"G.delete_vertices(ran)");
			tw.WriteLine(@"G.show()");

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
		/// Writes a BAT code that turns a "name".gv file into .png.
		/// </summary>
		/// <param name="textWriter">Probably a writer that represents a .BAT file</param>
		/// <param name="name"></param>
		public void BATWrite(TextWriter textWriter, string name)
		{
			textWriter.WriteLine("cd \"C:\\Users\\Zuzana\\Documents\\Visual Studio 2015\\Projects\\Triarc\\Triarc\\bin\\Debug\"");
			textWriter.WriteLine("  \"C:\\Program Files (x86)\\Graphviz2.38\\bin\\neato.exe\" -o \"grafy\\" + name + ".png\" -Tpng   \"grafy\\" + name + ".gv\"");
		}

	}

}

