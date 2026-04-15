using System.Threading.Tasks;

namespace TabletPal.Actions
{
	public class HideAction : ButtonAction
	{
		public override void Invoke()
		{
			EventBeacon.SendEvent(Events.ToggleMinimize);
		}
	}
}
