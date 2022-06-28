using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueController : MonoBehaviour {
    [SerializeField] private float defaultSpeed = 1.0f;
    [SerializeField] private Canvas mDialogueCanvas;

    private static DialogueController instance;

    private void Assert<T>(T obj, string name) {
        if (obj == null) {
            Debug.LogErrorFormat("Error: {0} not found.", name);
        }
    }

    private void Awake() {
        instance = this;
        Assert(mDialogueCanvas, "Dialogue Canvas");
    }

    public static DialogueController Instance { 
        get { return instance; }
    }

    private void Start() {
        // for test only
        StartDialogue();
    }

    public void StartDialogue() {
        mDialogueCanvas.gameObject.SetActive(true);
    }

    public void EndDialogue() {
        mDialogueCanvas.gameObject.SetActive(false);
    }

    public void SetAvatar(string name, string state = "") {

    }
}
