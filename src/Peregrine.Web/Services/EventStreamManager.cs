using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Peregrine.Web.Services
{
	class EventStreamManager
	{
		static readonly Dictionary<Guid, EventStreamManager> Instances = new Dictionary<Guid,EventStreamManager>();

		public static EventStreamManager GetInstance(Guid key)
		{
			if(!Instances.ContainsKey(key))
				Instances[key] = new EventStreamManager();

			return Instances[key];
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