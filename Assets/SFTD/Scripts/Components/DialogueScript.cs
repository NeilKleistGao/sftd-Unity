using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class DialogueScript : MonoBehaviour {
    [SerializeField] private TextAsset script;
    [SerializeField] private string interactKey = "Submit";

    private Stack<int> mStack = new Stack<int>();

    private DialogueILPack mIL;
    private DialogueData mCurrentDialogue;
    private int mPointer = -1;
    private bool mAuto = false;
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
    }

    private bool TryToStart(ref Dictionary<int, DialogueData> pList, bool mAuto = false) {
        var interpreter = Interpreter.Instance;
        bool found = false;
        foreach (var it in pList) {
            var dialogue = it.Value;
            bool end = false;
            mPointer = 0;

            do {
                var res = interpreter.Execute(ref dialogue.commands[mPointer], !mAuto);

                switch (res.type) {
                    case ExecutedResultType.SUCCESS:
                        ++mPointer;
                        found = end = true;
                        break;
                    case ExecutedResultType.CALL:
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
                        mPointer += res.code;
                        break;
                    case ExecutedResultType.REQUIRE_NEXT:
                        ++mPointer;
                        break;
                    case ExecutedResultType.END:
                        end = true;
                        break;
                }
            } while (!end);

            if (found) {
                this.mAuto = mAuto;
                StartCoroutine("Process");
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

    private void Process() { 

    }
}
