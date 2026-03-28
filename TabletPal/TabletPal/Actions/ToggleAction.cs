using System.Threading.Tasks;
// using WindowsInput.Events;

namespace TabletPal.Actions
{
	public class ToggleAction : ButtonAction
	{
		public readonly string Key;

		public ToggleAction(string key)
		{
			Key = key;
		}
		
		
		public override async Task Invoke()
		{
			await ToggleManager.Toggle(Key);
		}

		public override void Dispose()
		{
			base.Dispose();

		}
	}
}
