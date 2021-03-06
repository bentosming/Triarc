﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Triarc
{
	class Program
	{
		static void WhatToDoMessage()
		{
			Console.WriteLine();
			Console.WriteLine("- - - - - - - - - - T R I A R C S - - - - - - - - ");
			Console.WriteLine();
			Console.WriteLine("For finding out whether a triarc Exists, press E");
			Console.WriteLine("For Building and extracting a triarc, press R");
			Console.WriteLine("For examination of All triarc of given faces press A");
			Console.WriteLine("For examination of all (n,n,n) triarcs of given faces press N");
			Console.WriteLine("For examination of all (n,n,n-1) triarcs of given faces press M");
			Console.WriteLine();
			Console.WriteLine("- - - - - - - - - - B I A R C S - - - - - - - - - ");
			Console.WriteLine();
			Console.WriteLine("For examination of all (n,n) biarcs of given faces press B");
			Console.WriteLine();
			Console.WriteLine("- - - - - - - - - - A R C S - - - - - - - - - - - ");
			Console.WriteLine();
			Console.WriteLine("Fills given boundary G");

			Console.WriteLine("Enter Q to quit.");
			Console.WriteLine();

		}

		static List<int> ReadFaces()
		{

			Console.WriteLine("Enter sizes of faces. One per line, end by line with 0");
			Console.WriteLine();
			var faces = new List<int>();
			var input = Console.ReadLine();
			while (input != "0")
			{
				int face;
				if (
				int.TryParse(input, out face))
				{
					faces.Add(face);
				}
				input = Console.ReadLine();
			}
			return faces;
		}

		static void ReadTriarcSizes(out int x, out int y, out int z)
		{
			Console.WriteLine("Enter x from (x,y,z)-triarc.");
			int.TryParse(Console.ReadLine(), out x);
			Console.WriteLine("Enter y from (x,y,z)-triarc.");
			int.TryParse(Console.ReadLine(), out y);
			Console.WriteLine("Enter z from (x,y,z)-triarc.");
			int.TryParse(Console.ReadLine(), out z);
		}

		static void Help()
		{
			Console.WriteLine();
			Console.WriteLine("- - - - - - - - - - T R I A R C - - - - - - - - - - ");
			Console.WriteLine("- - - - - - - - - - - H E L P - - - - - - - - - - - ");
			Console.WriteLine();
			//		Console.WriteLine("-g .................. start GUI"); 
			Console.WriteLine("-t a b c ............ builds triarc of lengths abc");
			Console.WriteLine("-a x ................ builds arc of boudary x (has to start with 1)");
			Console.WriteLine("-n .................. finds parameters for all graphs needed");
			Console.WriteLine("                      to finish the proof for concrete faces");
			Console.WriteLine("-r .................. finds parameters for all graphs needed");
			Console.WriteLine("                      to finish the proof for all small sequences");
			Console.WriteLine("-f [list of ints] ... to specify face sizes");
			Console.WriteLine("-l x ................ to set maximum of transition spend on solving one graph");
			Console.WriteLine("                      default is " + Global.Limit);
			Console.WriteLine();
			Console.WriteLine("other parameters");
			Console.WriteLine("--ExportAsTutteSageScriptON");
			Console.WriteLine("--ExportAsSequenceOFF");
			Console.WriteLine("--ExportAsGraphVizON");
			Console.WriteLine("--ExportAsStandardGraphOFF");
			Console.WriteLine("--ExportFacesON");
			Console.WriteLine("--Count3ConnectivityON");
			Console.WriteLine("--ExportAsAll");
			Console.WriteLine("--ThreadCount x");
		}

		/// <summary>
		/// GUI, doesn't support some features, depricated.
		/// </summary>
		static void GUI()
		{


			char whatToDo;
			bool end = false;
			while (!end)
			{
				WhatToDoMessage();
				Console.WriteLine();
				whatToDo = char.ToLower(Console.ReadKey().KeyChar);
				Console.WriteLine();
				switch (whatToDo)
				{
					case 'e':
						#region Does triarc exist
						{
							int x;
							int y;
							int z;
							ReadTriarcSizes(out x, out y, out z);
							int limit = 0;
							bool limitEntered = false;
							while (!limitEntered)
							{
								Console.WriteLine("Enter limit that specifies how many boundaries to find before saying that triarc doesn't exist");
								limitEntered = int.TryParse(Console.ReadLine(), out limit);
							}
							if (Triarc.DoesTriarcExist(x, y, z, ReadFaces(), limit))
							{
								Console.WriteLine("Triarc exists!");
								Console.WriteLine();
							}
							else
							{
								Console.WriteLine("With this limit, triarc can't be found.");
							}

						}
						break;
					#endregion
					case 'r':
						#region Build and extract
						{
							int x;
							int y;
							int z;
							ReadTriarcSizes(out x, out y, out z);
							Triarc.FindAndBuildTriarc(x, y, z, ReadFaces());
						}
						break;
					#endregion
					case 'a':
						#region all triarcs of given faces
						{
							try
							{
								int limit = 0;
								bool limitEntered = false;
								while (!limitEntered)
								{
									Console.WriteLine("Enter limit that specifies how many boundaries to find before saying that triarc doesn't exist (0 for default)");
									limitEntered = int.TryParse(Console.ReadLine(), out limit);
									if (limit == 0)
									{
										limit = 100000;
									}
								}
								Triarc.DoTriarcsExist(Console.Out, ReadFaces(), limit);

							}
							catch (IOException)
							{
								Console.WriteLine("Invalid file name!");
							}
						}
						break;
					#endregion
					case 'n':
						#region nnn
						{
							Console.WriteLine("Output will be in file (n,n,n)TriarcsWithFaces[...]");
							int limit = 0;
							bool limitEntered = false;
							while (!limitEntered)
							{
								Console.WriteLine("Enter limit that specifies how many boundaries to find before saying that triarc doesn't exist");
								Console.WriteLine("0 for short default limit, 1 for medium default, 2 for long default");
								limitEntered = int.TryParse(Console.ReadLine(), out limit);
								if (limit == 0)
								{
									limit = 100000;
								}
								if (limit == 1)
								{
									limit = 1000000;
								}
								if (limit == 2)
								{
									limit = 10000000;
								}
							}
							Triarc.Do_nnn_TriarcsExist(ReadFaces(), limit);
						}
						break;
					#endregion
					case 'm':
						#region nnn-1
						{
							Console.WriteLine("Output will be in file (n,n,n-1)TriarcsWithFaces[...]");
							int limit = 0;
							bool limitEntered = false;
							while (!limitEntered)
							{
								Console.WriteLine("Enter limit that specifies how many boundaries to find before saying that triarc doesn't exist");
								Console.WriteLine("0 for short default limit, 1 for medium default, 2 for long default");
								limitEntered = int.TryParse(Console.ReadLine(), out limit);
								if (limit == 0)
								{
									limit = 100000;
								}
								if (limit == 1)
								{
									limit = 1000000;
								}
								if (limit == 2)
								{
									limit = 10000000;
								}
							}
							Triarc.Do_nnn1_TriarcsExist(ReadFaces(), limit);
						}
						break;
					#endregion
					case 'b':
						#region biarc nn
						{
							Console.WriteLine("Output will be in file (n,n)BiarcsWithFaces[...]");
							int limit = 0;
							bool limitEntered = false;
							while (!limitEntered)
							{
								Console.WriteLine("Enter limit that specifies how many boundaries to find before saying that triarc doesn't exist");
								Console.WriteLine("0 for short default limit, 1 for medium default, 2 for long default");
								limitEntered = int.TryParse(Console.ReadLine(), out limit);
								if (limit == 0)
								{
									limit = 100000;
								}
								if (limit == 1)
								{
									limit = 1000000;
								}
								if (limit == 2)
								{
									limit = 10000000;
								}
							}
							Triarc.DonnBiarcsExist(ReadFaces(), limit);
						}
						break;
					#endregion
					case 'g':
						#region from boundary
						{
							Console.WriteLine("Enter boundary se binary number");
							string boundaryString = Console.ReadLine();
							long boundary = Convert.ToInt64(boundaryString, 2);
							if (Triarc.DoesGeneralBoundaryExistAndConstruct(boundary, ReadFaces()))
							{
								Console.WriteLine("Exists");
							}
							else
							{
								Console.WriteLine("Doesn't exist");
							}
						}
						#endregion
						break;
					case 'q':
						#region QUIT
						{
							end = true;
							continue;
						}
					#endregion
					default:
						Console.WriteLine("Unvalid key pressed");
						break;
				}
				Console.ReadKey();
			}

		}

		static void Main(string[] args)
		{
			try
			{
				var ca = new ConsoleArguments(args);
				if (ca.Limit > 0)
				{
					Global.Limit = ca.Limit;
				}
				Global.TaskCount = ca.ThreadCount;
				Global.Count3Connectivity = ca.Count3Connectivity || ca.ExportAsAll;
				Global.ExportAsGraphViz = ca.ExportAsGraphViz || ca.ExportAsAll;
				Global.ExportAsTutteSageScript = ca.ExportAsTutteSageScript || ca.ExportAsAll;
				Global.ExportFaces = ca.ExportFaces || ca.ExportAsAll;
				Global.ExportAsSequence = !ca.ExportAsSequence || ca.ExportAsAll;
				Global.ExportAsStandardGraph = !ca.ExportAsStandardGraph || ca.ExportAsAll;

				ca.Validate();

				if (ca.GUI)
				{
					GUI();
				}
				else if (ca.Help)
				{
					Help();
				}
				else if (ca.Triarc)
				{
					if (!Triarc.FindAndBuildTriarc(ca.TriarcX, ca.TriarcY, ca.TriarcZ, ca.faces))
					{
						Console.WriteLine("Doesn't exist");
					}
				}
				else if (ca.Arc)
				{
					if (!Triarc.DoesGeneralBoundaryExistAndConstruct(ca.ArcBoundary, ca.faces))
					{
						Console.WriteLine("Doesn't exist");
					}
				}
				else if (ca.NeutralSequence)
				{
					var ns = new NeutralSequenceHelpingGraphs(ca.faces);
					ns.Find();
				}
				else if (ca.AllNeutralSequences)
				{
					#region AllNeutralSequences
					var sw = new StreamWriter("table.txt");
					for (int j = 7; j < 18; j++)
					{
						Console.WriteLine("................................");
						Console.WriteLine(j);
						Console.WriteLine("................................");
						Console.WriteLine();

						string firstLine = j.ToString();
						Func<List<int>, string> toFirst = ks => "(i): " + string.Join<int>(",", ks);
						string secondLine = "";
						Func<List<int>, string> toSecond = ksMinus => "(ii): " + string.Join<int>(",", ksMinus);
						string thirdLine = "";
						Func<string, string> toThird = ab => "AB : " + ab;
						string fourthLine = "";
						Func<string, string> toFourth = ab => "CD : " + ab;
						string zerothLine = j.ToString();
						Func<bool, string> toZeroth = found => found ? "Nalezeno" : "Nenalezeno";

						Func<string, string> join = line => line + " & ";

						for (int i = 3; i <= 5; i++)
						{
							zerothLine = join(zerothLine);
							firstLine = join(firstLine);
							secondLine = join(secondLine);
							thirdLine = join(thirdLine);
							fourthLine = join(fourthLine);

							var fc = new List<int> { i, j };
							var ns = new NeutralSequenceHelpingGraphs(fc) { slow = true };
							ns.Find();

							if (ns.found)
							{
								zerothLine += @"\cellcolor{lightgray}";
								firstLine += @"\cellcolor{lightgray}";
								secondLine += @"\cellcolor{lightgray}";
								thirdLine += @"\cellcolor{lightgray}";
								fourthLine += @"\cellcolor{lightgray}";
							}
							zerothLine += toZeroth(ns.found);
							firstLine += toFirst(ns.ks);
							secondLine += toSecond(ns.ksMinusOne);
							thirdLine += toThird(ns.AString);
							fourthLine += toFourth(ns.CDString);
						}
						//		sw.WriteLine(zerothLine + "\\\\");
						sw.WriteLine(firstLine + "\\\\");
						sw.WriteLine(secondLine + "\\\\");
						sw.WriteLine(thirdLine + "\\\\");
						sw.WriteLine(fourthLine + "\\\\" + "\\hline");
						sw.WriteLine();
					}
					sw.Close();

					#endregion
				}
				else {
					Help();
				}
			}

			catch (ArgumentException ex)
			{
				Console.WriteLine(ex.Message);
			}
			Console.ReadKey();
		}
	}
	static class Global
	{
		public static int Limit = 100000;
		public static int TaskCount = 1;
		public static bool ExportAsTutteSageScript = false;
		public static bool ExportAsSequence = true;
		public static bool ExportAsGraphViz = false;
		public static bool ExportAsStandardGraph = true;
		public static bool ExportFaces = false;
		public static bool Count3Connectivity = false;
	}
}





 