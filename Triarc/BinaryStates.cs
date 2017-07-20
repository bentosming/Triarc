using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triarc
{
	class TooManyVerticesToPutInStateException : Exception
	{ }
	/// <summary>
	/// Pamatuje si stavy v hashsetu intů a to ne všechny rotace, ale jen největší z nich. Pokud Je seznam vrcholů delší než 31, vyhodí chybu.
	/// </summary>
	class IntBinaryStatesWithHashSet : IStates<int>, IStates
	{
		HashSet<int> states = new HashSet<int>();
		public bool Add(IList<VertexStack> list)
		{
			return Add(this.VerticesToState(list));
		}

		public int Count()
		{
			return states.Count;
		}



		public bool Add(int item)
		{
			return states.Add(item);

		}

		/// <summary>
		/// list musí mít 31 a méně vrcholů
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		public int VerticesToState(IList<VertexStack> list)
		{
			Func<VertexStack, int> vertexToInt = x =>
			{
				if (x.HasAllTreeNeighbours())
				{
					return 0;
				}
				return 1;
			};


			if (list.Count > 31)
			{
				throw new TooManyVerticesToPutInStateException();
			}
			int temp = 0;
			for (int i = 0; i < list.Count; i++)
			{
				temp = (temp << 1) | vertexToInt(list[i]);
			}
			var max = temp;
			int length = 0;
			for (int i = 0; i < list.Count; i++)
			{
				length = (length << 1) | 1;
			}

			for (int i = 0; i < list.Count; i++)
			{
				temp = ((temp << 1) | vertexToInt(list[i])) & length; //posune o jedna, nastaví poslední bit a pak ořízne vše před
				if (temp > max)
				{
					max = temp;
				}
			}
			//teď není hotovo, protože potřebujeme rozlišovat stavy typu "000" a "0000", 
			//hodnota bude jedničky všude tam, kde nemá být nula, díky znaménkovému bitu je jednoznačné
			if (max == 0)
			{
				return ~length; //length = jedničky požadované délky, ~ je binární not
			}
			return max;
		}

		public string VerticesToString(IList<VertexStack> list)
		{
			return Convert.ToString(VerticesToState(list), 2);
		}
	}


	class LongBinaryStatesWithHashSet : IStates<long>, IStates
	{
		HashSet<long> states = new HashSet<long>();
		public bool Add(IList<VertexStack> list)
		{
			return Add(this.VerticesToState(list));
		}

		public int Count()
		{
			return states.Count;
		}



		public bool Add(long item)
		{
			return states.Add(item);

		}

		/// <summary>
		/// list musí mít 63 a méně vrcholů
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		public long VerticesToState(IList<VertexStack> list)
		{
			Func<VertexStack, long> vertexToLong = x =>
			{
				if (x.HasAllTreeNeighbours())
				{
					return 0;
				}
				return 1;
			};


			if (list.Count > 63)
			{
				throw new TooManyVerticesToPutInStateException();
			}
			long temp = 0;
			for (int i = 0; i < list.Count; i++)
			{
				temp = (temp << 1) | vertexToLong(list[i]);
			}
			var max = temp;
			long length = 0;
			for (long i = 0; i < list.Count; i++)
			{
				length = (length << 1) | 1;
			}

			for (int i = 0; i < list.Count; i++)
			{
				temp = ((temp << 1) | vertexToLong(list[i])) & length; //posune o jedna, nastaví poslední bit a pak ořízne vše před
				if (temp > max)
				{
					max = temp;
				}
			}
			//teď není hotovo, protože potřebujeme rozlišovat stavy typu "000" a "0000", 
			//hodnota bude jedničky všude tam, kde nemá být nula, díky znaménkovému bitu je jednoznačné
			if (max == 0)
			{
				return ~length; //length = jedničky požadované délky, ~ je binární not
			}
			return max;
		}
		public static long VerticesToStateStatic(IList<VertexStack> list)
		{
			Func<VertexStack, long> vertexToLong = x =>
			{
				if (x.HasAllTreeNeighbours())
				{
					return 0;
				}
				return 1;
			};


			if (list.Count > 63)
			{
				throw new TooManyVerticesToPutInStateException();
			}
			long temp = 0;
			for (int i = 0; i < list.Count; i++)
			{
				temp = (temp << 1) | vertexToLong(list[i]);
			}
			var max = temp;
			long length = 0;
			for (long i = 0; i < list.Count; i++)
			{
				length = (length << 1) | 1;
			}

			for (int i = 0; i < list.Count; i++)
			{
				temp = ((temp << 1) | vertexToLong(list[i])) & length; //posune o jedna, nastaví poslední bit a pak ořízne vše před
				if (temp > max)
				{
					max = temp;
				}
			}
			//teď není hotovo, protože potřebujeme rozlišovat stavy typu "000" a "0000", 
			//hodnota bude jedničky všude tam, kde nemá být nula, díky znaménkovému bitu je jednoznačné
			if (max == 0)
			{
				return ~length; //length = jedničky požadované délky, ~ je binární not
			}
			return max;
		}

		public string VerticesToString(IList<VertexStack> list)
		{
			return Convert.ToString(VerticesToState(list), 2);
		}
	}

}
