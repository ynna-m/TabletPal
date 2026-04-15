using System;
using System.Threading.Tasks;

namespace TabletPal.Actions
{
	public class ToggleAction : ButtonAction
	{
		public readonly string[] Key;

		public ToggleAction(string[] key)
		{
			Key = key;
            Console.WriteLine("ToggleAction.cs - key: {0}", key);
		}
		
		
		public override void Invoke()
		{
			ToggleManager.Toggle(Key);
		}

		public override void Dispose()
		{
			base.Dispose();

		}
	}
}
