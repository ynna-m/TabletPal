using System.Threading.Tasks;
// using WindowsInput;
// using WindowsInput.Events;

namespace TabletFriend.Actions
{
	public class HoldAction : ButtonAction
	{
		private readonly string _keys;

		public HoldAction(string keys)
		{
			_keys = keys;
		}

		public override async Task Invoke() =>
			await Simulate.Events().Hold(_keys).Invoke();
	}
}
