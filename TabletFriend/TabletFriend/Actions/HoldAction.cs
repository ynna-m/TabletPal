using System.Threading.Tasks;
using TabletFriend.InputSender;
// using WindowsInput;
// using WindowsInput.Events;

namespace TabletFriend.Actions
{
	public class HoldAction : ButtonAction
	{
		private readonly string _keys;
        private readonly IInputSender _inputSender;

		public HoldAction(string keys)
		{
			_keys = keys;
            _inputSender = InputSenderFactory.Create();

		}
		public override async Task Invoke() =>
			// await Simulate.Events().Hold(_keys).Invoke();
            await _inputSender.SendHold(_keys);
	}
}
