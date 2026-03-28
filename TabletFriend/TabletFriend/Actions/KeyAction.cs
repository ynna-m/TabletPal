using System.Threading.Tasks;
using TabletPal.InputSender;
// using WindowsInput;
// using WindowsInput.Events;

namespace TabletPal.Actions
{
	public class KeyAction : ButtonAction
	{
		// private readonly KeyCode[] _keys;
        private readonly string _keys;
        private readonly IInputSender _inputSender;

		public KeyAction(string keys)
		{
			_keys = keys;
            _inputSender = InputSenderFactory.Create();
		}

		public override async Task Invoke() =>
			await _inputSender.SendChord(_keys);
	}
}
