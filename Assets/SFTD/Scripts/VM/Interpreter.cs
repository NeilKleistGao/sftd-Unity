using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public struct DialogueData {
    public byte[][] commands;
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
    JUMP, // jump to another one
    REQUIRE_NEXT, // need next command now
    END
}

public struct ExecutedResult {
    public ExecutedResultType type;
    public int code;
}

public class Interpreter : MonoBehaviour {
    private static Interpreter sInstance;

    private Dictionary<int, int> mCommandSize = new Dictionary<int, int>();
    private Dictionary<int, bool> mBuzy = new Dictionary<int, bool>();

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

    private DialogueData ParseDialogue(ref byte[] pContent, ref int pPointer) {
        DialogueData data = new DialogueData();
        int count = ReadInt(ref pContent, ref pPointer);
        data.commands = new byte[count][];

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
            data.commands[i] = new byte[size];
            for (int j = 0; j < size; ++j) {
                data.commands[i][j] = pContent[pPointer];
                ++pPointer;
            }
        }

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

    public ExecutedResult Execute(ref byte[] pProgram) {
        ExecutedResult result = new ExecutedResult();
        int pointer = 0;
        int op = ReadInt(ref pProgram, ref pointer);

        try {
            switch (op) {
                case 0:
                    result.type = ExecutedResultType.REQUIRE_NEXT;
                    break;
                case 1:
                    result.type = ExecutedResultType.REQUIRE_NEXT;
                    break;
                case 2:
                case 3:
                    result.type = ExecutedResultType.REQUIRE_NEXT;
                    break;
                case 4:
                case 5:
                    result.type = ExecutedResultType.SUCCESS;
                    break;
                case 6:
                    result.type = ExecutedResultType.CALL;
                    result.code = ReadInt(ref pProgram, ref pointer);
                    break;
                case 7:
                    result.type = ExecutedResultType.JUMP;
                    result.code = ReadInt(ref pProgram, ref pointer);
                    break;
                case 8:
                    result.type = ExecutedResultType.REQUIRE_NEXT;
                    break;
                case 9:
                    result.type = ExecutedResultType.REQUIRE_NEXT;
                    break;
                case 10:
                    result.type = ExecutedResultType.REQUIRE_NEXT;
                    break;
                case 11:
                    result.type = ExecutedResultType.SUCCESS;
                    break;
                case 12:
                    //TODO:
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
                    result.type = ExecutedResultType.REQUIRE_NEXT;
                    break;
                case 27:
                    result.type = ExecutedResultType.SUCCESS;
                    break;
                case 28:
                    result.type = ExecutedResultType.REQUIRE_NEXT;
                    break;
                case 29:
                    result.type = ExecutedResultType.REQUIRE_NEXT;
                    break;
                case 255:
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

        return result;
    }

    private void OnSelecting(int pIndex) {
    }

    private void Start() {
        DialogueController.Instance.OnSelecting.AddListener(OnSelecting);
    }
}
