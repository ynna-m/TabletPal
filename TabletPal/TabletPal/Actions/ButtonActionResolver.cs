using System;
using System.Collections.Generic;

namespace TabletPal.Actions
{
	public static class ButtonActionResolver
	{
		private const string _typeKeyword = "type ";
		private const string _toggleKeyword = "toggle ";
		private const string _terminalKeyword = "terminal ";
		private const string _waitKeyword = "wait ";
		private const string _holdKeyword = "hold ";
		private const string _releaseKeyword = "release ";
		private const string _layoutKeyword = "layout ";
		private const string _repeatKeyword = "repeat ";
		private const string _hideKeyword = "hide";

		private const string _dockKeyword = "dock ";
		private const string _undockKeyword = "undock";


		public static ButtonAction Resolve(string actionString)
		{
			if (string.IsNullOrEmpty(actionString))
			{
				return null;
			}
			actionString = actionString.Trim();

			if (actionString.StartsWith(_typeKeyword))
			{
				return ResolveTypeAction(actionString.Substring(_typeKeyword.Length));
			}
			if (actionString.StartsWith(_toggleKeyword))
			{
				return ResolveToggleAction(actionString.Substring(_toggleKeyword.Length));
			}
			if (actionString.StartsWith(_terminalKeyword))
			{
				return ResolveTerminalAction(actionString.Substring(_terminalKeyword.Length));
			}
			if (actionString.StartsWith(_waitKeyword))
			{
				return ResolveWaitAction(actionString.Substring(_waitKeyword.Length));
			}
			if (actionString.StartsWith(_holdKeyword))
			{
				return ResolveHoldAction(actionString.Substring(_holdKeyword.Length));
			}
			if (actionString.StartsWith(_releaseKeyword))
			{
				return ResolveReleaseAction(actionString.Substring(_releaseKeyword.Length));
			}
			if (actionString.StartsWith(_layoutKeyword))
			{
				return ResolveLayoutAction(actionString.Substring(_layoutKeyword.Length));
			}
			if (actionString.StartsWith(_repeatKeyword))
			{
				return ResolveRepeatAction(actionString.Substring(_repeatKeyword.Length));
			}
			if (actionString.StartsWith(_hideKeyword))
			{
				return ResolveHideAction();
			}
			if (actionString.StartsWith(_dockKeyword))
			{
				return ResolveDockAction(actionString.Substring(_dockKeyword.Length));
			}
			if (actionString.StartsWith(_undockKeyword))
			{
				return ResolveUndockAction();
			}
            return new KeyAction(KeyListArray(actionString));
		}
        private static string[] KeyListArray(string keyString)
		{
			try
			{
				var args = keyString.Replace(" ", "").Split("+");
				var keysList = new List<string>();

				foreach (var arg in args)
				{
					keysList.Add(Translate(arg));
				}

				return keysList.ToArray();
			}
			catch (Exception e)
			{
				throw new FormatException(
					"Error parsing '" + keyString + "'. Command or key combination may be invalid. "
					+ Environment.NewLine + e.Message
				);
			}
		}
		private static ButtonAction ResolveToggleAction(string actionString) =>
			new ToggleAction(KeyListArray(actionString));

		private static ButtonAction ResolveHoldAction(string actionString) =>
			new HoldAction(KeyListArray(actionString));

		private static ButtonAction ResolveReleaseAction(string actionString) =>
			new ReleaseAction(KeyListArray(actionString));

		private static ButtonAction ResolveTypeAction(string actionString) =>
			new TypeAction(actionString.Trim());

		private static ButtonAction ResolveTerminalAction(string actionString) =>
			new TerminalAction(actionString.Trim());

		private static ButtonAction ResolveWaitAction(string actionString) =>
			new WaitAction(int.Parse(actionString));

		private static ButtonAction ResolveLayoutAction(string actionString) =>
			new LayoutAction(actionString);
		
		private static ButtonAction ResolveRepeatAction(string actionString) =>
			new RepeatAction(KeyListArray(actionString));
		
		private static ButtonAction ResolveHideAction() =>
			new HideAction();
		
		private static ButtonAction ResolveDockAction(string actionString) =>
			new DockAction(actionString);
		
		private static ButtonAction ResolveUndockAction() =>
			new UndockAction();


		public static ButtonAction[] Resolve(string[] actionStrings)
		{
			var actions = new List<ButtonAction>();

			foreach (var actionString in actionStrings)
			{
				var action = Resolve(actionString);
				if (action != null)
				{
					actions.Add(action);
				}
			}

			return actions.ToArray();
		}
		private static string Translate(string inputKey)
		{
			if (_translationTable.TryGetValue(inputKey, out var outputKey))
			{
				return outputKey;
			}
			return inputKey;
		}

		/// <summary>
		/// Some enum entries don't look very pretty, so we translate them.
		/// </summary>
		private static Dictionary<string, string> _translationTable = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "Windows", "Super_L" },   // Linux “Super” key
            { "Win", "Super_L" },
            { "Shift", "Shift_L" },
            { "Ctrl", "Control_L" },
            { "Alt", "Alt_L" },
            { "0", "0" },
            { "1", "1" },
            { "2", "2" },
            { "3", "3" },
            { "4", "4" },
            { "5", "5" },
            { "6", "6" },
            { "7", "7" },
            { "8", "8" },
            { "9", "9" },
            { "Enter", "Return" },    // xdotool uses Return
            { "Esc", "Escape" },
            { "Backspace", "BackSpace" }
            // Add F1-F12 or others as needed
		};
	}
}
