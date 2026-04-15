using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using TabletPal.InputSender;

namespace TabletPal
{
	public static class ToggleManager
	{
		public static HashSet<string[]> _heldKeys = new HashSet<string[]>();
		private static long _inputLocked;
        private static readonly IInputSender _inputSender = InputSenderFactory.Create();

		private static Dictionary<string, List<ToggleButton>> _buttons = new Dictionary<string, List<ToggleButton>>();

		public static void AddButton(string key, ToggleButton button)
		{
			List<ToggleButton> list;

			if (!_buttons.TryGetValue(key, out list))
			{
				list = new List<ToggleButton>();
				_buttons.Add(key, list);
			}

			list.Add(button);
		}


		public static void DisableAllButtons(string key)
		{
			if (_buttons.TryGetValue(key, out var list))
			{
				foreach (var button in list.ToArray())
				{
					Dispatcher.UIThread.Invoke(()=>
						{
							button.IsChecked = false;
						}
					);
				}
			}
		}


		private static ToggleButton[] GetButtons(string key)
		{
			if (_buttons.TryGetValue(key, out var list))
			{
				return list.ToArray();
			}
			return null;
		}


		private static void SetButtons(string key, bool active)
		{
			var buttons = GetButtons(key);
			if (buttons == null)
			{
				return;
			}
			foreach (var button in buttons)
			{
				if (button.IsChecked != active)
				{
					button.IsChecked = active;
				}
			}
		}


		public static void ClearButtons() =>
			_buttons = new Dictionary<string, List<ToggleButton>>();


		public static bool IsHeld(string[] key) =>
			_heldKeys.Contains(key);



		public static void Toggle(string[] keys)
		{
			Interlocked.Exchange(ref _inputLocked, 1);


            var keysJoin = string.Join("+", keys);
			if (!IsHeld(keys))
			{
                _inputSender.SendHold(keys);
				_heldKeys.Add(keys);
				SetButtons(keysJoin, true);
			}
			else
			{
				_inputSender.SendRelease(keys);
				_heldKeys.Remove(keys);
				SetButtons(keysJoin, false);
			}

			Interlocked.Exchange(ref _inputLocked, 0);
		}
	}
}
