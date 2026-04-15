using System.Threading;
using System.Threading.Tasks;

namespace TabletPal.Actions
{
	public class WaitAction : ButtonAction
	{
		private int _delay;

		public WaitAction(int delay)
		{
			_delay = delay;
		}


		public override void Invoke() =>
			Thread.Sleep(_delay);
	}
}
