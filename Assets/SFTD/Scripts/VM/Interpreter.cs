using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public struct DialogueData {
    public byte[] commands;
}

public struct DialogueILPack {
    public bool enableI18N;
    public string[] symbols;
    public string[] strings;

    public Dictionary<int, DialogueData> autoDialogues;
    public Dictionary<int, DialogueData> triggerDialogues;
    public Dictionary<int, DialogueData> interactDialogues;
    public Dictionary<int, DialogueData> defaultDialogues;
}

public enum ExecutedResultType { 
    SUCCESS, // success, move to next one
    FAILED, // runtime error
    NOT_APPLIED, // not available yet
    CALL, // call another one
    JUMP, // jump to another local place
    GOTO, // jump to another dialogue process
    REQUIRE_NEXT, // need next command now
    END
}

public struct ExecutedResult {
    public ExecutedResultType type;
    public int code;
}

public class Interpreter : MonoBehaviour {
    private struct OptionsData {
        public List<string> options;
        public List<int> startPosition;
    }

    private enum WaitingType { 
        NONE, ANIMATION, MOVE
    }

    private static Interpreter sInstance;

    private Dictionary<int, int> mCommandSize = new Dictionary<int, int>();
    private Dictionary<int, bool> mBusy = new Dictionary<int, bool>();
    private OptionsData mOptionsData = new OptionsData();
    private int mPreviousType = -1;
    private int mSelectedPos = -1;
    private bool mGlobalBusy = false;
    private WaitingType mWaitingType = WaitingType.NONE;
    private CharacterController mWatingController = null;
    private int mWaitingID = -1;

    private void Awake() {
        sInstance = this;

        mCommandSize.Add(0, 1);
        mCommandSize.Add(1, 1);
        mCommandSize.Add(2, 2);
        mCommandSize.Add(3, 3);
        mCommandSize.Add(4, 2);
        mCommandSize.Add(5, 4);
        mCommandSize.Add(6, 2);
        mCommandSize.Add(7, 2);
        mCommandSize.Add(8, 3);
        mCommandSize.Add(9, 4);
        mCommandSize.Add(10, 3);
        mCommandSize.Add(11, 8);
        mCommandSize.Add(12, 4);
        mCommandSize.Add(13, 6);
        mCommandSize.Add(14, 6);
        mCommandSize.Add(15, 6);
        mCommandSize.Add(16, 6);
        mCommandSize.Add(17, 6);
        mCommandSize.Add(18, 6);
        mCommandSize.Add(19, 6);
        mCommandSize.Add(20, 6);
        mCommandSize.Add(21, 4);
        mCommandSize.Add(22, 4);
        mCommandSize.Add(23, 6);
        mCommandSize.Add(24, 6);
        mCommandSize.Add(25, 6);
        mCommandSize.Add(26, 6);
        mCommandSize.Add(27, 3);
        mCommandSize.Add(28, -1);
        mCommandSize.Add(29, 4);
        mCommandSize.Add(30, 2);
        mCommandSize.Add(255, 1);
    }

    public static Interpreter Instance { 
        get { return sInstance; }
    }

    private int ReadInt(ref byte[] pContent, ref int pPointer) {
        byte[] buffer = new byte[4] { pContent[pPointer], pContent[pPointer + 1],
                                        pContent[pPointer + 2], pContent[pPointer + 3]};
        pPointer += 4;
        int i = (buffer[0] << 24) | (buffer[1] << 16) | (buffer[2] << 8) | (buffer[3]);
        return i;
    }

    private string ReadString(ref byte[] pContent, ref int pPointer) {
        int length = ReadInt(ref pContent, ref pPointer);
        byte[] buffer = new byte[length];

        for (int i = 0; i < length; i++) {
            buffer[i] = pContent[pPointer];
            ++pPointer;
        }

        return Encoding.ASCII.GetString(buffer);
    }

    private float ReadFloat(ref byte[] pContent, ref int pPointer) {
        byte[] buffer = new byte[4] { pContent[pPointer], pContent[pPointer + 1],
                                        pContent[pPointer + 2], pContent[pPointer + 3]};
        pPointer += 4;
        return BitConverter.ToSingle(buffer);
    }

    private DialogueData ParseDialogue(ref byte[] pContent, ref int pPointer) {
        DialogueData data = new DialogueData();
        int count = ReadInt(ref pContent, ref pPointer);
        var temp = new List<byte>();

        for (int i = 0; i < count; ++i) {
            int pos = pPointer;
            int op = ReadInt(ref pContent, ref pPointer);
            pPointer = pos;

            if (!mCommandSize.ContainsKey(op)) {
                Debug.LogErrorFormat("File has broken at {0}: {1}", pos, op);
            }

            int size = mCommandSize[op];
            if (size < 0) {
                pPointer += 8;
                int subSize = ReadInt(ref pContent, ref pPointer);
                pPointer = pos;
                size = subSize * 2 + 3;
            }

            size <<= 2;
            for (int j = 0; j < size; ++j) {
                temp.Add(pContent[pPointer]);
                ++pPointer;
            }
        }

        data.commands = temp.ToArray();
        return data;
    }

    public DialogueILPack LoadScript(byte[] pContent) {
        DialogueILPack pack = new DialogueILPack();
        int pointer = 0;

        byte[] magicNumber = new byte[2] { pContent[pointer], pContent[pointer + 1] };
        pointer += 2;

        if (magicNumber[0] != 0xAE || magicNumber[1] != 0x86) {
            Debug.LogError("Invalid Dialogue File!");
            return pack;
        }

        byte[] i18n = new byte[2] { pContent[pointer], pContent[pointer + 1] };
        pointer += 2;
        pack.enableI18N = (i18n[0] != '\0');

        int symbolCount = ReadInt(ref pContent, ref pointer);
        pack.symbols = new string[symbolCount];
        for (int i = 0; i < symbolCount; ++i) {
            ReadInt(ref pContent, ref pointer);
            pack.symbols[i] = ReadString(ref pContent, ref pointer);
        }

        int stringCount = ReadInt(ref pContent, ref pointer);
        pack.strings = new string[stringCount];
        for (int i = 0; i < stringCount; ++i) {
            ReadInt(ref pContent, ref pointer);
            pack.strings[i] = ReadString(ref pContent, ref pointer);
        }

        pointer += (pointer % 4);

        int autoCount = ReadInt(ref pContent, ref pointer);
        pack.autoDialogues = new Dictionary<int, DialogueData>();
        for (int i = 0; i < autoCount; ++i) {
            int pos = pointer;
            pack.autoDialogues.Add(pos, ParseDialogue(ref pContent, ref pointer));
        }

        int triggerCount = ReadInt(ref pContent, ref pointer);
        pack.triggerDialogues = new Dictionary<int, DialogueData>();
        for (int i = 0; i < triggerCount; ++i) {
            int pos = pointer;
            pack.triggerDialogues.Add(pos, ParseDialogue(ref pContent, ref pointer));
        }

        int interactCount = ReadInt(ref pContent, ref pointer);
        pack.interactDialogues = new Dictionary<int, DialogueData>();
        for (int i = 0; i < interactCount; ++i) {
            int pos = pointer;
            pack.interactDialogues.Add(pos, ParseDialogue(ref pContent, ref pointer));
        }

        int defaultCount = ReadInt(ref pContent, ref pointer);
        pack.defaultDialogues = new Dictionary<int, DialogueData>();
        for (int i = 0; i < defaultCount; ++i) {
            int pos = pointer;
            pack.defaultDialogues.Add(pos, ParseDialogue(ref pContent, ref pointer));
        }

        return pack;
    }

    private CharacterController GetController(ref byte[] pProgram, ref int pPointer, ref string[] pSymbols) {
        int targetID = ReadInt(ref pProgram, ref pPointer);
        if (targetID >= pSymbols.Length) {
            Debug.LogError("Character not found.");
            return null;
        }

        string target = pSymbols[targetID];
        var cc = CharacterDatabase.Instance.GetController(target);
        if (cc == null) {
            Debug.LogErrorFormat("Character {0} not found.", target);
        }

        return cc;
    }

    private string GetString(ref byte[] pProgram, ref int pPointer, ref string[] pStrings, ref string[] pSymbols) {
        int type = ReadInt(ref pProgram, ref pPointer);
        string str = "";
        if (type == 0) {
            int id = ReadInt(ref pProgram, ref pPointer);
            if (id < 0) {
                str = VariableDatabase.Instance.GetString(id);
            }
            else {
                string name = pSymbols[id];
                str = VariableDatabase.Instance.GetString(name);
            }
        }
        else if (type == 1) {
            str = pStrings[ReadInt(ref pProgram, ref pPointer)];
        }
        else {
            Debug.LogError("GetString Runtime Error.");
        }

        return str;
    }

    private float GetFloat(ref byte[] pProgram, ref int pPointer, ref string[] pStrings, ref string[] pSymbols) {
        int type = ReadInt(ref pProgram, ref pPointer);
        float f = 0.0f;
        if (type == 0) {
            int pos = ReadInt(ref pProgram, ref pPointer);
            if (pos < 0) {
                f = VariableDatabase.Instance.GetFloat(pos);
            }
            else {
                f = VariableDatabase.Instance.GetFloat(pSymbols[pos]);
            }
        }
        else if (type == 2) {
            f = ReadInt(ref pProgram, ref pPointer);
        }
        else if (type == 3) {
            f = ReadFloat(ref pProgram, ref pPointer);
        }
        else {
            Debug.LogError("GetFloat Runtime Error.");
        }

        return f;
    }

    private bool GetBool(ref byte[] pProgram, ref int pPointer, ref string[] pStrings, ref string[] pSymbols) {
        int type = ReadInt(ref pProgram, ref pPointer);
        bool b = false;
        if (type == 0) {
            int pos = ReadInt(ref pProgram, ref pPointer);
            if (pos < 0) {
                b = VariableDatabase.Instance.GetBool(pos);
            }
            else {
                b = VariableDatabase.Instance.GetBool(pSymbols[pos]);
            }
        }
        else if (type == 4) {
            b = ReadInt(ref pProgram, ref pPointer) != 0;
        }
        else {
            Debug.LogError("GetBool Runtime Error.");
        }

        return b;
    }

    private VariableData GetAny(ref byte[] pProgram, ref int pPointer, ref string[] pStrings, ref string[] pSymbols) {
        int type = ReadInt(ref pProgram, ref pPointer);
        VariableData data = new VariableData();

        if (type == 0) {
            int id = ReadInt(ref pProgram, ref pPointer);
            if (id > 0) {
                string name = pSymbols[id];
                data = VariableDatabase.Instance.GetNamedVariable(name);
            }
            else {
                data = VariableDatabase.Instance.GetTempVariable(id);
            }
        }
        else if (type == 1) {
            data.type = VariableType.STRING;
            int id = ReadInt(ref pProgram, ref pPointer);
            data.s = pStrings[id];
        }
        else if (type == 2) {
            data.type = VariableType.INT;
            data.i = ReadInt(ref pProgram, ref pPointer);
        }
        else if (type == 3) {
            data.type = VariableType.FLOAT;
            data.f = ReadFloat(ref pProgram, ref pPointer);
        }
        else if (type == 4) {
            data.type = VariableType.BOOL;
            data.b = ReadInt(ref pProgram, ref pPointer) != 0;
        }
        else {
            Debug.LogError("GetVariable Runtime Error.");
        }

        return data;
    }

    private void ExecuteSpeak(int op, ref byte[] pProgram, ref int pPointer, ref string[] pStrings, ref string[] pSymbols) {
        int id = ReadInt(ref pProgram, ref pPointer);
        string str = StringDatabase.Instance.GetString(pStrings[id]);
        if (op == 4) {
            DialogueController.Instance.ShowText(str);
        }
        else {
            float time = GetFloat(ref pProgram, ref pPointer, ref pStrings, ref pStrings);
            DialogueController.Instance.ShowText(str, time);
        }
    }

    private void ExecuteAnimation(ref byte[] pProgram, ref int pPointer, ref string[] pStrings, ref string[] pSymbols) {
        var cc = GetController(ref pProgram, ref pPointer, ref pSymbols);
        if (cc == null) {
            return;
        }

        mWaitingType = WaitingType.ANIMATION;
        mWatingController = cc;
        string anime = GetString(ref pProgram, ref pPointer, ref pStrings, ref pStrings);
        cc.PlayAnimation(anime);
    }

    private void ExecuteSound(ref byte[] pProgram, ref int pPointer, ref string[] pStrings, ref string[] pSymbols) {
        string sound = GetString(ref pProgram, ref pPointer, ref pStrings, ref pStrings);
        DialogueController.Instance.PlaySoundEffect(sound);
    }

    private void ExecuteMove(ref byte[] pProgram, ref int pPointer, ref string[] pStrings, ref string[] pSymbols) {
        var cc = GetController(ref pProgram, ref pPointer, ref pSymbols);
        if (cc == null) {
            return;
        }

        mWaitingType = WaitingType.MOVE;
        mWatingController = cc;
        Vector2 dis = new Vector2(GetFloat(ref pProgram, ref pPointer, ref pStrings, ref pSymbols),
            GetFloat(ref pProgram, ref pPointer, ref pStrings, ref pSymbols));
        float time = GetFloat(ref pProgram, ref pPointer, ref pStrings, ref pSymbols);
        cc.MoveBy(dis, time);
    }

    private void Calculate(int op, ref byte[] pProgram, ref int pPointer, ref string[] pStrings, ref string[] pSymbols) {
        int target = ReadInt(ref pProgram, ref pPointer);
        VariableData res;
        if (op == 21 || op == 22) {
            Calculator.UnaryOperator f = (op == 21) ? Calculator.CalculateNot : Calculator.CalculateNeg;
            var v = GetAny(ref pProgram, ref pPointer, ref pStrings, ref pSymbols);
            res = f(v);
        }
        else {
            Calculator.BinaryOperator f = Calculator.CalculateAdd;
            switch (op) {
                case 14:
                    f = Calculator.CalculateSub;
                    break;
                case 15:
                    f = Calculator.CalculateMul;
                    break;
                case 16:
                    f = Calculator.CalculateDiv;
                    break;
                case 17:
                    f = Calculator.CalculateMod;
                    break;
                case 18:
                    f = Calculator.CalculateEqual;
                    break;
                case 19:
                    f = Calculator.CalculateAnd;
                    break;
                case 20:
                    f = Calculator.CalculateOr;
                    break;
                case 23:
                    f = Calculator.CalculateLess;
                    break;
                case 24:
                    f = Calculator.CalculateGreater;
                    break;
                case 25:
                    f = Calculator.CalculateLE;
                    break;
                case 26:
                    f = Calculator.CalculateGE;
                    break;
            }

            var lhs = GetAny(ref pProgram, ref pPointer, ref pStrings, ref pSymbols);
            var rhs = GetAny(ref pProgram, ref pPointer, ref pStrings, ref pSymbols);
            res = f(lhs, rhs);
        }

        if (target >= 0) {
            string name = pSymbols[target];
            VariableDatabase.Instance.Set(name, res);
        }
        else {
            VariableDatabase.Instance.SetTempVariable(target, res);
        }
    }

    private IEnumerator DelayDialogue(int pID, float pTime) {
        yield return new WaitForSeconds(pTime);
        mBusy[pID] = false;
    }

    private void ExecutePublish(ref byte[] pProgram, ref int pPointer, ref string[] pStrings, ref string[] pSymbols) { 
        int id = ReadInt(ref pProgram, ref pPointer);
        string name = pSymbols[id];

        List<VariableData> variables = new List<VariableData>();
        int count = ReadInt(ref pProgram, ref pPointer);
        for (int i = 0; i < count; ++i) { 
            variables.Add(GetAny(ref pProgram, ref pPointer, ref pStrings, ref pSymbols));
        }

        EventsDatabase.Instance.Publish(name, variables);
    }

    private void ExecuteSet(ref byte[] pProgram, ref int pPointer, ref string[] pStrings, ref string[] pSymbols) {
        int target = ReadInt(ref pProgram, ref pPointer);
        var res = GetAny(ref pProgram, ref pPointer, ref pStrings, ref pSymbols);
        if (target >= 0) {
            string name = pSymbols[target];
            VariableDatabase.Instance.Set(name, res);
        }
        else {
            VariableDatabase.Instance.SetTempVariable(target, res);
        }
    }

    public ExecutedResult Execute(int pID, ref int pPointer, ref byte[] pProgram, ref string[] pStrings, ref string[] pSymbols, bool pAuto) {
        ExecutedResult result = new ExecutedResult();
        int op = ReadInt(ref pProgram, ref pPointer);

        if (!mBusy.ContainsKey(pID)) {
            mBusy[pID] = false;
        }

        if (mBusy[pID] || (mGlobalBusy && !pAuto)) {
            result.type = ExecutedResultType.NOT_APPLIED;
            return result;
        }

        if (op != 8 && mPreviousType == 8) {
            if (mSelectedPos == -1) {
                DialogueController.Instance.ShowOptions(mOptionsData.options.ToArray());
                result.type = ExecutedResultType.NOT_APPLIED;
                mBusy[pID] = true;
            }
            else {
                mBusy[pID] = false;
                result.type = ExecutedResultType.JUMP;
                result.code = mSelectedPos;
                mSelectedPos = -1;
            }

            return result;
        }

        try {
            switch (op) {
                case 0:
                    result.type = ExecutedResultType.REQUIRE_NEXT;
                    break;
                case 1:
                    DialogueController.Instance.StartDialogue();
                    mGlobalBusy = true;
                    result.type = ExecutedResultType.REQUIRE_NEXT;
                    break;
                case 2:
                case 3: {
                        string name = ReadString(ref pProgram, ref pPointer);
                        if (op == 2) {
                            DialogueController.Instance.SetAvatar(name);
                        }
                        else {
                            string state = ReadString(ref pProgram, ref pPointer);
                            DialogueController.Instance.SetAvatar(name, state);
                        }
                        
                        result.type = ExecutedResultType.REQUIRE_NEXT;
                        break;
                    }
                case 4:
                case 5: {
                        ExecuteSpeak(op, ref pProgram, ref pPointer, ref pStrings, ref pSymbols);
                        result.type = ExecutedResultType.SUCCESS;
                        break;
                    }
                case 6:
                    result.type = ExecutedResultType.CALL;
                    result.code = ReadInt(ref pProgram, ref pPointer);
                    break;
                case 7:
                    result.type = ExecutedResultType.GOTO;
                    result.code = ReadInt(ref pProgram, ref pPointer);
                    break;
                case 8:
                    result.type = ExecutedResultType.JUMP;
                    string s = pStrings[ReadInt(ref pProgram, ref pPointer)];
                    result.code = ReadInt(ref pProgram, ref pPointer);
                    mOptionsData.options.Add(s);
                    mOptionsData.startPosition.Add(pPointer);
                    break;
                case 9:
                    DialogueController.Instance.EndDialogue();
                    ExecuteAnimation(ref pProgram, ref pPointer, ref pStrings, ref pSymbols);
                    result.type = ExecutedResultType.SUCCESS;
                    mBusy[pID] = true; mWaitingID = pID;
                    break;
                case 10:
                    ExecuteSound(ref pProgram, ref pPointer, ref pStrings, ref pSymbols);
                    result.type = ExecutedResultType.REQUIRE_NEXT;
                    break;
                case 11:
                    DialogueController.Instance.EndDialogue();
                    ExecuteMove(ref pProgram, ref pPointer, ref pStrings, ref pSymbols);
                    result.type = ExecutedResultType.SUCCESS;
                    break;
                case 12:
                    bool test = GetBool(ref pProgram, ref pPointer, ref pStrings, ref pSymbols);
                    result.type = (test) ? ExecutedResultType.REQUIRE_NEXT : ExecutedResultType.JUMP;
                    result.code = ReadInt(ref pProgram, ref pPointer);
                    break;
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                case 19:
                case 20:
                case 21:
                case 22:
                case 23:
                case 24:
                case 25:
                case 26:
                    Calculate(op, ref pProgram, ref pPointer, ref pStrings, ref pSymbols);
                    result.type = ExecutedResultType.REQUIRE_NEXT;
                    break;
                case 27:
                    mBusy[pID] = true; mWaitingID = pID;
                    float time = GetFloat(ref pProgram, ref pPointer, ref pStrings, ref pSymbols);
                    StartCoroutine(DelayDialogue(pID, time));
                    result.type = ExecutedResultType.SUCCESS;
                    break;
                case 28:
                    ExecutePublish(ref pProgram, ref pPointer, ref pStrings, ref pSymbols);
                    result.type = ExecutedResultType.REQUIRE_NEXT;
                    break;
                case 29:
                    ExecuteSet(ref pProgram, ref pPointer, ref pStrings, ref pSymbols);
                    result.type = ExecutedResultType.REQUIRE_NEXT;
                    break;
                case 30:
                    result.type = ExecutedResultType.JUMP;
                    result.code = ReadInt(ref pProgram, ref pPointer);
                    break;
                case 255:
                    mGlobalBusy = false;
                    DialogueController.Instance.EndDialogue();
                    result.type = ExecutedResultType.END;
                    break;
                default:
                    result.type = ExecutedResultType.FAILED;
                    break;
            }
        }
        catch (Exception e) {
            Debug.LogError(e.Message);
            result.type = ExecutedResultType.FAILED;
        }

        mPreviousType = op;
        return result;
    }

    private void OnSelecting(int pIndex) {
        mSelectedPos = mOptionsData.startPosition[pIndex];
    }

    private void Start() {
        DialogueController.Instance.OnSelecting.AddListener(OnSelecting);
    }

    private void UndoBusy() {
        mWatingController = null;
        mWaitingType = WaitingType.NONE;
        mBusy[mWaitingID] = false;
        DialogueController.Instance.StartDialogue();
    }

    private void Update() {
        if (mWaitingType != WaitingType.NONE) {
            if (mWaitingType == WaitingType.ANIMATION) {
                if (mWatingController.HasAnimationEnded()) {
                    UndoBusy();
                }
            }
            else {
                if (mWatingController.HasMovementEnded()) {
                    UndoBusy();
                }
            }
        }
    }
}
