using System.Threading.Tasks;
using TabletFriend.InputSender;
// using WindowsInput;
// using WindowsInput.Events;

namespace TabletFriend.Actions
{
	public class ReleaseAction : ButtonAction
	{
		private readonly string _keys;
        private readonly IInputSender _inputSender;

		public ReleaseAction(string keys)
		{
			_keys = keys;
            _inputSender = InputSenderFactory.Create();
		}

		public override async Task Invoke() =>
			await _inputSender.SendRelease(_keys);
	}
}
