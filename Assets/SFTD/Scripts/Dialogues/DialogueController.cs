using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueController : MonoBehaviour {
    [SerializeField] private float defaultSpeed = 1.0f;
    [SerializeField] private Canvas mDialogueCanvas;

    [SerializeField] private Image mAvatar;
    [SerializeField] private TMP_Text mName;
    [SerializeField] private TMP_Text mMainContent;
    // TODO: music

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
        SetAvatar("NPC", "happy");
    }

    public void StartDialogue() {
        mDialogueCanvas.gameObject.SetActive(true);
    }

    public void EndDialogue() {
        mDialogueCanvas.gameObject.SetActive(false);
    }

    public void SetAvatar(string name, string state = "") {
        var chara = CharacterDatabase.Instance.GetCharacter(name);
        AvatarInfo info = null;
        if (chara != null) {
            if (state.Length == 0) {
                info = chara.defaultInfo;
            }
            else {
                foreach (var ai in chara.states) {
                    if (ai.name == state) {
                        info = ai;
                        break;
                    }
                }
            }

            if (info != null) {
                mAvatar.sprite = info.avatar;
                mName.text = name;
                // TODO: font
                // TODO: music
            }
        }
    }
}
