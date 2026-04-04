using System.Threading.Tasks;
using TabletPal.Docking;

namespace TabletPal.Actions
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
