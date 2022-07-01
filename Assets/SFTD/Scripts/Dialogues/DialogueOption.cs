using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueOption : MonoBehaviour {
    [SerializeField] private TMP_Text mText;
    [SerializeField] private Color mDefaultColor;
    [SerializeField] private Color mSelectedColor;

    private int mIndex;

    private void Awake() {
        if (mText == null) {
            Debug.LogError("Text in Dialogue Option not Found.");
        }
    }

    public void Initialize(int pIndex, string mString) { 
        mIndex = pIndex;
        mText.text = mString;
        mText.color = mDefaultColor;
    }

    public void Select(int pCurrent) {
        if (mIndex == pCurrent) {
            mText.color = mSelectedColor;
        }
        else { 
            mText.color = mDefaultColor;
        }
    }
}
