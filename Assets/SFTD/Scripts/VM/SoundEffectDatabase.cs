using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SoundEffect {
    public string name;
    public AudioClip clip;
}

public class SoundEffectDatabase : MonoBehaviour {
    [SerializeField] private SoundEffect[] effects;

    private static SoundEffectDatabase sInstance;

    private Dictionary<string, int> mIndexMapping = new Dictionary<string, int>();

    public static SoundEffectDatabase Instance { 
        get { return sInstance; }
    }

    private void Awake() {
        sInstance = this;
        if (effects == null || effects.Length == 0) {
            return;
        }

        for (int i = 0; i < effects.Length; ++i) { 
            mIndexMapping[effects[i].name] = i;
        }
    }

    public AudioClip GetAudioClip(string name) {
        if (mIndexMapping.ContainsKey(name)) {
            return effects[mIndexMapping[name]].clip;
        }

        Debug.LogErrorFormat("Audio Clip {0} not Found.", name);
        return null;
    }
}
