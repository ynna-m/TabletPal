using System.Threading.Tasks;
using TabletFriend.InputSender;
// using WindowsInput;

namespace TabletFriend.Actions
{
	public class TypeAction : ButtonAction
	{
		private readonly string _text;
        private readonly IInputSender _inputSender;

		public TypeAction(string text)
		{
			_text = text;
            _inputSender = InputSenderFactory.Create();
		}

		public override async Task Invoke() =>
			await _inputSender.SendClick(_text);
	}
}
