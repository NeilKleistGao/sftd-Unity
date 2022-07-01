using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueController : MonoBehaviour {
    [SerializeField] private float defaultSpeed = 1.0f;
    [SerializeField] private Canvas dialogueCanvas;

    [SerializeField] private Image avatarImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text mainContentText;
    [SerializeField] private TMP_FontAsset defaultFont;
    [SerializeField] private AudioSource effectPlayer;
    [SerializeField] private AudioClip defaultEffect;

    private float mSpeed = 1.0f;
    private Coroutine mTextCoroutine = null;

    private static DialogueController instance;

    private void Assert<T>(T pObj, string pName) {
        if (pObj == null) {
            Debug.LogErrorFormat("Error: {0} not found.", pName);
        }
    }

    private void Awake() {
        instance = this;
        Assert(dialogueCanvas, "Dialogue Canvas");
        Assert(avatarImage, "Avatar Image");
        Assert(nameText, "Name Text");
        Assert(mainContentText, "Main Content Text");
        Assert(defaultFont, "Default Font");
        Assert(effectPlayer, "Effect Player");
    }

    public static DialogueController Instance { 
        get { return instance; }
    }

    private void Start() {
        // for test only
        StartDialogue();
    }

    public void StartDialogue() {
        dialogueCanvas.gameObject.SetActive(true);
    }

    public void EndDialogue() {
        dialogueCanvas.gameObject.SetActive(false);
    }

    public void SetAvatar(string pName, string pState = "") {
        var chara = CharacterDatabase.Instance.GetCharacter(pName);
        AvatarInfo info = null;
        if (chara != null) {
            if (pState.Length == 0) {
                info = chara.defaultInfo;
            }
            else {
                foreach (var ai in chara.states) {
                    if (ai.name == pState) {
                        info = ai;
                        break;
                    }
                }
            }

            if (info != null) {
                avatarImage.sprite = info.avatar;
                nameText.text = name;

                if (info.font == null) {
                    mainContentText.font = defaultFont;
                }
                else { 
                    mainContentText.font = info.font;
                }

                if (info.sound == null) {
                    effectPlayer.clip = defaultEffect;
                }
                else {
                    effectPlayer.clip = info.sound;
                }
            }
        }
    }

    public float ShowText(string pText, float pTime = -1.0f) {
        mainContentText.text = StringProcessor.ReplaceWithVariables(pText);
        float time = pTime;
        if (time < 0) {
            mSpeed = defaultSpeed;
            time = mainContentText.text.Length / mSpeed;
        }
        else if (time > 0) {
            mSpeed = mainContentText.text.Length / time;
        }

        if (time > 0) {
            mainContentText.maxVisibleCharacters = 0;
            mTextCoroutine = StartCoroutine(TypeCharacter());
        }

        return time;
    }

    public void Skip() {
        if (mTextCoroutine != null) {
            mainContentText.maxVisibleCharacters = mainContentText.text.Length;
            StopCoroutine(mTextCoroutine);
            mTextCoroutine = null;
        }
    }

    private IEnumerator TypeCharacter() {
        while (mainContentText.maxVisibleCharacters < mainContentText.text.Length) {
            if (effectPlayer.clip != null) {
                effectPlayer.Play();
            }
            
            ++mainContentText.maxVisibleCharacters;
            yield return new WaitForSeconds(1.0f / mSpeed);
        }
    }
}
