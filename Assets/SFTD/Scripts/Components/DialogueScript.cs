using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class DialogueScript : MonoBehaviour {
    [SerializeField] private TextAsset script;

    private DialogueILPack mIL;

    private void Start() {
        var interpreter = Interpreter.Instance;
        if (interpreter == null) { 
            Debug.LogError("No interpreter has been set in the scene.");
            return;
        }

        if (script == null) {
            Debug.LogError("No dialogue script has been assigned.");
        }

        mIL = interpreter.LoadScript(script.bytes);
    }
}
