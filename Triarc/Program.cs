using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Triarc
{
	class Program
	{
		static void  WhatToDoMessage()
		{
			Console.WriteLine();
			Console.WriteLine("- - - - - - - - - - T R I A R C S - - - - - - - - ");
			Console.WriteLine();
			Console.WriteLine("For finding out whether a triarc Exists, press E");
			Console.WriteLine("For Building and extracting a triarc, press B");
			Console.WriteLine("For examination of All triarc of given faces press A");
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

		static void Main(string[] args)
		{

			char whatToDo;
			bool end = false;
			while (!end)
			{
				WhatToDoMessage();
				Console.WriteLine();
				whatToDo = char.ToLower( Console.ReadKey().KeyChar);
				Console.WriteLine();
				switch (whatToDo)
				{
					case 'e':
						{
							int x;
							int y;
							int z;
							ReadTriarcSizes(out x,out y,out z);
							int limit=0;
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
					case 'b':
						{
							int x;
							int y;
							int z;
							ReadTriarcSizes(out x, out y, out z);
							Triarc.FindAndBuildTriarc(x, y, z, ReadFaces());
						}
						break;
					case 'a':
						{
							try
							{

								Console.WriteLine("Enter output file name");

								var writer = new StreamWriter(Console.ReadLine());
								Console.WriteLine();
								int limit = 0;
								bool limitEntered = false;
								while (!limitEntered)
								{
									Console.WriteLine("Enter limit that specifies how many boundaries to find before saying that triarc doesn't exist");
									limitEntered = int.TryParse(Console.ReadLine(), out limit);
								}
								Triarc.DoTriarcsExist(writer, ReadFaces(), limit);

							}
							catch (IOException)
							{
								Console.WriteLine("Invalid file name!");
							}}
						break;
					case 'q':
						{
							end = true;
							continue;
						}
					default:
						Console.WriteLine("Unvalid key pressed");
						break;
				}
				Console.ReadKey();
			}
			
		}
	}
		//	var writer = new StreamWriter("existing-5,7 Limit FFFF.txt");
		//	Triarc.DoTriarcsExist(writer,new int[2]{ 5,7},0xFFFF);
		//	writer.Close();

		//	Triarc.FindAndBuildTriarc(4,3,4, new List<int> { 4, 7 });
		//	Console.ReadKey();
		//	TriarcSolving bla = new TriarcSolving(4, 3, 4, new int[2] { 4, 7 });

	
	


}
