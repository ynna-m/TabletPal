using System.Threading.Tasks;
// using WindowsInput;
// using WindowsInput.Events;

namespace TabletFriend.Actions
{
	public class KeyAction : ButtonAction
	{
		// private readonly KeyCode[] _keys;
        private readonly string _keys;

		public KeyAction(string keys)
		{
			_keys = keys;
		}

		public override async Task Invoke() =>
			await Simulate.Events().ClickChord(_keys).Invoke();
	}
}
