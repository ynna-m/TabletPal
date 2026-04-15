using System.Threading.Tasks;
using TabletPal.Docking;

namespace TabletPal.Actions
{
	public class UndockAction : ButtonAction
	{
		public override void Invoke()
		{
			EventBeacon.SendEvent(Events.DockingChanged, DockingMode.None);
		}
	}
}
