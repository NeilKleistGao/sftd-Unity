using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UserEvent : UnityEvent<List<VariableData>> { }

[System.Serializable]
public struct EventData { 
    public string Name;
    public UserEvent userEvent;
}

public class EventsDatabase : MonoBehaviour {
    [SerializeField] private EventData[] events;
    private Dictionary<string, int> mNameMapping = new Dictionary<string, int>();

    public static EventsDatabase Instance { get; private set; }

    private void Awake() {
        Instance = this;
        if (events == null || events.Length == 0) {
            return;
        }

        for (int i = 0; i < events.Length; ++i) {
            mNameMapping.Add(events[i].Name, i);
        }
    }

    public void Publish(string name, List<VariableData> variables) {
        if (!mNameMapping.ContainsKey(name)) {
            Debug.LogErrorFormat("Event {0} not found.", name);
            return;
        }

        events[mNameMapping[name]].userEvent.Invoke(variables);
    }
}
