using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DunGen
{
	[Serializable]
	public sealed class TileInjectionRule
	{
		public TileSet TileSet;
		public FloatRange NormalizedPathDepth = new FloatRange(0, 1);
		public FloatRange NormalizedBranchDepth = new FloatRange(0, 1);
		public bool CanAppearOnMainPath = true;
		public bool CanAppearOnBranchPath = false;
		public bool IsRequired = false;
	}
}
