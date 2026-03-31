using System;
using System.Collections.Generic;

namespace TabletPal
{
	public static class EventBeacon
	{
		private static Dictionary<string, List<Action<object[]>>> _subscribers
			= new Dictionary<string, List<Action<object[]>>>();


		public static void Subscribe(string tag, Action<object[]> action)
		{
            Console.WriteLine($"EventBeacon.cs - Subscribing to event: {tag} with action: {action}");
			if (!_subscribers.TryGetValue(tag, out var subscribers))
			{
				subscribers = new List<Action<object[]>>();
				_subscribers.Add(tag, subscribers);
			}
			subscribers.Add(action);
		}


		public static void SendEvent(string tag, params object[] args)
		{
            Console.WriteLine($"EventBeacon.cs - Sending event: {tag} with args: {string.Join(", ", args)}");
			if (_subscribers.TryGetValue(tag, out var subscribers))
			{
				foreach (var subscriber in subscribers.ToArray())
				{
					subscriber?.Invoke(args);
				}
			}
		}
	}
}
