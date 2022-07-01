using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Text;


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
        string content = Encoding.UTF8.GetString(pAsset.bytes);
        string[] lines = content.Split('\n');
        if (lines == null || lines.Length < 2) {
            return;
        }

        string[] keys = lines[0].Split('\t');
        int target = -1;
        for (int i = 1; i < keys.Length; ++i) {
            if (keys[i] == languageCode) {
                target = i;
                break;
            }
        }

        if (target == -1) {
            Debug.LogWarningFormat("Language code {0} missing in {1}", languageCode, pAsset.name);
            return;
        }

        for (int i = 1; i < lines.Length; ++i) {
            string[] values = lines[i].Split('\t');
            if (values.Length < keys.Length) {
                Debug.LogWarningFormat("Translation missing in {0}, line {1}", pAsset.name, i + 1);
            }
            else {
                mTranslation.Add(values[0], values[target]);
            }
        }
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
