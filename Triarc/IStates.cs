using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triarc
{

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
}