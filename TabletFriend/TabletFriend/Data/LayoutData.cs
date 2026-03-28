using System.Collections.Generic;

namespace TabletPal.Data
{
	public class LayoutData
	{
		public int LayoutWidth;

		public int? ButtonSize;
		public int? Margin;
		public string MinOpacity;
		public string MaxOpacity;

		public string App;

		public Dictionary<string, ButtonData> Buttons;
	}
}
