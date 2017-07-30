#undef DebugOutputWeek
#undef DebugOutputStrong
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;


namespace Triarc
{
	class TriarcReconstruction
	{
		/// <summary>
		/// Sequence of states (boundaries) that start by single face, end with triarc outer boundary and consecutive states differes by one edge.
		/// </summary>
		public List<long> SequenceOfStatesLeadingToResult = new List<long>();

		public int maxNumberOfVertices = 38;

		/// <summary>
		/// All found states
		/// </summary>
		public IStates<long> states = new LongBinaryStatesWithHashSet();


		public TriarcGraph triarc;
		bool found = false;
		int version = 0;

		public TriarcReconstruction(TriarcGraph tg, int MaxNumberOfVerticesToAdd)
		{
			triarc = tg;
			maxNumberOfVertices = triarc.CountOfVertices + MaxNumberOfVerticesToAdd;
		}

		void ExtractResult()
		{
			string name = "Triarc" + triarc.Name + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "__" + version + "v" + triarc.CountOfVertices;
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

		IEnumerable<IList<TriarcGraph.VertexStack>> SelectWhatToChange(List<TriarcGraph.VertexStack> active)
		{
			var result = new List<IList<TriarcGraph.VertexStack>>();
			int i = 0;
			int max = active.Count;
			while (i < max && active[i].HasAllThreeNeighbours())
			{
				i++;
			}
			int toGoTroughAgain = i;
			List<TriarcGraph.VertexStack> temp = new List<TriarcGraph.VertexStack>();
			while (i < max + toGoTroughAgain)
			{
				int ii = i % max;
				temp = new List<TriarcGraph.VertexStack>();
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

		public int NumberOfActiveWithTwoNeighbours(List<TriarcGraph.VertexStack> active)
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

		[Conditional("DebugOutputWeek")]
		void WriteDepthAndVertices(IList<TriarcGraph.VertexStack> list, string s, bool extraNewLine, string prefix)
		{
			var writer = Console.Out;
			if (extraNewLine)
			{

				writer.WriteLine(prefix + "-");
			}
			WriteVertices(list, s, prefix, writer);

		}

		[Conditional("DebugOutputStrong")]
		void WriteVertices(IList<TriarcGraph.VertexStack> list, string s, string prefix, TextWriter writer)
		{
			writer.WriteLine(s);
			foreach (var item in list)
			{
				writer.WriteLine(prefix + item.ToString());
			}
		}

		public void ReconstructTriarc(string depth)
		{
			long boundaryFromActive = (LongBinaryStatesWithHashSet.VerticesToStateStatic(triarc.ActiveVertices())).BoundaryToStandardizedForm();

			List<TriarcGraph.VertexStack> active = triarc.ActiveVertices();
			if (NumberOfActiveWithTwoNeighbours(active) == 0 && triarc.FaceSizes.Contains(active.Count))
			{
				Console.WriteLine("Triarc's graph representation has been found.");
				ExtractResult();
				found = true;
			}
			if (boundaryFromActive != SequenceOfStatesLeadingToResult.Last() || (NumberOfActiveWithTwoNeighbours(active) == 0))
			{
				var bla = triarc.ActiveVertices();
				long test = LongBinaryStatesWithHashSet.VerticesToStateStatic(triarc.ActiveVertices());
				return; //jsme ve špatné větvi výpočtu
			}
			SequenceOfStatesLeadingToResult.RemoveAt(SequenceOfStatesLeadingToResult.Count - 1);
			if (triarc.CountOfVertices > maxNumberOfVertices)
			{

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


					WriteDepthAndVertices(active, "active ", true, depth);
					WriteDepthAndVertices(selected, "selected  ", false, depth);

					foreach (var polySize in triarc.FaceSizes)
					{
						version++;

						if (triarc.FaceSizes[triarc.FaceSizes.Count - 1] < selected.Count)
						{
							return;
						}
						if (polySize < selected.Count)
						{
							continue;
						}


						//předejití násobným hranám
						if ((selected[0].A == selected[selected.Count - 1] || selected[0].B == selected[selected.Count - 1]) && selected.Count == polySize)
						{
							continue;
						}

						int NumberOfVerticesInTriarc = triarc.CountOfVertices;
						int root = triarc.ActiveRootID;
						List<TriarcGraph.VertexStack> replacedBy;

						Replace(selected, out replacedBy, polySize);

						WriteDepthAndVertices(replacedBy, "replaced by", false, depth);
						triarc.ActiveRootID = selected[0].ID;
						List<TriarcGraph.VertexStack> activeNew = triarc.ActiveVertices();
						//				triarc.ActiveRootID = IndexOfMinimalID(activeNew);

						ReconstructTriarc(depth + ".");
						if (found)
						{
							return;
						}
						foreach (var item in selected)
						{
							item.Pop();
						}
						triarc.vertices.RemoveRange(NumberOfVerticesInTriarc, polySize - selected.Count);
						triarc.CountOfVertices = NumberOfVerticesInTriarc;
						triarc.ActiveRootID = root;

					}

				}

			}

		}

		int IndexOfMinimalID(IList<TriarcGraph.VertexStack> list)

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

		void Replace(IList<TriarcGraph.VertexStack> selected, out List<TriarcGraph.VertexStack> replacedBy, int polySize)
		{
			//střed vybraných bude neaktivní
			for (int i = 1; i < selected.Count - 1; i++)
			{
				TriarcGraph.Vertex temp = selected[i].Peek().Clone();
				temp.Active = false;
				selected[i].Push(temp);
			}

			replacedBy = new List<TriarcGraph.VertexStack>();
			replacedBy.Add(selected[0]);
			selected[0].Push(selected[0].Peek().Clone());
			for (int i = 0; i < polySize - selected.Count; i++)
			{
				TriarcGraph.VertexStack tempVS = new TriarcGraph.VertexStack(triarc.CountOfVertices);
				triarc.vertices.Add(tempVS);
				triarc.CountOfVertices++;
				TriarcGraph.Vertex tempV = new TriarcGraph.Vertex();
				tempV.A = replacedBy[i];
				tempV.Active = true;
				tempVS.Push(tempV);
				TriarcGraph.Vertex tempP = replacedBy[i].Pop();
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
				TriarcGraph.VertexStack tempVS = selected[selected.Count - 1];
				TriarcGraph.Vertex tempV = tempVS.Peek().Clone();
				tempV.C = replacedBy[replacedBy.Count - 1];
				tempV.Active = true;
				tempVS.Push(tempV);
				TriarcGraph.Vertex tempP = replacedBy[replacedBy.Count - 1].Pop();
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
}
