using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Triarc
{
	class TriarcGraph
	{
		public List<long> StatesToGoTrough = new List<long>();
		public string Name;
		public List<int> PolygonSizes = new List<int> { 5, 7 }; //setřízený od nejmenšího po největší
		public List<VertexStack> vertices;
		public int ActiveRootID;
		public int Count;
		public int Circumference;
		public TriarcGraph(int x, int y, int z)
		{
			Name = "(" + x + "," + y + "," + z + ")";
			Circumference = (x + y + z) * 2;
			VertexStack Outside = new VertexStack(-1);
			Vertex o = new Vertex();
			o.A = Outside;
			o.B = Outside;
			o.C = Outside;
			o.Active = false;
			Outside.Push(o);
			List<VertexStack> vrcholy = new List<VertexStack>();
			for (int i = 0; i < Circumference; i++)
			{
				vrcholy.Add(new VertexStack(i));
				vrcholy[i].Push(new Vertex());
			}
			//praví sousedé + aktivní
			for (int i = 0; i < Circumference; i++)
			{
				Vertex temp = vrcholy[i].Pop();
				temp.Active = true;
				temp.A = vrcholy[(i + 1) % Circumference];
				vrcholy[i].Push(temp);
			}
			//leví sousedé
			for (int i = 1; i < Circumference + 1; i++)
			{
				Vertex temp = vrcholy[i % Circumference].Pop();
				temp.B = vrcholy[(i - 1) % Circumference];
				vrcholy[i % Circumference].Push(temp);
			}
			for (int i = 0; i < Circumference; i++)
			{
				if (i % 2 == 1 || i == 0 || i == 2 * x || i == 2 * (x + y))
				{
					Vertex temp = vrcholy[i].Pop();
					temp.C = Outside;
					vrcholy[i].Push(temp);
				}
			}
			this.vertices = vrcholy;
			this.Count = vertices.Count;
			ActiveRootID = 0;

		}
		public TriarcGraph(List<VertexStack> vertices)
		{
			this.vertices = vertices;
			this.Count = vertices.Count;
			ActiveRootID = 0;
		}

		public List<VertexStack> ActiveVertices()
		{
			List<VertexStack> active = new List<VertexStack>();
			active.Add(vertices[ActiveRootID]);
			int i = 0;
			if (active.First().HasAllThreeNeighbours() && active.First().A.Active && active.First().B.Active && active.First().C.Active)
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
				if (temp.A.Active &&temp.B.Active && temp.C.Active) //nastavení prvního out vrcholu, který nesouseí s in vrcholy
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
					if (temp.A.Active && !active.Contains(temp.A))
					{
						temp = temp.A;
						continue;
					}
					if (temp.B.Active && !active.Contains(temp.B))
					{
						temp = temp.B;
						continue;
					}
					if (temp.C.HasAllThreeNeighbours()&& temp.C.Active && !active.Contains(temp.C))
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

		public void BATWrite(TextWriter tw, string name)
		{
			tw.WriteLine("cd \"C:\\Users\\Zuzana\\Documents\\Visual Studio 2015\\Projects\\Triarc\\Triarc\\bin\\Debug\"");
			tw.WriteLine("  \"C:\\Program Files (x86)\\Graphviz2.38\\bin\\neato.exe\" -o \"grafy\\" + name + ".png\" -Tpng   \"grafy\\" + name + ".gv\"");
		}

		public void WAWrite(TextWriter tw)
		{

			
			foreach (var item in vertices)
			{
				if (item.ID < Circumference)
				{
					
					if (item.ID < item.A.ID)
					{
						tw.Write(item.ID + " -> " + item.A.ID + ", ");
					}															 
					if (item.ID < item.B.ID)							 
					{															 
						tw.Write(item.ID + " -> " + item.B.ID + ", ");
					}															 
				}																 
				else															 
				{																 
					if (item.ID < item.A.ID)							 
					{															 
						tw.Write(item.ID + " -> " + item.A.ID + ", ");
					}
					if (item.ID < item.B.ID)
					{
						tw.Write(item.ID + " -> " + item.B.ID + ", ");
					}

				}
				if (item.C != null && item.ID < item.C.ID)
				{
					tw.Write(item.ID + " -> " + item.C.ID + ", ");
				}


			}
		}
	
		public void GWWrite(TextWriter tw)
		{
			double diameter = Math.Sqrt(Count);
			tw.WriteLine("graph G {");
			foreach (var item in vertices)
			{
				if (item.ID < Circumference)
				{
					double sin = (Math.Sin(((double)360 / Circumference * item.ID) * Math.PI / 180) * diameter);
					double cos = (Math.Cos(((double)360 / Circumference * item.ID) * Math.PI / 180) * diameter);
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
	}


	interface IStates<T>
	{
		T VerticesToState(IList<VertexStack> list);
		/// <summary>
		/// Adds state, returns false if state has already been in.
		/// </summary>
		/// <param name="item"></param> 
		/// <returns></returns>
		bool Add(T item);
		int Count();
		bool Add(IList<VertexStack> list);
		string VerticesToString(IList<VertexStack> list);
	}
	interface IStates
	{

		int Count();
		bool Add(IList<VertexStack> list);
		string VerticesToString(IList<VertexStack> list);
	}


	class CannotCreateMultipleEdgesException : Exception
	{

	}


	class TriarcReconstruction
	{
		public int maxNumberOfVertices = 38;

		public IStates<long> states = new LongBinaryStatesWithHashSet();
		public TextWriter writer = Console.Out;

		public TriarcGraph triarc;
		bool found = false;
		int version = 0;

		public TriarcReconstruction(TriarcGraph tg, int MaxNumberOfVerticesToAdd)
		{
			triarc = tg;
			maxNumberOfVertices = triarc.Count + MaxNumberOfVerticesToAdd;
		}
		void ExtractResult()
		{
			string name = "Triarc" + triarc.Name + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "__" + version + "v" + triarc.Count;
			StreamWriter sw = new StreamWriter("grafy\\" + name + ".gv");
			StreamWriter sw2 = new StreamWriter("grafy\\" + name + ".txt");
			triarc.WAWrite(sw2);
			sw2.Close();
			triarc.GWWrite(sw);
			sw.Close();
			sw = new StreamWriter("grafy\\" + name + ".BAT");
			triarc.BATWrite(sw, name);
			sw.Close();
		}

		IEnumerable<IList<VertexStack>> SelectWhatToChange(List<VertexStack> active)
		{
			var result = new List<IList<VertexStack>>();
			int i = 0;
			int max = active.Count;
			while (i < max && active[i].HasAllThreeNeighbours())
			{
				i++;
			}
			int toGoTroughAgain = i;
			List<VertexStack> temp = new List<VertexStack>();
			while (i < max + toGoTroughAgain)
			{
				int ii = i % max;
				temp = new List<VertexStack>();
				temp.Add(active[ii]);
				i++;
				ii = i % max;
				while (active[ii].HasAllThreeNeighbours()) //dokued nemíří dovnitř
				{
					temp.Add(active[ii]);
					i++;
					ii = i % max;

				}
				temp.Add(active[ii]);
				result.Add(temp);
			}
			return from item in result
					 orderby item.Count descending
					 select item;
		}

		public int NumberOfActiveWithTwoNeighbours(List<VertexStack> active)
		{
			int i = 0;
			foreach (var item in active)
			{
				if (!item.HasAllThreeNeighbours())
				{
					i++;
				}
			}
			return i;
		}

		void WriteVertices(IList<VertexStack> list, string s, bool extraNewLine, string prefix)
		{

			if (extraNewLine)
			{

				writer.WriteLine(prefix + "-");
			}
			return;
			writer.WriteLine(s);
			foreach (var item in list)
			{
				writer.WriteLine(prefix + item.ToString());
			}
		}

		public void ReconstructTriarc(string depth)
		{
			long boundaryFromActive = (LongBinaryStatesWithHashSet.VerticesToStateStatic(triarc.ActiveVertices())).BoundaryToStandardizedForm();

			List<VertexStack> active = triarc.ActiveVertices();
			if (NumberOfActiveWithTwoNeighbours(active) == 0 && triarc.PolygonSizes.Contains(active.Count))
			{
				ExtractResult();
				writer.WriteLine("že by?");
				found = true;
				//				return;
			}
			if (boundaryFromActive != triarc.StatesToGoTrough.Last() || (NumberOfActiveWithTwoNeighbours(active) == 0))
			{
				var bla = triarc.ActiveVertices();
				long test = LongBinaryStatesWithHashSet.VerticesToStateStatic(triarc.ActiveVertices());
				return; //jsme ve špatné větvi výpočtu
			}
			triarc.StatesToGoTrough.RemoveAt(triarc.StatesToGoTrough.Count - 1);
			if (triarc.Count > maxNumberOfVertices)
			{
				
				return;
			}

			try
			{
				//			writer.WriteLine(states.VerticesToString(active));
			}
			catch (TooManyVerticesToPutInStateException)
			{
				writer.WriteLine("TooManyVerticesToPutInStateException");
				return;
			}

			//	writer.WriteLine(states.VerticesToString(active));

			if (!states.Add(active))
			{
				version++;
				return;
			}
			int activeNUmber = NumberOfActiveWithTwoNeighbours(active);

		


			if (activeNUmber > 1 && !found)
			{
				var selectWhatToChange = SelectWhatToChange(active).ToArray();
				foreach (var selected in selectWhatToChange)
				{


					WriteVertices(active, "active ", true, depth);
					WriteVertices(selected, "selected  ", false, depth);

					foreach (var polySize in triarc.PolygonSizes)
					{
						version++;

						if (triarc.PolygonSizes[triarc.PolygonSizes.Count - 1] < selected.Count)
						{
							return;
						}
						if (polySize < selected.Count)
						{
							continue;
						}


						//předejití násobným hranám
						if ((selected[0].A == selected[selected.Count - 1] || selected[0].B == selected[selected.Count - 1]) && selected.Count==polySize)
						{
							continue;
						}

						int NumberOfVerticesInTriarc = triarc.Count;
						int root = triarc.ActiveRootID;
						List<VertexStack> replacedBy;

						Replace(selected, out replacedBy, polySize);

						WriteVertices(replacedBy, "replaced by", false, depth);
						triarc.ActiveRootID = selected[0].ID;
						List<VertexStack> activeNew = triarc.ActiveVertices();
		//				triarc.ActiveRootID = IndexOfMinimalID(activeNew);

						ReconstructTriarc(depth + ".");
						if (found)
						{
							return;
						}
						foreach (var item in selected)
						{
							//				writer.WriteLine(depth + "Poping " + item);
							item.Pop();
							//				writer.WriteLine(depth + "  on   " + item);
						}
						triarc.vertices.RemoveRange(NumberOfVerticesInTriarc, polySize - selected.Count);
						triarc.Count = NumberOfVerticesInTriarc;
						triarc.ActiveRootID = root;

					}

				}

			}
			writer.Flush();

		}
		int IndexOfMinimalID(IList<VertexStack> list)

		{
			int index = list[0].ID;
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].ID < index)
				{
					index = list[i].ID;
				}
			}
			return index;
		}
		void Replace(IList<VertexStack> selected, out List<VertexStack> replacedBy, int polySize)
		{
			//střed vybraných bude neaktivní
			for (int i = 1; i < selected.Count - 1; i++)
			{
				Vertex temp = selected[i].Peek().Clone();
				temp.Active = false;
				selected[i].Push(temp);
			}

			replacedBy = new List<VertexStack>();
			replacedBy.Add(selected[0]);
			selected[0].Push(selected[0].Peek().Clone());
			for (int i = 0; i < polySize - selected.Count; i++)
			{
				VertexStack tempVS = new VertexStack(triarc.Count);
				triarc.vertices.Add(tempVS);
				triarc.Count++;
				Vertex tempV = new Vertex();
				tempV.A = replacedBy[i];
				tempV.Active = true;
				tempVS.Push(tempV);
				Vertex tempP = replacedBy[i].Pop();
				if (tempP.B == null)
				{
					tempP.B = tempVS;
				}
				else
				{
					tempP.C = tempVS;
				}
				replacedBy[i].Push(tempP);
				replacedBy.Add(tempVS);
			}
			//and connect last
			{
				VertexStack tempVS = selected[selected.Count - 1];
				Vertex tempV = tempVS.Peek().Clone();
				tempV.C = replacedBy[replacedBy.Count - 1];
				tempV.Active = true;
				tempVS.Push(tempV);
				Vertex tempP = replacedBy[replacedBy.Count - 1].Pop();
				if (tempP.B == null)
				{
					tempP.B = tempVS;
				}
				else
				{
					tempP.C = tempVS;
				}
				replacedBy[replacedBy.Count - 1].Push(tempP);
				replacedBy.Add(tempVS);
			}
		}
	}

	class Vertex
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
					else
					{
						throw new CannotCreateMultipleEdgesException();
					}
				}
			}
		}

		public bool Active;
		public VertexStack Succescor(VertexStack predecessor)
		{

			if (A != null && A != predecessor && A.Active)
			{
				return A;
			}
			if (B != null && B != predecessor && B.Active)
			{
				return B;
			}
			if (C != null && C != predecessor && C.Active)
			{

				return C;
			}
			return null;
		}

		public Vertex Clone()
		{
			var clone = new Vertex();
			clone.A = this.A;
			clone.B = this.B;
			clone.c = this.c;
			clone.Active = this.Active;
			return clone;
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
			return neighbours;
		}


	}
	class VertexStack
	{
		Stack<Vertex> stack;
		public int ID;
		public VertexStack(int id)
		{
			stack = new Stack<Vertex>();
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
		public bool Active
		{
			get
			{
				return stack.Peek().Active;
			}
		}
		public VertexStack Succescor(VertexStack predecessor)
		{
			return stack.Peek().Succescor(predecessor);
		}
		public bool HasAllThreeNeighbours()
		{
			return A != null && B != null && C != null;
		}

		public void Push(Vertex v)
		{
			stack.Push(v);
		}
		public Vertex Pop()
		{
			return stack.Pop();
		}
		public Vertex Peek()
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


	class StringStatesWithHasSetAndRedundants : IStates<string>, IStates
	{
		int numberOfDifferent = 0;
		HashSet<string> states = new HashSet<string>();

		public bool Add(IList<VertexStack> list)
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
		public string VerticesToString(IList<VertexStack> list)
		{
			return VerticesToState(list);
		}

		public string VerticesToState(IList<VertexStack> list)
		{

			Func<VertexStack, char> toState = x =>
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



