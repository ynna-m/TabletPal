using System.Threading.Tasks;

namespace TabletPal.Actions
{
	public class HideAction : ButtonAction
	{
		public override Task Invoke()
		{
			EventBeacon.SendEvent(Events.ToggleMinimize);
			return Task.CompletedTask;
		}
	}
}
