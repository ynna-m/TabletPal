using System.Threading.Tasks;
using TabletPal.InputSender;

namespace TabletPal.Actions
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

		public override void Invoke() =>
			_inputSender.SendClick(_text);
	}
}
