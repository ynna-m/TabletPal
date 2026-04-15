using System.Threading.Tasks;
using TabletPal.InputSender;

namespace TabletPal.Actions
{
	public class KeyAction : ButtonAction
	{
        private readonly string[] _keys;
        private readonly IInputSender _inputSender;

		public KeyAction(string[] keys)
		{
			_keys = keys;
            _inputSender = InputSenderFactory.Create();
		}

		public override void Invoke() => 
			 _inputSender.SendChord(_keys);
	}
}
