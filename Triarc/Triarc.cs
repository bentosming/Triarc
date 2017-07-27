using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;


namespace Triarc
{
	public class Triarc
	{
		/// <summary>
		/// Signalizuje přidání koncového stavu do Boundaries, dohodou vím, že je měměno jen na true, čímž je thread safe
		/// </summary>
		public bool Found = false;

		SortedSet<long> BoundariesToDo = new SortedSet<long>();

		long[]  SetBits;
		public ImmutableDictionary<long,long> Boundaries = ImmutableDictionary.Create<long, long>();
		long[] Faces;
		int[] FacesSizes;
		int BiggestFace;
		string Name;
		public long Boundary;
		public Triarc(int a, int b, int c, int[] facesSizes)
		{
			this.Name = "(" + a + "," + b + "," + c + ",)";
			this.FacesSizes = facesSizes; 
			Boundary = CreateBoundary(a, b, c);
			BiggestFace = facesSizes.Last();
			Faces = new long[facesSizes.Length];
			for (int i = 0; i < Faces.Length; i++)
			{
				Faces[i] = BoundaryInt.FaceToBoundary(FacesSizes[i]);
			}
			
			SetBits = new long[64];
			long j=1;
			for (int i = 0; i < 64; i++)
			{
				SetBits[i] = j;
				j <<= 1;
			}
		}
		public static long CreateBoundary(int a, int b, int c)
		{
			long Boundary = 0;
			for (int i = 2; i < (a + b + c) * 2; i++)
			{
				Boundary <<= 1;
				if ((i & 1) == 0 && i != a + a && i != a + a + b + b && i != 0)
				{
					Boundary |= 1;
				}

			}
			//první dva jsou 00 - je třeba je napsat na konec
			Boundary <<= 2;
			return Boundary.ToBoundary();
		}

		/// <summary>
		/// funguje pro čísla s alespoň třemi 1
		/// </summary>
		/// <param name="b"></param>
		/// <param name="h"></param>
		/// <param name="hv"></param>
		/// <param name="s"></param>
		public long SubstituteAndStartTask(long b, int first,  int second, long originalb, int face)
		{
			int lengthAlready = first - second + 1; //count of vertices, that already have to share face


			int lengthMissing = face - lengthAlready;
			if (lengthMissing >= 0)
			{
				if (second + lengthMissing >= 64)
				{
					return 0;
				}
				long temp = b ^ SetBits[first] ^ SetBits[second];//vynuluje jedničky na začátku
				for (int i = second + 1; i <= second + lengthMissing; i++)
				{
					temp |= SetBits[i];
				}
				if (first - lengthAlready + 1 + lengthMissing >= 63)
				{
					return 0;
				}
				//second  + face -first + second -1) 
				//while ((SetBits[first - lengthAlready + 1 + lengthMissing] & temp) == 0) //nuly na začátku by byly ztraceny
				if (lengthMissing == 0)//nahrazeno jen za 00
				{
					while ((SetBits[second] & temp) == 0) //nuly za druhou jedničkou + místo jedné jeničky
					{
						temp <<= 1;
					}
				}
				
					temp <<= 1; //nula za jedničku na začátku
				
				long newBoundary = temp.ToBoundary();
				if (IsValid(newBoundary))
				{

					if (
					ThreadSafeAdd(originalb, newBoundary))
					{
						return newBoundary;
					}
				}
			}
			return 0;

		}
		public bool ThreadSafeAdd(long value, long key)
		{
			if (!Boundaries.ContainsKey(key))
			{
				var dict = Boundaries;
				var modifiedDict = dict.Add(key,value);
				bool someOtherThreadHasAddedThis = false;
				while (!someOtherThreadHasAddedThis && Interlocked.CompareExchange(ref Boundaries, modifiedDict, dict) != dict)
				{
					dict = Boundaries;
					if (dict.ContainsKey(key))
					{
						someOtherThreadHasAddedThis = true;
					}
					modifiedDict = dict.Add(key, value);
				}
				if (!someOtherThreadHasAddedThis)
				{
					return true;
				}
			}
			return false;
		}
		public void SubstituteAndStartTaskForTwoBitsSet(long b, int first, int second)
		{
			int shorterSequenceOfNulls = first - second-1;
			int longerSequenceOfNulls = second;
			foreach (var face in FacesSizes)
			{
				if (shorterSequenceOfNulls+2==face && FacesSizes.Contains(longerSequenceOfNulls+2)) //koncový případ
				{
					long newBoundary = BoundaryLong.FaceToBoundary(face);
					if (ThreadSafeAdd(b, newBoundary))
					{
						Found = true;
					}
				}//koncový stav konec if
				if (shorterSequenceOfNulls + 2 + 1 < face) //musí se zvětšit o alespoň 2
				{ //jedničky se změní na nuly, zbydou nové jedničky + počet nul v delší sérii + 2
					long newBoundary = 0;
					for (int i = longerSequenceOfNulls + 2; i < longerSequenceOfNulls + face - shorterSequenceOfNulls; i++)
					{
						newBoundary |= SetBits[i];
					}
					if (IsValid(newBoundary) && ThreadSafeAdd(b, newBoundary))
					{
						FindTriarc(newBoundary);
					}
				}
				if (longerSequenceOfNulls + 2 + 1 < face) //musí se zvětšit o alespoň 2
				{ //jedničky se změní na nuly, zbydou nové jedničky + počet nul v delší sérii + 2
					long newBoundary = 0;
					for (int i = shorterSequenceOfNulls + 2; i < shorterSequenceOfNulls + face - longerSequenceOfNulls; i++)
					{
						newBoundary |= SetBits[i];
					}
				
					if (IsValid(newBoundary) && ThreadSafeAdd(b, newBoundary))
					{
						FindTriarc(newBoundary);
					}
				}
			}
		}

		public List<long> MainFindTriarc()
		{
			BoundariesToDo.Add(Boundary);

			Boundaries = Boundaries.Add(Boundary,Boundary);
			while (BoundariesToDo.Count!=0 && !Found)
			{
				long boundaryToDo = BoundariesToDo.First();
				BoundariesToDo.Remove(boundaryToDo);
				FindTriarc(boundaryToDo);
			}

		return	PrintSequenceOfStates();
		}
		public void FindTriarc(long b)
		{

			long originalb = b;
			int highest = b.OrderOfHighestSetBit();
			long highestValue = SetBits[highest];
			int i = 0;
			int second = (b - highestValue).OrderOfHighestSetBit();
			if (second == -1) //důležité, jinak by mohlo dojít ke smyčce
			{
				return;
			}
			if ((b^highestValue^SetBits[second])==0)
			{
				SubstituteAndStartTaskForTwoBitsSet(b, highest,  second);
				return;
			}
			while (i <= highest && highest > 0)
			{
				second = (b - highestValue).OrderOfHighestSetBit();
				// před jen procházecí cyklus
				foreach (var face in FacesSizes)
				{
				long newBoundary = SubstituteAndStartTask(b, highest, second,originalb, face);
					if (newBoundary==0)
					{
						continue;
					}
					
						BoundariesToDo.Add(newBoundary);
					
				}

				//po je procházecí cyklus
				//	int secondValue = HighestSetBit(l - highestValue);
				b = ((b ^ highestValue) << (highest - second)) | (highestValue >> (second + 1));
				i += highest - second;

			}

		}


		/// <summary>
		/// pokud je za sebou počet nul delší než nejdelší strana, je nevalidní
		/// </summary>
		/// <param name="b"></param>
		/// <returns></returns>
		public bool IsValid(long b)
		{
			int counter = 0;
			if ((b & SetBits[63]) == 0)
			{
				while (b != 0 )
				{
					if (counter > this.BiggestFace - 1)
					{
						return false;
					}
					if ((b & 1) == 0)
					{
						counter++;
					}
					else
					{
						counter = 0;
					}
					b >>= 1;
				}
				return true;
			}
			else //začíná jedničkou
			{
				return Faces.Contains(b);
			}
		}


		

		//triarc recreation
		public List<long> PrintSequenceOfStates()
		{
			var result = new List<long>();
			//	TextWriter tw = new StreamWriter(Name+".txt");
			TextWriter tw = Console.Out;
			tw.WriteLine(Name);
			foreach (var face in Faces)
			{
				if (Boundaries.ContainsKey(face))
				{

					tw.WriteLine(Convert.ToString( face,2));
					result.Add(face);
					long key = face;
					long value;
					Boundaries.TryGetValue(key, out value);
					tw.WriteLine(Convert.ToString(value, 2));
					result.Add(value);
					while (value!=Boundary)
					{
						key = value;
						Boundaries.TryGetValue(key, out value);
						tw.WriteLine(Convert.ToString(value, 2));
						result.Add(value);
					}
					
				}
			}
			tw.Flush();
			tw.Close();
			return result;
		}
	}
}
