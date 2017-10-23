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


		/// <summary>
		/// Maximal count of vertices in triarc, including outer bourdary.
		/// </summary>
		public int maxNumberOfVertices;


		/// <summary>
		/// All states found during execution.
		/// </summary>
		IStates<long> states = new LongBinaryStatesWithHashSet();

		public bool ExtractResultOff = false;

		/// <summary>
		/// Triarc to build.
		/// </summary>
		public TriarcGraph triarc;


		/// <summary>
		/// Indicates state of reconstructing triarc.
		/// </summary>
		bool found = false;
		

		/// <summary>
		/// Creates a TriarcReconstruction.
		/// </summary>
		/// <param name="triarcGraph">triarcGraph containing only boundary that is the same as last state from sequence.</param>
		/// <param name="sequenceOfStatesLeadingToResult">Solution of finding triarc.</param>
		public TriarcReconstruction(ITriarcReconstruction triarcGraph, List<long> sequenceOfStatesLeadingToResult)
		{
			triarc = triarcGraph;
			maxNumberOfVertices = int.MaxValue;
			this.SequenceOfStatesLeadingToResult = sequenceOfStatesLeadingToResult;
		}


		/// <summary>
		/// Creates set of files representing result
		/// grafy\name.gv - graphVizard readable.
		/// grafy\name.BAT - converts .gv file into png.
		/// grafy\name.txt - WolframAlpha readable.
		/// </summary>
		void ExtractResult()
		{
			if (ExtractResultOff)
			{
				return;
			}

			string fileName = "Triarc" + triarc.Name + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "__"  + "v" + triarc.CountOfVertices;
			Console.WriteLine("Triarc will be saved into grafy\\" + fileName + " as gv, txt and BAT files.");
			StreamWriter gWStreamWriter = new StreamWriter("grafy\\" + fileName + ".gv");
			StreamWriter waStreamWriter = new StreamWriter("grafy\\" + fileName + ".txt");
			triarc.WAWrite(waStreamWriter);
			waStreamWriter.Close();
			triarc.GWWrite(gWStreamWriter);
			gWStreamWriter.Close();
			gWStreamWriter = new StreamWriter("grafy\\" + fileName + ".BAT");
			triarc.BATWrite(gWStreamWriter, fileName);
			gWStreamWriter.Close();
		}


		/// <summary>
		/// Takes vertices forming boundary and returns all sequences starting and ending by vertex, that has only two neighbours,
		/// and all between have three neighbours.  
		/// </summary>
		/// <param name="activeBoundary"></param>
		/// <returns></returns>
		IEnumerable<IList<TriarcGraph.VertexStack>> SelectWhatToChange(List<TriarcGraph.VertexStack> activeBoundary)
		{
			var selectedSequences = new List<IList<TriarcGraph.VertexStack>>();
			int i = 0;
			int max = activeBoundary.Count;
			
			while (i < max && activeBoundary[i].HasAllThreeNeighbours())
			{
				i++;
			}
			int NumberOfVerticesFromBeginingWithThreeNeighbours	 = i;


			List<TriarcGraph.VertexStack> oneSequence = new List<TriarcGraph.VertexStack>();
			while (i < max + NumberOfVerticesFromBeginingWithThreeNeighbours)
			{
				int ii = i % max;
				oneSequence = new List<TriarcGraph.VertexStack>();
				oneSequence.Add(activeBoundary[ii]);
				i++;
				ii = i % max;
				while (activeBoundary[ii].HasAllThreeNeighbours()) //dokued nemíří dovnitř
				{
					oneSequence.Add(activeBoundary[ii]);
					i++;
					ii = i % max;
				}
				oneSequence.Add(activeBoundary[ii]);
				selectedSequences.Add(oneSequence);
			}

			return selectedSequences;
		}

		/// <summary>
		/// Specifies in which order will be sequences changed.
		/// </summary>
		/// <param name="selectedSequences"></param>
		/// <returns>Sorted list by priority.</returns>
		IEnumerable<IList<TriarcGraph.VertexStack>> OrderOfExecution(IEnumerable<IList<TriarcGraph.VertexStack>> selectedSequences)
		{
			return from item in selectedSequences
			orderby item.Count descending
			select item;
		}

		/// <summary>
		/// Counts how many vertices have less than three neighbours.
		/// </summary>
		/// <param name="sequenceOfVertices"></param>
		/// <returns></returns>
		public int CountOfVerticesWithLessThanThreeNeighbours(List<TriarcGraph.VertexStack> sequenceOfVertices)
		{
			int i = 0;
			foreach (var item in sequenceOfVertices)
			{
				if (!item.HasAllThreeNeighbours())
				{
					i++;
				}
			}
			return i;
		}

		/// <summary>
		/// Debug Statements. 
		/// </summary>
		/// <param name="sequence">Vertices to write.</param>
		/// <param name="message">Line written before vertices.</param>
		/// <param name="extraNewLine">Print extra new line?</param>
		/// <param name="prefix">Start of each line.</param>
		[Conditional("DebugOutputWeek")]
		void WriteDepthAndVertices(IList<TriarcGraph.VertexStack> sequence, string message, bool extraNewLine, string prefix)
		{

			var writer = Console.Out;
			WriteDepth(extraNewLine, prefix, writer);
			WriteVertices(sequence, message, prefix, writer);

		}

		[Conditional("DebugOutputWeek")]
		void WriteDepth(bool extraNewLine, string prefix, TextWriter writer)
		{
			if (extraNewLine)
			{
				writer.WriteLine(prefix + "-");
			}
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


		/// <summary>
		/// Symbolizes depth of recursion.
		/// </summary>
		string depth ="";


		public bool ReconstructTriarc()
		{
			if (SequenceOfStatesLeadingToResult==null)
			{

				return false;
			}
			reconstructTriarc();
			return false;
		}

		/// <summary>
		/// Builds a full triarc in this.triarc following sequence of boundary from constructor.
		/// </summary>
		private void reconstructTriarc()
		{

			List<TriarcGraph.VertexStack> active = triarc.ActiveVertices();
			long boundaryFromActive = (LongBinaryStatesWithHashSet.VerticesToStateStatic(active)).BoundaryToStandardizedForm();
			int countOfActiveWithLessThanThreeNeighbours = CountOfVerticesWithLessThanThreeNeighbours(active);

			//if triarc has been finished
			if (countOfActiveWithLessThanThreeNeighbours == 0 && triarc.FaceSizes.Contains(active.Count))
			{
				Console.WriteLine("Triarc has been found, reconstructed and will be saved.");
				Console.WriteLine();
				ExtractResult();
				found = true;
			}

			//If this state isn't the one solution suggests.
			if (boundaryFromActive != SequenceOfStatesLeadingToResult.Last() || (countOfActiveWithLessThanThreeNeighbours == 0))
			{
				return; //jsme ve špatné větvi výpočtu
			}
			//Else remove it to signal that it has been gone trough.
			SequenceOfStatesLeadingToResult.RemoveAt(SequenceOfStatesLeadingToResult.Count - 1);

			//If there is a restriction on maximal count of vertices, defaultly set to int.maxValue
			if (triarc.CountOfVertices > maxNumberOfVertices)
			{
				Console.WriteLine("Triarc won't be reconstructed due to having more vertices than allowed.");
				Console.WriteLine("The limit is set to " + maxNumberOfVertices);
				return;
			}


			states.Add(active);
			

			if (countOfActiveWithLessThanThreeNeighbours > 1 && !found)
			{
				var selectWhatToChange = OrderOfExecution( SelectWhatToChange(active).ToArray());
				foreach (var selected in selectWhatToChange)
				{
					//Conditional debug statements
					WriteDepthAndVertices(active, "active ", true, depth);
					WriteDepthAndVertices(selected, "selected  ", false, depth);

					foreach (var faceSize in triarc.FaceSizes)
					{

						//Skips invalid states
						if (triarc.FaceSizes[triarc.FaceSizes.Count - 1] < selected.Count)
						{
							return;
						}

						//Skips invalid state
						if (faceSize < selected.Count)
						{
							continue;
						}

						//Prevents multipleEdges and loops
						if ((selected[0].A == selected[selected.Count - 1] || selected[0].B == selected[selected.Count - 1]) && selected.Count == faceSize)
						{
							continue;
						}

						int NumberOfVerticesInTriarc = triarc.CountOfVertices;
						int localRoot = triarc.ActiveRootID;
						List<TriarcGraph.VertexStack> replacedBy;

						Replace(selected, out replacedBy, faceSize);

						//Conditional debug statements.
						WriteDepthAndVertices(replacedBy, "replaced by", false, depth);

						triarc.ActiveRootID = selected[0].ID;
						List<TriarcGraph.VertexStack> activeNew = triarc.ActiveVertices();

						string localDepth = depth;
						depth += ".";

						//Recursive calling
						reconstructTriarc();

						depth = localDepth;

						if (found)
						{
							return;
						}

						//Undo changes done by this method before returning
						foreach (var item in selected)
						{
							item.Pop();
						}
						triarc.vertices.RemoveRange(NumberOfVerticesInTriarc, faceSize - selected.Count);
						triarc.CountOfVertices = NumberOfVerticesInTriarc;
						triarc.ActiveRootID = localRoot;

					}

				}

			}

		}

		/// <summary>
		/// Returns vertex with least value of ID.
		/// </summary>
		/// <param name="list"></param>
		/// <returns>Vertex with least value of ID.</returns>
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

		/// <summary>
		/// Takes sequence of vertices, turns them into face and returns active part of face in replacedBy
		/// </summary>
		/// <param name="selected">Sequence of original vertices.</param>
		/// <param name="replacedBy">Sequence of new vertices.</param>
		/// <param name="faceSize">size of face used when connecting selected.</param>
		void Replace(IList<TriarcGraph.VertexStack> selected, out List<TriarcGraph.VertexStack> replacedBy, int faceSize)
		{
			//střed vybraných bude neaktivní
			for (int i = 1; i < selected.Count - 1; i++)
			{
				TriarcGraph.SingleVertex temp = selected[i].Peek().Clone();
				temp.IsActive = false;
				selected[i].Push(temp);
			}

			replacedBy = new List<TriarcGraph.VertexStack>();
			replacedBy.Add(selected[0]);
			selected[0].Push(selected[0].Peek().Clone());
			for (int i = 0; i < faceSize - selected.Count; i++)
			{
				TriarcGraph.VertexStack tempVS = new TriarcGraph.VertexStack(triarc.CountOfVertices);
				triarc.vertices.Add(tempVS);
				triarc.CountOfVertices++;
				TriarcGraph.SingleVertex tempV = new TriarcGraph.SingleVertex();
				tempV.A = replacedBy[i];
				tempV.IsActive = true;
				tempVS.Push(tempV);
				TriarcGraph.SingleVertex tempP = replacedBy[i].Pop();
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
				TriarcGraph.SingleVertex tempV = tempVS.Peek().Clone();
				tempV.C = replacedBy[replacedBy.Count - 1];
				tempV.IsActive = true;
				tempVS.Push(tempV);
				TriarcGraph.SingleVertex tempP = replacedBy[replacedBy.Count - 1].Pop();
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
