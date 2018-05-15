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
		public ReconstructionGraph triarc;

		string path;

		/// <summary>
		/// Indicates state of reconstructing triarc.
		/// </summary>
		bool found = false;

		List<string> SequencesOfVerticesLeaidngToResult = new List<string>();

		/// <summary>
		/// Creates a TriarcReconstruction.
		/// </summary>
		/// <param name="triarcGraph">triarcGraph containing only boundary that is the same as last state from sequence.</param>
		/// <param name="sequenceOfStatesLeadingToResult">Solution of finding triarc.</param>
		public TriarcReconstruction(ReconstructionGraph triarcGraph, List<long> sequenceOfStatesLeadingToResult, string path = "")
		{
			this.path = path;
			triarc = triarcGraph;
			triarc.Faces = new List<List<int>>();
			maxNumberOfVertices = int.MaxValue;
			this.SequenceOfStatesLeadingToResult = sequenceOfStatesLeadingToResult;

		}


		/// <summary>
		/// Creates set of files representing result
		/// </summary>
		void ExtractResult()
		{
			if (ExtractResultOff)
			{
				return;
			}
			var dir = Directory.CreateDirectory("grafy\\" + path);
			string fileName = "Triarc" + triarc.Name + "__" + "v" + triarc.CountOfVertices;
			if (Global.ExportAsStandardGraph || Global.ExportAsGraphViz || Global.ExportAsTutteSageScript || Global.ExportFaces)
			{
				Console.WriteLine("Saving into " + dir.FullName + fileName + ". For more output files change parameters.");
			}
			
			if (Global.ExportAsStandardGraph)
			{
				StreamWriter waStreamWriter = new StreamWriter("grafy\\" + path + fileName + ".txt");
				triarc.Write(waStreamWriter);
				waStreamWriter.Close();
			}

			if (Global.ExportAsGraphViz)
			{
				StreamWriter gWStreamWriter = new StreamWriter("grafy\\" + path + fileName + ".gv");
				triarc.GraphvizWrite(gWStreamWriter);
				gWStreamWriter.Close();
			}

			if (Global.ExportAsTutteSageScript)
			{
				StreamWriter CoCalcStreamWriter = new StreamWriter("grafy\\" + path + fileName + "_SageMathScript.txt");
				triarc.SageMathScriptForTuttesEmbeddingWrite(CoCalcStreamWriter);
				CoCalcStreamWriter.Close();
			}

			if (Global.ExportFaces)
			{
				StreamWriter faceswriter = new StreamWriter("grafy\\" + path + fileName + ".faces.txt");
				foreach (var x in triarc.Faces)
				{
					faceswriter.WriteLine(string.Join<int>(",", x));
				}
				faceswriter.Close();
			}


			if (Global.Count3Connectivity && ReconstructionGraph.ThreeConnected.IsGraph3Connected(triarc))
			{
				Console.WriteLine("Graph is 3-connected");
			}
		}


		/// <summary>
		/// Takes vertices forming boundary and returns all sequences starting and ending by vertex, that has only two neighbours,
		/// and all between have three neighbours.  
		/// </summary>
		/// <param name="activeBoundary"></param>
		/// <returns></returns>
		IEnumerable<IList<VertexStack>> SelectWhatToChange(List<VertexStack> activeBoundary)
		{
			var selectedSequences = new List<IList<VertexStack>>();
			int i = 0;
			int max = activeBoundary.Count;

			while (i < max && activeBoundary[i].HasAllThreeNeighbours())
			{
				i++;
			}
			int NumberOfVerticesFromBeginingWithThreeNeighbours = i;


			List<VertexStack> oneSequence = new List<VertexStack>();
			while (i < max + NumberOfVerticesFromBeginingWithThreeNeighbours)
			{
				int ii = i % max;
				oneSequence = new List<VertexStack>();
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
		IEnumerable<IList<VertexStack>> OrderOfExecution(IEnumerable<IList<VertexStack>> selectedSequences)
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
		public int CountOfVerticesWithLessThanThreeNeighbours(List<VertexStack> sequenceOfVertices)
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
		void WriteDepthAndVertices(IList<VertexStack> sequence, string message, bool extraNewLine, string prefix)
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
		void WriteVertices(IList<VertexStack> list, string s, string prefix, TextWriter writer)
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
		string depth = "";


		public bool ReconstructTriarc()
		{
			if (SequenceOfStatesLeadingToResult == null)
			{
				return false;
			}
			reconstructTriarc();
			if (!found)
			{
				Console.WriteLine("was not found, nex state would be " + Convert.ToString(SequenceOfStatesLeadingToResult.Last(), 2));
				return false;
			}
			return true;
		}

		/// <summary>
		/// Builds a full triarc in this.triarc following sequence of boundary from constructor.
		/// </summary>
		private void reconstructTriarc()
		{

			List<VertexStack> active = triarc.ActiveVertices();
			long boundaryFromActive = (LongBinaryStatesWithHashSet.VerticesToStateStatic(active)).BoundaryToStandardizedForm();
			int countOfActiveWithLessThanThreeNeighbours = CountOfVerticesWithLessThanThreeNeighbours(active);

			#region if triarc has been finished
			if (countOfActiveWithLessThanThreeNeighbours == 0 && triarc.FaceSizes.Contains(active.Count))
			{
				triarc.Faces.Add(active.Select(x => x.ID).ToList());
				Console.WriteLine("Triarc has been found, reconstructed and will be saved.");
				ExtractResult();
				if (Global.Count3Connectivity)
				{
						var conn = "Graph " + (ReconstructionGraph.ThreeConnected.IsGraph3Connected(triarc) ? "is" : "isn't") + " 3-connected";
					Console.WriteLine(conn);
				}
				found = true;
				return;
			}
			#endregion


			//If this state isn't the one solution suggests.
			if (boundaryFromActive != SequenceOfStatesLeadingToResult.Last() || (countOfActiveWithLessThanThreeNeighbours == 0))
			{
				return; //jsme ve špatné větvi výpočtu
			}
			//Else remove it to signal that it has been gone trough.
			SequenceOfStatesLeadingToResult.RemoveAt(SequenceOfStatesLeadingToResult.Count - 1);

			SequencesOfVerticesLeaidngToResult.Add(string.Join<string>(", ", active.Select(
					 x => { return x.ID.ToString() + (x.HasAllThreeNeighbours() == true ? "out" : "in"); })));

			#region  If there is a restriction on maximal count of vertices, defaultly set to int.maxValue
			if (triarc.CountOfVertices > maxNumberOfVertices)
			{
				Console.WriteLine("Triarc won't be reconstructed due to having more vertices than allowed.");
				Console.WriteLine("The limit is set to " + maxNumberOfVertices);
				return;
			}
			#endregion

			states.Add(active);

			if (countOfActiveWithLessThanThreeNeighbours > 1 && !found)
			{
				var selectWhatToChange = OrderOfExecution(SelectWhatToChange(active).ToArray());
				foreach (var selected in selectWhatToChange)
				{
					//Conditional debug statements
					//	WriteDepthAndVertices(active, "active ", true, depth);
					//	WriteDepthAndVertices(selected, "selected  ", false, depth);

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
						List<VertexStack> replacedBy;

						Replace(selected, out replacedBy, faceSize);

						//Conditional debug statements.
						//		WriteDepthAndVertices(replacedBy, "replaced by", false, depth);

						triarc.ActiveRootID = selected[0].ID;
						List<VertexStack> activeNew = triarc.ActiveVertices();

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
						triarc.Faces.RemoveAt(triarc.Faces.Count - 1);
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

		/// <summary>
		/// Takes sequence of vertices, turns them into face and returns active part of face in replacedBy
		/// </summary>
		/// <param name="selected">Sequence of original vertices.</param>
		/// <param name="replacedBy">Sequence of new vertices.</param>
		/// <param name="faceSize">size of face used when connecting selected.</param>
		void Replace(IList<VertexStack> selected, out List<VertexStack> replacedBy, int faceSize)
		{
			var newFace = selected.Select(x => x.ID).ToList();

			//střed vybraných bude neaktivní
			for (int i = 1; i < selected.Count - 1; i++)
			{
				SingleVertex temp = selected[i].Peek().Clone();
				temp.IsActive = false;
				selected[i].Push(temp);
			}

			replacedBy = new List<VertexStack>();
			replacedBy.Add(selected[0]);
			selected[0].Push(selected[0].Peek().Clone());
			for (int i = 0; i < faceSize - selected.Count; i++)
			{
				VertexStack tempVS = new VertexStack(triarc.CountOfVertices);
				newFace.Add(tempVS.ID);
				triarc.vertices.Add(tempVS);
				triarc.CountOfVertices++;
				SingleVertex tempV = new SingleVertex();
				tempV.A = replacedBy[i];
				tempV.IsActive = true;
				tempVS.Push(tempV);
				SingleVertex tempP = replacedBy[i].Pop();
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
				SingleVertex tempV = tempVS.Peek().Clone();
				tempV.C = replacedBy[replacedBy.Count - 1];
				tempV.IsActive = true;
				tempVS.Push(tempV);
				SingleVertex tempP = replacedBy[replacedBy.Count - 1].Pop();
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
			triarc.Faces.Add(newFace);
		}

	}
}
