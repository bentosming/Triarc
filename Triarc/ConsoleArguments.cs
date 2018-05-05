using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triarc
{
	class ConsoleArguments
	{
		public bool GUI;
		public bool Triarc;
		public bool Arc;
		public bool Help;
		public bool NeutralSequence;
		string[] args;
		string input;

		public ConsoleArguments(string[] args)
		{
			GUI = args.Contains("-g");
			Triarc = args.Contains("-t");
			Arc = args.Contains("-a");
			Help = args.Contains("-h");
			NeutralSequence = args.Contains("-n");
			this.args = args;
			input = string.Join(" ", args);
		}

		public List<int> faces
		{
			get
			{
				int j = Array.IndexOf<string>(args, "-f")+1;
				List<int> f = new List<int>();
				int temp;
				while (args.Count() > j && int.TryParse(args[j], out temp))
				{
					f.Add(temp);
					j++;
				}
				if (f.Count() == 0)
				{
					Console.WriteLine("Invalid arguments, missing face sizes");
				}
				return f;
			}
		}

		public int Limit
		{
			get
			{
				try
				{
					int value;
					int.TryParse(args[Array.IndexOf<string>(args, "-l") + 1], out value);
					return value;
				}
				catch (Exception)
				{
					Console.WriteLine("Invalid argument, LIMIT");
					throw new ArgumentException("LIMIT");
				}
			}
		}

		public int TriarcX
		{
			get
			{
				try
				{
					int value;
					int.TryParse(args[Array.IndexOf<string>(args, "-t") + 1], out value);
					return value;
				}
				catch (Exception)
				{
					Console.WriteLine("Invalid argument, TRIARC LENGTH 1");
					throw new ArgumentException("TRIARC LENGTH 1");
				}
			}
		}
		public int TriarcY
		{
			get
			{
				try
				{
					int value;
					int.TryParse(args[Array.IndexOf<string>(args, "-t") + 3], out value);
					return value;
				}
				catch (Exception)
				{
					Console.WriteLine("Invalid argument, TRIARC LENGTH 3");
					throw new ArgumentException("TRIARC LENGTH 3");
				}
			}
		}
		public int TriarcZ
		{
			get
			{
				try
				{
					int value;
					int.TryParse(args[Array.IndexOf<string>(args, "-t") + 3], out value);
					return value;
				}
				catch (Exception)
				{
					Console.WriteLine("Invalid argument, TRIARC LENGTH 3");
					throw new ArgumentException("TRIARC LENGTH 3");
				}
			}
		}

		public long ArcBoundary
		{
			get
			{
				return Convert.ToInt64(args[Array.IndexOf<string>(args, "-a") + 1], 2);
			}
		} 
	}
}
