﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Triarc
{
	class NeutralSequenceHelpingGraphs
	{
		public List<int> FaceSizes;

		string path;
		public NeutralSequenceHelpingGraphs(List<int> fs)
		{
			this.FaceSizes = fs;
			path = Triarc.FacesToString(fs) + "\\";
		}

		public string CDString { get; private set; }
		public IEnumerable<string> Bstrings;
		public string AString { get; private set; }
		public int? k;

		public List<int> ks = new List<int>();
		public List<int> ksMinusOne = new List<int>();
		public bool found = false;
		public bool slow { get; set; }


		public List<string> strings = new List<string>() { "1010", "101010", "01001101", "10110010", "110010", "10101010", "10111010100","10000110010", "111000000010010110", "101010101101101", "101010101010" };

		public void Find()
		{

			FindCD();
			if (CDString != null || slow)
			{
				Bstrings = strings.Where(x => Triarc.DoesGeneralBoundaryExist(Convert.ToInt64(B(x), 2), FaceSizes));
			}
			if ((Bstrings != null && Bstrings.Count() > 0 && CDString != null) || slow) 
			{
				Findk();
			}

			if (AString != null && CDString != null & k.HasValue)
			{
				found = true;
				Console.WriteLine("----------------------------------------");
				Console.WriteLine("Nalezeno:");
				Console.WriteLine("AB řetízek: " + AString);
				Console.WriteLine("CD řežízek: " + CDString);
				Console.WriteLine("k: " + k);
				Console.WriteLine();
				Console.WriteLine("Saving all helping graphs");
				Triarc.FindAndBuildTriarc(k.Value, k.Value, k.Value, FaceSizes, path + "(k,k,k)\\");
				Triarc.FindAndBuildTriarc(k.Value, k.Value, k.Value - 1, FaceSizes, path + "(k,k,k-1)\\");
				Triarc.DoesGeneralBoundaryExistAndConstruct(Convert.ToInt64(A(AString, k.Value), 2), FaceSizes, path + "A\\");
				Triarc.DoesGeneralBoundaryExistAndConstruct(Convert.ToInt64(B(AString), 2), FaceSizes, path + "B\\");
				Triarc.DoesGeneralBoundaryExistAndConstruct(Convert.ToInt64(C(CDString), 2), FaceSizes, path + "C\\");
				Triarc.DoesGeneralBoundaryExistAndConstruct(Convert.ToInt64(D(CDString), 2), FaceSizes, path + "D\\");
				StreamWriter info = new StreamWriter("grafy\\" + path + "info.txt");
				info.WriteLine("Nalezeno:");
				info.WriteLine("AB řetízek: " + AString);
				info.WriteLine("CD řežízek: " + CDString);
				info.WriteLine("k: " + k);
				info.Close();
			}
			else
			{
				Console.WriteLine("----------------------------------------");
				Console.WriteLine("Nenalezeno.");
			}
		}

		bool FindCD()
		{
			foreach (var R in strings)
			{
				if (Triarc.DoesGeneralBoundaryExist(Convert.ToInt64(C(R), 2), this.FaceSizes))
				{
					if (Triarc.DoesGeneralBoundaryExist(Convert.ToInt64(D(R), 2), FaceSizes))
					{
						CDString = R;
						return true;
					}

				}
			}
			Console.WriteLine("CD řetízek nenalezen");
			return false;
		}

		bool Findk()
		{
			for (int k = 3; k <= 10; k++)
			{
			//	Console.WriteLine("k=" + k);
				if (Triarc.DoesTriarcExist(k, k, k, FaceSizes, 100000))
				{
					//Console.WriteLine("	Existuje (k,k,k)");
					ks.Add(k);
					if (Triarc.DoesTriarcExist(k, k, k - 1, FaceSizes, 100000))
					{
						ksMinusOne.Add(k);
						//Console.WriteLine("	Existuje (k,k,k-1)");

						if (Bstrings != null)
						{
							foreach (var r in Bstrings)
							{
								if (A(r, k).Count() < 64 && Triarc.DoesGeneralBoundaryExist(Convert.ToInt64(A(r, k), 2), FaceSizes))
								{
									//		Console.WriteLine("	A s řetízkem " + r + " existuje");
									this.k = k;
									this.AString = r;
									return true;
								}
							}
						}
					}
				}
			}
				return false;
		}

		string A(string R, int k)
		{
			var x = new StringBuilder();
			for (int i = 0; i < k - 1; i++)
			{
				x.Append("10");
			}
			x.Append("000");
			x.Append(R);
			x.Append("00");
			x.Append(reverse(R));
			x.Append("000");
			for (int i = 0; i < k - 1; i++)
			{
				x.Append("01");
			}
			x.Append("000");
			return x.ToString();
		}

		string B(string R)
		{
			var x = new StringBuilder();
			
				x.Append(R);
				x.Append("00");
				x.Append(reverse(R));
				x.Append("000");
			string y = x.ToString();
			//musí začínat "1", tak ji  najděme.

			return rotateToStartWithOne(y);
			
		}

		string rotateToStartWithOne(string y)
		{
			var beforeOne = new StringBuilder();
			for (int i = 0; i < y.Length && y[i] != '1'; i++)
			{
				beforeOne.Append(y[i]);
			}
			var result = new StringBuilder();
			int lengthBO = beforeOne.ToString().Length;
			for (int i = 0; i < y.Length - lengthBO; i++)
			{
				result.Append(y[i + lengthBO]);
			}
			result.Append(beforeOne);
			return result.ToString();
		}

		string C(string R)
		{
			var x = new StringBuilder();
			x.Append("10");
			x.Append(R);
			x.Append("010");
			x.Append(reverse(R));
			x.Append("0");
			return x.ToString();
		}

		string D(string R)
		{
			var x = new StringBuilder();
			x.Append("000");
			x.Append(R);
			x.Append("000");
			x.Append(reverse(R));
		var result = rotateToStartWithOne(x.ToString());
			return result;
		}

		string reverse(string R)
		{
			var x = new StringBuilder();
			for (int i = R.Length - 1; i >= 0; i--)
			{
				if (R[i] == '0')
				{
					x.Append("1");
				}
				else { x.Append("0"); }
			}
			return x.ToString();
		}
	}
}
