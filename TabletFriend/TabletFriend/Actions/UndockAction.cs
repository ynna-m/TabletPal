using System.Threading.Tasks;
// using WpfAppBar;
using TabletFriend.Docking;

namespace TabletFriend.Actions
{
	public class UndockAction : ButtonAction
	{
		public override Task Invoke()
		{
			EventBeacon.SendEvent(Events.DockingChanged, DockingMode.None);
			return Task.CompletedTask;
		}
	}
}
