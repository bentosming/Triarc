using System;
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
			Console.WriteLine("-g .................. start GUI");
			Console.WriteLine("-t a b c ............ builds triarc of lengths abc");
			Console.WriteLine("-a x ................ builds arc of boudary x (has to start with 1)");
			Console.WriteLine("-f [list of ints] ... to specify face sizes");
		}

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

								Console.WriteLine("Enter output file name");

								//	var writer = new StreamWriter(Console.ReadLine());
								//	Console.WriteLine();
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
							if (Triarc.DoesGeneralBoundaryExistAndConstruct(boundary, ReadFaces(), 1000000))
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
			var ca = new ConsoleArguments(args);
			try
			{

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
					Triarc.FindAndBuildTriarc(ca.TriarcX, ca.TriarcY, ca.TriarcZ, ca.faces);
				}
				else if (ca.Arc)
				{
					if (!Triarc.DoesGeneralBoundaryExistAndConstruct(ca.ArcBoundary, ca.faces, 1000000))
					{
						Console.WriteLine("Doesn't exist");
					}
				}
				else { Console.WriteLine("try -h"); }
			}

			
			catch (ArgumentException ex)
			{
				Console.WriteLine(ex.Message);
			}
			Console.ReadKey();
		}
	}



	//	var writer = new StreamWriter("existing-5,7 Limit FFFF.txt");
	//	Triarc.DoTriarcsExist(writer,new int[2]{ 5,7},0xFFFF);
	//	writer.Close();

	//	Triarc.FindAndBuildTriarc(4,3,4, new List<int> { 4, 7 });
	//	Console.ReadKey();
	//	TriarcSolving bla = new TriarcSolving(4, 3, 4, new int[2] { 4, 7 });


	/*
	string[] parts = new string[7] {
				"1000010100010",
				"10000101010001010",
				"100001010101000101010",
				"1000010101010100010101010",
				"10000101010101010001010101010",
				"100001010101010101000101010101010",
				"1000010101010101010100010101010101010" };
			foreach (var item in parts)
			{
				//var item = "1000010100010";
				Console.WriteLine(item);
				long boundary = Convert.ToInt64(item, 2);
				if (Triarc.DoesGeneralBoundaryExistAndConstruct(boundary, new List<int> { 4, 7 }, 1000000))
				{
					Console.WriteLine("Exists");
				}
				else
				{
					Console.WriteLine("Doesn't exist");
				}
				*/
}





 