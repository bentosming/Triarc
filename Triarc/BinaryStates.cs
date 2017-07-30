using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triarc
{
	class TooManyVerticesToPutInStateException : Exception
	{ }

	class LongBinaryStatesWithHashSet : IStates<long>, IStates
	{
		/// <summary>
		/// Collection of all 
		/// </summary>
		HashSet<long> states = new HashSet<long>();
		public bool Add(IList<TriarcGraph.VertexStack> list)
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
		public long VerticesToState(IList<TriarcGraph.VertexStack> list)
		{
			Func<TriarcGraph.VertexStack, long> vertexToLong = x =>
			{
				if (x.HasAllThreeNeighbours())
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
		public static long VerticesToStateStatic(IList<TriarcGraph.VertexStack> list)
		{
			Func<TriarcGraph.VertexStack, long> vertexToLong = x =>
			{
				if (x.HasAllThreeNeighbours())
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

		public string VerticesToString(IList<TriarcGraph.VertexStack> list)
		{
			return Convert.ToString(VerticesToState(list), 2);
		}
	}

}
