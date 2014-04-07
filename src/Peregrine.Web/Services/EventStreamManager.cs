using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Peregrine.Web.Services
{
	class EventStreamManager
	{
		static readonly ConcurrentDictionary<string, EventStreamManager> Instances;

		static EventStreamManager()
		{
			Instances = new ConcurrentDictionary<string, EventStreamManager>();
		}

		public static EventStreamManager GetInstance(string key)
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
					PublishTo(listener, eventName, message);
				}
				catch
				{
					Listeners.TryRemove(listenerKey, out listener);
				}
			}
		}

		public static void PublishTo(StreamWriter listener, string eventName, object message)
		{
			if(message == null)
				return;

			if(!String.IsNullOrEmpty(eventName))
				listener.WriteLine("event: {0}", eventName);

			listener.WriteLine("data: {0}", JsonConvert.SerializeObject(message));
			listener.WriteLine();
			listener.Flush();

			// For some reason, outputs lag one behind unless we write and flush a second time.
			listener.WriteLine();
			listener.Flush();
		}
	}
}