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
		public bool AllNeutralSequences;

		public bool ExportAsAll;
		public bool ExportAsTutteSageScript = false;
		public bool ExportAsSequence = true;
		public bool ExportAsGraphViz = false;
		public bool ExportAsStandardGraph = true;
		public bool ExportFaces = false;
		public bool Count3Connectivity = false;

		string[] args;
		string input;

		public ConsoleArguments(string[] args)
		{
			GUI = args.Contains("-g");
			Triarc = args.Contains("-t");
			Arc = args.Contains("-a");
			Help = args.Contains("-h");
			NeutralSequence = args.Contains("-n");
			AllNeutralSequences = args.Contains("-r");
			ExportAsAll = args.Contains("--ExportAsAll");
			ExportAsTutteSageScript = args.Contains("--ExportAsTutteSageScriptON");
			ExportAsSequence = args.Contains("--ExportAsSequenceOFF");
			ExportAsGraphViz = args.Contains("--ExportAsGraphVizON");
			ExportAsStandardGraph = args.Contains("--ExportAsStandardGraphOFF");
			ExportFaces = args.Contains("--ExportFacesON");
			Count3Connectivity = args.Contains("--Count3ConnectivityON");

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
					throw new ArgumentException("Invalid arguments, missing face sizes");
				}
				return f;
			}
		}

		public int Limit
		{
			get
			{
				if (!args.Contains("-l"))
				{
					return -1;
				}
				int value;
				int.TryParse(args[Array.IndexOf<string>(args, "-l") + 1], out value);
				return value;

			}
		}


		public int ThreadCount
		{
			get
			{
				if (!args.Contains("--ThreadCount"))
				{
					return 1;
				}
				int value;
				int.TryParse(args[Array.IndexOf<string>(args, "--ThreadCount") + 1], out value);
				return value;

			}
		}

		public int TriarcX
		{
			get
			{
				int value;
				int.TryParse(args[Array.IndexOf<string>(args, "-t") + 1], out value);
				return value;
			}
		}
		public int TriarcY
		{
			get
			{
				int value;
				int.TryParse(args[Array.IndexOf<string>(args, "-t") + 2], out value);
				return value;
			}
		}
		public int TriarcZ
		{
			get
			{
					int value;
					int.TryParse(args[Array.IndexOf<string>(args, "-t") + 3], out value);
					return value;		
			}
		}

		public long ArcBoundary
		{
			get
			{
				try {
					return Convert.ToInt64(args[Array.IndexOf<string>(args, "-a") + 1], 2); }
				catch(Exception)
				{
					throw new ArgumentException("Invalid boundary");
				}
			}
		}

		public void Validate()
		{
			if (Triarc && (TriarcX <= 0 || TriarcY <= 0 || TriarcZ <= 0))
			{
				throw new ArgumentException("Ivalid arguments for triarc sizes.");
			}
			if (ThreadCount<=0)
			{
				throw new ArgumentException("Ivalid ThreadCount");
			}

		}
	}
}
