using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class DialogueScript : MonoBehaviour {
    [SerializeField] private TextAsset script;
    [SerializeField] private TextAsset i18n;
    [SerializeField] private string interactKey = "Submit";

    private struct StackValue {
        public int pointer;
        public DialogueData data;
    }

    private Stack<StackValue> mStack = new Stack<StackValue>();

    private DialogueILPack mIL;
    private DialogueData mCurrentDialogue;
    private int mPointer = -1;
    private bool mTriggered = false;
    private bool mInteracting = false;

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

        var stringDatabase = StringDatabase.Instance;
        if (stringDatabase == null) {
            Debug.LogError("No string database has been set in the scene.");
            return;
        }

        if (i18n != null) {
            stringDatabase.LoadI18NFile(i18n);
            stringDatabase.OnLanguageCodeChanged.AddListener(OnLanguageCodeChanged);
        }
    }

    private void OnLanguageCodeChanged() {
        StringDatabase.Instance.LoadI18NFile(i18n);
    }

    private bool TryToStart(ref Dictionary<int, DialogueData> pList, bool pAuto = false) {
        var interpreter = Interpreter.Instance;
        bool found = false;
        foreach (var it in pList) {
            var dialogue = it.Value;
            bool end = false;
            mPointer = 0;

            do {
                var res = interpreter.Execute(GetHashCode(), ref mPointer, ref dialogue.commands, ref mIL.strings, ref mIL.symbols, pAuto);

                switch (res.type) {
                    case ExecutedResultType.SUCCESS:
                        found = end = true;
                        break;
                    case ExecutedResultType.CALL:
                    case ExecutedResultType.GOTO:
                    case ExecutedResultType.FAILED:
                        mPointer = -1;
                        Debug.LogError("Runtime Error!");
                        end = true;
                        break;
                    case ExecutedResultType.NOT_APPLIED:
                        mPointer = -1;
                        end = true;
                        break;
                    case ExecutedResultType.JUMP:
                        mPointer = res.code;
                        break;
                    case ExecutedResultType.REQUIRE_NEXT:
                        break;
                    case ExecutedResultType.END:
                        end = true;
                        break;
                }
            } while (!end);

            if (found) {
                mCurrentDialogue = dialogue;
                StartCoroutine(Process(pAuto));
                break;
            }
        }

        return found;
    }

    private void Update() {
        if (mPointer > -1) {
            return;
        }

        mInteracting = Mathf.Abs(Input.GetAxis(interactKey)) > 0.01f;
        if (mInteracting) {
            Input.ResetInputAxes();
        }

        bool _ = (mInteracting && TryToStart(ref mIL.interactDialogues)) ||
            (mTriggered && TryToStart(ref mIL.triggerDialogues)) ||
            TryToStart(ref mIL.autoDialogues, true);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        mTriggered = true;
    }

    private void OnTriggerExit2D(Collider2D collision) {
        mTriggered = false;
    }

    private IEnumerator Process(bool pAuto) {
        var interpreter = Interpreter.Instance;
        while (true) {
            bool end = false;

            do {
                var res = interpreter.Execute(GetHashCode(), ref mPointer, ref mCurrentDialogue.commands, ref mIL.strings, ref mIL.symbols, pAuto);

                switch (res.type) {
                    case ExecutedResultType.SUCCESS:
                        end = true;
                        break;
                    case ExecutedResultType.CALL: {
                            StackValue stackValue = new StackValue();
                            stackValue.pointer = mPointer + 1;
                            stackValue.data = mCurrentDialogue;
                            mStack.Push(stackValue);
                            mPointer = 0;
                            mCurrentDialogue = mIL.defaultDialogues[res.code];
                            break;
                        }
                    case ExecutedResultType.GOTO: {
                            mPointer = 0;
                            mCurrentDialogue = mIL.defaultDialogues[res.code];
                            break;
                        }
                    case ExecutedResultType.FAILED:
                        mPointer = -1;
                        Debug.LogError("Runtime Error!");
                        end = true;
                        break;
                    case ExecutedResultType.NOT_APPLIED:
                        end = true;
                        break;
                    case ExecutedResultType.JUMP:
                        mPointer = res.code;
                        break;
                    case ExecutedResultType.REQUIRE_NEXT:
                        break;
                    case ExecutedResultType.END: {
                            if (mStack.Count > 0) {
                                var top = mStack.Pop();
                                mPointer = top.pointer;
                                mCurrentDialogue = top.data;
                            }
                            else {
                                mPointer = -1;
                                end = true;
                            }

                            break;
                        }

                }
            } while (!end);

            if (mPointer > -1) {
                yield return null;
            }
            else {
                break;
            }
        }
    }

    public override int GetHashCode() {
        return script.GetHashCode();
    }
}
