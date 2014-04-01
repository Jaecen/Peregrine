using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Peregrine.Web.Services
{
	class EventStreamManager
	{
		static readonly ConcurrentDictionary<Tuple<string, string>, EventStreamManager> Instances;

		static EventStreamManager()
		{
			Instances = new ConcurrentDictionary<Tuple<string, string>, EventStreamManager>(
				new TupleEqualityComparer<string, string>(
					StringComparer.OrdinalIgnoreCase,
					StringComparer.OrdinalIgnoreCase
				)
			);
		}

		public static EventStreamManager GetInstance(string type, string key)
		{
			var keyTuple = Tuple.Create(type, key);

			if(!Instances.ContainsKey(keyTuple))
				Instances[keyTuple] = new EventStreamManager();

			return Instances[keyTuple];
		}

		readonly ConcurrentDictionary<Guid, StreamWriter> Listeners;

		private EventStreamManager()
		{
			Listeners = new ConcurrentDictionary<Guid, StreamWriter>();
		}

		public void AddListener(StreamWriter streamWriter)
		{
			Listeners.TryAdd(Guid.NewGuid(), streamWriter);
		}

		public void Publish(string eventName, object message)
		{
			foreach(var listenerKey in Listeners.Keys.ToArray())
			{
				var listener = Listeners[listenerKey];

				try
				{
					listener.WriteLine("event: {0}", eventName);
					listener.WriteLine("data: {0}", JsonConvert.SerializeObject(message));
					listener.WriteLine();
					listener.Flush();

					// For somereason, outputs lag one behind unless we write and flush a second time.
					listener.WriteLine();
					listener.Flush();
				}
				catch
				{
					Listeners.TryRemove(listenerKey, out listener);
				}
			}
		}
	}
}