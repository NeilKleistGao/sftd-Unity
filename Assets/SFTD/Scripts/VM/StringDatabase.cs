using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringDatabase : MonoBehaviour {
    private static StringDatabase sInstance = null;
    [SerializeField] private string languageCode = "en_GB";

    private Dictionary<string, string> mTranslation = new Dictionary<string, string>();

    private void Awake() {
        sInstance = this;
    }

    public static StringDatabase Instance { 
        get { return sInstance; }
    }

    public string LanguageCode { 
        get { return languageCode; }
        set { languageCode = value; }
    }

    public void LoadI18NFile (TextAsset pAsset) {
        // TODO:
    }

    public string GetString(string pName) {
        if (mTranslation.ContainsKey(pName)) { 
            return mTranslation[pName];
        }

        Debug.LogErrorFormat("String \"{0}\" not Found", pName);
        return "";
    }
}
