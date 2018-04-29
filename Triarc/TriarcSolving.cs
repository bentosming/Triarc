using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Text;

namespace Triarc
{
	class TriarcSolving
	{
		/// <summary>
		/// Indicates, that Triarc has been found. Value is never changed to false which makes it ThreadSafe
		/// </summary>
		bool Found = false;
		

		/// <summary>
		/// All known boundaries, that can be accessed from the StartingBoundary, that aren't proccessed or being proccesed. 
		/// </summary>
		ImmutableSortedSet<long> UnresolvedBoundaries = ImmutableSortedSet<long>.Empty;

		/// <summary>
		/// Count of tasks at ThreadPool that will try to run sy at once, no need putting in more than expected number of proccessors available, defaultly 4
		/// </summary>
		public int TaskCount { get; set; }

		const int NumberOfBitsInStruct = 64;

		//Allows direct access to each bit.
		readonly static long[] SetBits = new long[NumberOfBitsInStruct]
		{
		0x1, 0x2, 0x4, 0x8 ,
		0x10, 0x20, 0x40, 0x80,
		0x100, 0x200, 0x400, 0x800,
		0x1000, 0x2000, 0x4000, 0x8000 ,
		0x00010000, 0x00020000, 0x00040000, 0x00080000,
		0x00100000, 0x00200000, 0x00400000, 0x00800000,
		0x01000000, 0x02000000, 0x04000000, 0x08000000,
		0x10000000, 0x20000000, 0x40000000, 0x80000000,
		0x100000000, 0x200000000, 0x400000000, 0x800000000 ,
		0x1000000000, 0x2000000000, 0x4000000000, 0x8000000000,
		0x10000000000, 0x20000000000, 0x40000000000, 0x80000000000,
		0x100000000000, 0x200000000000, 0x400000000000, 0x800000000000 ,
		0x0001000000000000, 0x0002000000000000, 0x0004000000000000, 0x0008000000000000,
		0x0010000000000000, 0x0020000000000000, 0x0040000000000000, 0x0080000000000000,
		0x0100000000000000, 0x0200000000000000, 0x0400000000000000, 0x0800000000000000,
		0x1000000000000000, 0x2000000000000000, 0x4000000000000000, long.MinValue};

		/// <summary>
		/// Each pair represents two boundaries that differ by one added edge. 
		/// ("value" can be obtained from "key" by connecting a pair of "ones" (set bits) that have no "one" between them).
		/// </summary>
		ImmutableDictionary<long, long> BoundaryTransitions = ImmutableDictionary.Create<long, long>();

		/// <summary>
		/// Standartized form of faces that are allowed in triarc.
		/// </summary>
		long[] Faces;

		/// <summary>
		/// Lengths of faces that are allowed in triarc.
		/// </summary>
		int[] FacesSizes;

		/// <summary>
		/// Name of triarc, initialized to (a,b,c).
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Starting point for search, initialized to outter boundary of triarc.
		/// </summary>
		long StartingBoundary;



		/// <summary>
		/// Constructor takes three counts which represent counts of vertices between main triarc vertices, that have degree 2 in triarc.
		/// FaceSizes must be those values that Eberhard-like theorems suggest. 
		/// </summary>
		/// <param name="a">First number that determines triarc</param>
		/// <param name="b">Second number that determines triarc</param>
		/// <param name="c">Third number that determines triarc</param>
		/// <param name="facesSizes">List of values that satisfy neccesary conditions. Must has greatest value last, should be sorted. </param>
		/// <param name="taskCount">Best speed should be obtained with taskCount similar to number of proccesors.</param>
		public TriarcSolving(int a, int b, int c, int[] facesSizes, int taskCount = 4)
		{
			this.TaskCount = taskCount;
			this.Name = "(" + a + "," + b + "," + c + ")";
			this.FacesSizes = facesSizes;
			StartingBoundary = CreateOuterBoundaryOfTriarc(a, b, c);
			Faces = new long[facesSizes.Length];
			for (int i = 0; i < Faces.Length; i++)
			{
				Faces[i] = BoundaryLong.FaceToBoundary(FacesSizes[i]);
			}

		}

		public TriarcSolving(long startingBoundary, int[]facesSizes, int limit=100000, int taskCount = 1)
		{
			this.TaskCount = taskCount;
			this.TransitionLimit = limit;
			this.Name = "(" +Convert.ToString(startingBoundary,2) + ")";
			this.FacesSizes = facesSizes;
			StartingBoundary = startingBoundary;
			Faces = new long[facesSizes.Length];
			for (int i = 0; i < Faces.Length; i++)
			{
				Faces[i] = BoundaryLong.FaceToBoundary(FacesSizes[i]);
			}
		}


		/// <summary>
		/// Returns biggest face allowed in triarc.
		/// </summary>
		/// <returns></returns>
		int BiggestFace()
		{
			return FacesSizes[FacesSizes.Count() - 1];
		}



		/// <summary>
		/// Human-readable representation of allowed faces.
		/// </summary>
		/// <returns>Human-readable representation of allowed faces.</returns>
		string FaceSizesToString()
		{
			StringBuilder faceSizesStringBuilder = new StringBuilder("[");
			for (int i = 0; i < FacesSizes.Count() - 1; i++)
			{
				faceSizesStringBuilder.Append(FacesSizes[i] + ",");
			}

			faceSizesStringBuilder.Append(FacesSizes[FacesSizes.Count() - 1] + "]");
			return faceSizesStringBuilder.ToString();
		}



		/// <summary>
		/// Returns the outer boundary of (a,b,c)-triarc in standardized form.
		/// </summary>
		/// <param name="a">First number that determines triarc</param>
		/// <param name="b">Second number that determines triarc</param>
		/// <param name="c">Third number that determines triarc</param>
		/// <returns></returns>
		static long CreateOuterBoundaryOfTriarc(int a, int b, int c)
		{
			//Starting from first main vertex of triarc and adding 0 or 1 for each vertex. 
			long Boundary = 0;
			for (int i = 2; i < (a + b + c) * 2; i++)
			{
				Boundary <<= 1;
				if ((i & 1) == 0 && i != a + a && i != a + a + b + b && i != 0)
				{
					Boundary |= 1;
				}

			}

			//First and second vertex when starting from one of mains are "zeros" so they need to be added to the end to not to be lost.
			Boundary <<= 2;
			return Boundary.BoundaryToStandardizedForm();
		}
		
		

		/// <summary>
		/// Adds edge or chain between first and second ones and adds the transition.
		/// </summary>
		/// <param name="boundary">Boundary in any valid representation, has to have at least 3 ones</param>
		/// <param name="orderOfHighestBitSet">Pre-processed information about boundary.</param>
		/// <param name="orderOfSecondHighestBitSet">Pre-processed information about boundary.</param>
		/// <param name="boundaryInStandardizedForm">Pre-processed information about boundary.</param>
		/// <param name="faceLength">Length of face, that first and second ones will be conected by.</param>
		/// <returns>If there is a way to transition and the new boundary was not already explored, then it is the returning value, otherwise returns 0</returns>
		long TransitAndAdd(long boundary, int orderOfHighestBitSet, int orderOfSecondHighestBitSet, long boundaryInStandardizedForm, int faceLength)
		{
			//count of vertices, that already have to share the face
			int lengthAlready = orderOfHighestBitSet - orderOfSecondHighestBitSet + 1;
			//number of vertices that will have to be added on the new edge
			int lengthMissing = faceLength - lengthAlready;
			if (lengthMissing >= 0)
			{
				//to long for choosen representation
				if (orderOfSecondHighestBitSet + lengthMissing >= NumberOfBitsInStruct - 1)
				{
					return 0;
				}

				//remove first two ones
				long newBoundary = boundary ^ SetBits[orderOfHighestBitSet] ^ SetBits[orderOfSecondHighestBitSet];

				//adding ones for ones on new edge
				for (int i = orderOfSecondHighestBitSet + 1; i <= orderOfSecondHighestBitSet + lengthMissing; i++)
				{
					newBoundary |= SetBits[i];
				}

				//no vertex added on new edge - length of boundary remains and first vertex needs to be one
				if (lengthMissing == 0)//nahrazeno jen za 00
				{
					while ((SetBits[orderOfSecondHighestBitSet] & newBoundary) == 0) //nulls right to second one + a zero instead of one of first ones
					{
						newBoundary <<= 1;
					}
				}

				newBoundary <<= 1; //null representing the first substitued one

				long newBoundaryStandardized = newBoundary.BoundaryToStandardizedForm();
				if (IsValid(newBoundaryStandardized))
				{
					if (
					AddTransition(boundaryInStandardizedForm, newBoundaryStandardized))
					{
						return newBoundaryStandardized;
					}
				}
			}

			return 0;

		}



		/// <summary>
		/// Takes boundary in standardized form and instead of trying to add all boundaries that can be transited to from this one,
		/// it immediately resolves them (It certainly will be one of the smallest possible.).
		/// If it finds the end of triarc it changes triarc state to found and starts the finishing backtracking of triarc.
		/// </summary>
		/// <param name="boundary">Boundary to transit from</param>
		/// <param name="orderOfHighestBitSet">Pre-proccesed information boundary</param>
		/// <param name="orderOfSecondHighestBitSet">Pre-procccesed information about boundary</param>
		void TransitAndAddOnlyTwoOnes(long boundary, int orderOfHighestBitSet, int orderOfSecondHighestBitSet)
		{

			int shorterSequenceOfNulls = orderOfHighestBitSet - orderOfSecondHighestBitSet - 1;
			int longerSequenceOfNulls = orderOfSecondHighestBitSet;

			//defensive programming - control check
			if (orderOfHighestBitSet - orderOfSecondHighestBitSet - 1 > orderOfSecondHighestBitSet)
			{
				shorterSequenceOfNulls = orderOfSecondHighestBitSet;
				longerSequenceOfNulls = orderOfHighestBitSet - orderOfSecondHighestBitSet - 1;
			}

			foreach (var face in FacesSizes)
			{
				//If  connecting first and second one by an edge causes that bouth new faces are in allowed, than solution has been found
				if (shorterSequenceOfNulls + 2 == face && FacesSizes.Contains(longerSequenceOfNulls + 2))
				{
					long newBoundary = BoundaryLong.FaceToBoundary(face);
					if (AddTransition(boundary, newBoundary))
					{
						Found = true;
					}
				}

				if (shorterSequenceOfNulls + 2 + 1 < face) //needs to enlarge by two at least
				{  //ones are changed to nulls, new ones remains + number of nulls in secondSequence + 2
					//jedničky se změní na nuly, zbydou nové jedničky + počet nul v delší sérii + 2
					long newBoundary = 0;
					for (int i = longerSequenceOfNulls + 2; i < longerSequenceOfNulls + face - shorterSequenceOfNulls; i++)
					{
						newBoundary |= SetBits[i];
					}
					if (IsValid(newBoundary) && AddTransition(boundary, newBoundary))
					{
						ResolveBoundary(newBoundary);
					}
				}
				if (longerSequenceOfNulls + 2 + 1 < face) //needs to enlarge by two at least
				{ //jedničky se změní na nuly, zbydou nové jedničky + počet nul v delší sérii + 2
					long newBoundary = 0;
					for (int i = shorterSequenceOfNulls + 2; i < shorterSequenceOfNulls + face - longerSequenceOfNulls; i++)
					{
						newBoundary |= SetBits[i];
					}

					if (IsValid(newBoundary) && AddTransition(boundary, newBoundary))
					{
						ResolveBoundary(newBoundary);
					}
				}
			}
		}



		/// <summary>
		/// Thread safe addition to set of boundaries that will be resolved later.
		/// Should be used only after succcesfully adding transition to this boundary
		/// </summary>
		/// <param name="boundary">value to be added</param>
		/// <returns>returns false only if boundary is contained before adding</returns>
		bool AddUnresolvedBoundary(long boundary)
		{
			var localUnresolvedBoundaries = UnresolvedBoundaries;
			var modifiedLocalUnresolvedBoundaries = localUnresolvedBoundaries.Add(boundary);
			bool someOtherThreadHasAddedThis = localUnresolvedBoundaries.Contains(boundary);
			while (!someOtherThreadHasAddedThis && Interlocked.CompareExchange(ref UnresolvedBoundaries, modifiedLocalUnresolvedBoundaries, localUnresolvedBoundaries) != localUnresolvedBoundaries)
			{
				localUnresolvedBoundaries = UnresolvedBoundaries;
				someOtherThreadHasAddedThis = localUnresolvedBoundaries.Contains(boundary);
				modifiedLocalUnresolvedBoundaries = localUnresolvedBoundaries.Add(boundary);
			}

			return !someOtherThreadHasAddedThis;
		}



		/// <summary>
		/// ThreadSafe addition of transitions. Will be added only if the new boundary isn't included already.
		/// </summary>
		/// <param name="originalBoundary"></param>
		/// <param name="newBoundary"></param>
		/// <returns></returns>
		bool AddTransition(long originalBoundary, long newBoundary)
		{
			var localBoundaryTransitions = BoundaryTransitions;
				
			if (!localBoundaryTransitions.ContainsKey(newBoundary))
			{
				var modifiedLoacalBoundaryTransitions = localBoundaryTransitions.Add(newBoundary, originalBoundary);
				bool someOtherThreadHasAddedThis = localBoundaryTransitions.ContainsKey(newBoundary);
				while (!someOtherThreadHasAddedThis && Interlocked.CompareExchange(ref BoundaryTransitions, modifiedLoacalBoundaryTransitions, localBoundaryTransitions) != localBoundaryTransitions)
				{
					localBoundaryTransitions = BoundaryTransitions;
					if (localBoundaryTransitions.ContainsKey(newBoundary))
					{
						someOtherThreadHasAddedThis = true;
					}
					else {
						modifiedLoacalBoundaryTransitions = localBoundaryTransitions.Add(newBoundary, originalBoundary);
					}
				}
				if (!someOtherThreadHasAddedThis)
				{
					return true;
				}
			}
			return false;
		}



		/// <summary>
		/// Finds sequence of boundaries that lead from outer boundary to finished triarc.
		/// </summary>
		/// <returns>Sequence of boundaries that lead from starting boundary to finished triarc</returns>
		public List<long> SolveTriarc()
		{

			//Adding starting points
			AddUnresolvedBoundary(StartingBoundary);
			BoundaryTransitions = BoundaryTransitions.Add(StartingBoundary, StartingBoundary);


			//FindTriarc procedure starts several tasks that than each add new and new transitions.
			//Found field serves as cancellation token and each task has to regurallely check it.
			//Main thread waits for all tasks to finish, but thanks to cancelation doesn't block.


			//Prepearing some boundaries to solve for tasks, otherwise they might actively wait 
			//at the very beggining when they try to get a boundary to solve.
			int j = 0;
			while (UnresolvedBoundaries.Count != 0 && !Found && j < TaskCount)
			{
				j++;
				long boundaryToDo = RemoveFirstUnresolvedBoundary();
				if (boundaryToDo != 0L)
				{
					ResolveBoundary(boundaryToDo);
				}
			}

			List<Task> tasks = new List<Task>();
			for (int i = 0; i < TaskCount; i++)
			{
				tasks.Add(Task.Factory.StartNew(TaskSolveTriarc, TaskCreationOptions.LongRunning));

			}
			var continuation = Task.WhenAll(tasks);
			continuation.Wait();

			GetResultFromTransitions();
			return ResultingSequenceOfBoundaries;
		}

		/// <summary>
		/// Allows to stop execution if it reaches to many transitiones
		/// </summary>
		public int TransitionLimit = int.MaxValue;

		/// <summary>
		/// Action for tasks
		/// </summary>
		void TaskSolveTriarc()
		{
			while (UnresolvedBoundaries.Count != 0 && !Found && BoundaryTransitions.Count<TransitionLimit)
			{
				long unresolvedBoundaryToBeProcessed = RemoveFirstUnresolvedBoundary();
				if (unresolvedBoundaryToBeProcessed != 0L)
				{

					ResolveBoundary(unresolvedBoundaryToBeProcessed);
				}
			}
		}



		/// <summary>
		/// Returns the smallest boundary that is unresolved. If empty, returns zero.
		/// </summary>
		/// <returns> Smallest unresolved boundary or zero.</returns>
		long RemoveFirstUnresolvedBoundary()
		{
			var localUnresolvedBoundaries = UnresolvedBoundaries;
			var firstOrDefault = localUnresolvedBoundaries.FirstOrDefault();
			if (firstOrDefault == 0L)
			{
				return firstOrDefault;
			}
			var modifiedLocalUnresolvedBoundaries = localUnresolvedBoundaries.Remove(firstOrDefault);
			while (Interlocked.CompareExchange(ref UnresolvedBoundaries, modifiedLocalUnresolvedBoundaries, localUnresolvedBoundaries)
				!= localUnresolvedBoundaries)
			{

				localUnresolvedBoundaries = UnresolvedBoundaries;
				firstOrDefault = localUnresolvedBoundaries.FirstOrDefault();
				if (firstOrDefault == 0L)
				{
					return firstOrDefault;
				}
				modifiedLocalUnresolvedBoundaries = localUnresolvedBoundaries.Remove(firstOrDefault);
			}
	//		Console.WriteLine(Convert.ToString(firstOrDefault, 2) + " in progress by task " + Task.CurrentId);
			return firstOrDefault;
		}



		/// <summary>
		/// Takes a valid boundary in standard form and tries to add all valid boundaries that can be transmitted to.
		/// If boundary has only two ones, all of its direct transmitioned boundaries will be resolved as well.
		/// </summary>
		/// <param name="boundary">Boundary to be resolved</param>
		void ResolveBoundary(long boundary)
		{
			long originalBoundary = boundary;
			int orderOfHighestSetBit = boundary.OrderOfHighestSetBit();
			long onlyOrderOfHighestSetBitSet = SetBits[orderOfHighestSetBit];

			int alreadyRotatedBy = 0;
			int orderOfSecondHighestBitSet = (boundary - onlyOrderOfHighestSetBitSet).OrderOfHighestSetBit();
			if (orderOfSecondHighestBitSet == -1) //defensive programming
			{
				return;
			}

			if ((boundary ^ onlyOrderOfHighestSetBitSet ^ SetBits[orderOfSecondHighestBitSet]) == 0)
			{
				TransitAndAddOnlyTwoOnes(boundary, orderOfHighestSetBit, orderOfSecondHighestBitSet);
				return;
			}

			while (alreadyRotatedBy <= orderOfHighestSetBit && orderOfHighestSetBit > 0)
			{

				foreach (var face in FacesSizes)
				{
					long newBoundary = TransitAndAdd(boundary, orderOfHighestSetBit, orderOfSecondHighestBitSet, originalBoundary, face);
					if (newBoundary == 0)
					{
						continue;
					}

					AddUnresolvedBoundary(newBoundary);

				}

				//Get next representation of boundary
				boundary = ((boundary ^ onlyOrderOfHighestSetBitSet) << (orderOfHighestSetBit - orderOfSecondHighestBitSet)) | (onlyOrderOfHighestSetBitSet >> (orderOfSecondHighestBitSet + 1));
				alreadyRotatedBy += orderOfHighestSetBit - orderOfSecondHighestBitSet;
				orderOfSecondHighestBitSet = (boundary - onlyOrderOfHighestSetBitSet).OrderOfHighestSetBit();

			}

		}



		/// <summary>
		/// Test if bondary is valid in this triarc, which means that there is no sequence of zeros, that can't be in one face.
		/// </summary>
		/// <param name="boundary"></param>
		/// <returns>True if boundary is valid, false otherwise</returns>
		public bool IsValid(long boundary)
		{
			int counter = 0;
			if ((boundary & SetBits[NumberOfBitsInStruct - 1]) == 0)
			{
				while (boundary != 0)
				{
					if (counter > BiggestFace() - 1)
					{
						return false;
					}
					if ((boundary & 1) == 0)
					{
						counter++;
					}
					else
					{
						counter = 0;
					}
					boundary >>= 1;
				}
				return true;
			}
			else //Starts with one
			{
				return Faces.Contains(boundary);
			}
		}


		List<long> ResultingSequenceOfBoundaries;


		/// <summary>
		/// Finds sequence of boundaries from found transitions, that leads from last face closed to outer boundary.
		/// </summary>
		/// <returns>Sequence of boundaries that lead from singe face to outer boundary.</returns>
		public List<long> GetResultFromTransitions()
		{
			if (!Found)
			{
				return null;
			}
			this.ResultingSequenceOfBoundaries = new List<long>();

			//Finds which of faces is the final state
			foreach (var face in Faces)
			{
				if (BoundaryTransitions.ContainsKey(face))
				{
					ResultingSequenceOfBoundaries.Add(face);
					long key = face;
					long value;
					BoundaryTransitions.TryGetValue(key, out value);
					ResultingSequenceOfBoundaries.Add(value);
					while (value != StartingBoundary)
					{
						key = value;
						BoundaryTransitions.TryGetValue(key, out value);
						ResultingSequenceOfBoundaries.Add(value);
					}

				}
			}
			ExtractResultingSequence(new StreamWriter("solutions\\" +Name + "-" + FaceSizesToString() + ".txt"));
			return ResultingSequenceOfBoundaries;
		}

		void ExtractResultingSequence(TextWriter textWriter)
		{
			//TextWriter textWriter = new StreamWriter(Name + "-" + FaceSizesToString() + ".txt");

			textWriter.WriteLine(Name);
			foreach (var value in ResultingSequenceOfBoundaries)
			{
				textWriter.WriteLine(Convert.ToString(value, 2));
			}
			textWriter.Flush();
			textWriter.Close();

		}
	}
}
