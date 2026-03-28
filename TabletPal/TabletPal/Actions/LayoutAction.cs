using System.IO;
using System.Threading.Tasks;

namespace TabletPal.Actions
{
	public class LayoutAction : ButtonAction
	{
		private readonly string _layout;

		public LayoutAction(string layout)
		{
			_layout = layout;
		}

		public override Task Invoke()
		{
			EventBeacon.SendEvent(Events.ChangeLayout, Path.GetFileNameWithoutExtension(_layout));
			return Task.CompletedTask;
		}
	}
}
