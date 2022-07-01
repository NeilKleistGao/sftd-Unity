using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class LanguageCodeChangedEvnet : UnityEvent { }

public class StringDatabase : MonoBehaviour {
    private static StringDatabase sInstance = null;
    [SerializeField] private string languageCode = "en_GB";
    private LanguageCodeChangedEvnet mChangedEvent = new LanguageCodeChangedEvnet();

    private Dictionary<string, string> mTranslation = new Dictionary<string, string>();

    private void Awake() {
        sInstance = this;
    }

    public static StringDatabase Instance { 
        get { return sInstance; }
    }

    public string LanguageCode { 
        get { return languageCode; }
        set { languageCode = value; mChangedEvent.Invoke(); }
    }

    public LanguageCodeChangedEvnet OnLanguageCodeChanged { 
        get { return mChangedEvent; }
        set { mChangedEvent = value; }
    }

    public void LoadI18NFile (TextAsset pAsset) {
        // TODO:
    }

    public string GetString(string pName) {
        if (mTranslation.ContainsKey(pName)) { 
            return mTranslation[pName];
        }

        return pName;
    }

    public void Clear() { 
        mTranslation.Clear();
    }
}
