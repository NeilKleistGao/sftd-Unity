using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

[System.Serializable]
public class SelectEvent : UnityEvent<int> { }

public class DialogueController : MonoBehaviour {
    [SerializeField] private float defaultSpeed = 1.0f;
    [SerializeField] private Canvas dialogueCanvas;

    [SerializeField] private Image avatarImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text mainContentText;
    [SerializeField] private TMP_FontAsset defaultFont;
    [SerializeField] private AudioSource effectPlayer;
    [SerializeField] private AudioClip defaultEffect;
    [SerializeField] private GameObject optionPrefab;
    [SerializeField] private GameObject optionsParent;
    [SerializeField] private string moveUpKey;
    [SerializeField] private string moveDownKey;
    [SerializeField] private string submitKey;

    private SelectEvent mSelectEvent = new SelectEvent();

    private float mSpeed = 1.0f;
    private Coroutine mTextCoroutine = null;
    private Coroutine mSelectingCoroutine = null;
    private int mSelectedIndex = -1;
    private int mOptionsSize = 0;
    private DialogueOption[] mOptions = null;

    private static DialogueController instance;

    public SelectEvent OnSelecting {
        get { return mSelectEvent; }
        set { mSelectEvent = value; }
    }

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
        Assert(optionPrefab, "Option Prefab");
        Assert(optionsParent, "Options Parent");
        Assert(optionPrefab.GetComponent<DialogueOption>(), "Dialogue Option Component in Option Prefab");
    }

    public static DialogueController Instance { 
        get { return instance; }
    }

    private void Start() {
        // for test only
        StartDialogue();
        ShowOptions(new string[] { "test1", "test2", "test3" });
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

    public void ShowOptions(string[] pOptions) {
        mSelectedIndex = 0; mOptionsSize = pOptions.Length;
        mOptions = new DialogueOption[mOptionsSize];
        for (int i = 0; i < mOptionsSize; ++i) {
            var obj = Instantiate(optionPrefab);
            var optionScript = obj.GetComponent<DialogueOption>();
            optionScript.Initialize(i, pOptions[i]);
            var trans = obj.GetComponent<RectTransform>();
            trans.SetParent(optionsParent.GetComponent<RectTransform>());
            trans.localScale = new Vector3(1, 1, 1);
            mOptions[i] = optionScript;
        }

        mOptions[0].Select(mSelectedIndex);
        mSelectingCoroutine = StartCoroutine(Select());
    }

    private IEnumerator Select() {
        while (true) {
            if (Mathf.Abs(Input.GetAxis(moveUpKey)) >= 0.1f) {
                int prev = mSelectedIndex;
                mSelectedIndex = (mSelectedIndex + mOptionsSize - 1) % mOptionsSize;
                mOptions[prev].Select(mSelectedIndex);
                mOptions[mSelectedIndex].Select(mSelectedIndex);
                Input.ResetInputAxes();
            }
            else if (Mathf.Abs(Input.GetAxis(moveDownKey)) >= 0.1f) {
                int prev = mSelectedIndex;
                mSelectedIndex = (mSelectedIndex + 1) % mOptionsSize;
                mOptions[prev].Select(mSelectedIndex);
                mOptions[mSelectedIndex].Select(mSelectedIndex);
                Input.ResetInputAxes();
            }
            else if (Mathf.Abs(Input.GetAxis(submitKey)) >= 0.1f) {
                break;
            }

            yield return new WaitForSecondsRealtime(0.1f);
        }

        mSelectEvent.Invoke(mSelectedIndex);
        mSelectedIndex = -1;
        mOptionsSize = 0;

        foreach (var op in mOptions) {
            var obj = op.gameObject;
            DestroyImmediate(obj.gameObject);
        }

        mOptions = null;
    }
}
