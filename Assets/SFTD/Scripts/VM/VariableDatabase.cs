using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum VariableType {
    STRING = 1, INT = 2, FLOAT = 3, BOOL = 4
}

[System.Serializable]
public struct VariableData {
    public string name;
    public VariableType type;
    public string s;
    public int i;
    public float f;
    public bool b;
}

public class VariableDatabase : MonoBehaviour {
    [SerializeField] private VariableData[] variables;

    private Dictionary<string, int> mNameMapping = new Dictionary<string, int>();

    private static VariableDatabase instance;

    private Dictionary<int, VariableData> mTempVariables = new Dictionary<int, VariableData>();

    private void Awake() {
        instance = this;
        if (variables == null) {
            return;
        }

        for (int i = 0; i < variables.Length; ++i) {
            var v = variables[i];
            mNameMapping.Add(v.name, i);
        }
    }

    public static VariableDatabase Instance {
        get { return instance; }
    }

    public void Set(string pName, VariableData value) {
        switch (value.type) { 
            case VariableType.STRING:
                Set(pName, value.s);
                break;
            case VariableType.INT:
                Set(pName, value.i);
                break;
            case VariableType.FLOAT:
                Set(pName, value.f);
                break;
            case VariableType.BOOL:
                Set(pName, value.b);
                break;
        }
    } 

    public void Set(string pName, int value) {
        if (mNameMapping.ContainsKey(pName)) {
            int i = mNameMapping[pName];
            variables[i].type = VariableType.INT;
            variables[i].i = value;
        }
        else {
            Debug.LogWarningFormat("Variable {0} doesn't exist.", pName);
        }
    }

    public void Set(string pName, float value) {
        if (mNameMapping.ContainsKey(pName)) {
            int i = mNameMapping[pName];
            variables[i].type = VariableType.FLOAT;
            variables[i].f = value;
        }
        else {
            Debug.LogWarningFormat("Variable {0} doesn't exist.", pName);
        }
    }

    public void Set(string pName, bool value) {
        if (mNameMapping.ContainsKey(pName)) {
            int i = mNameMapping[pName];
            variables[i].type = VariableType.BOOL;
            variables[i].b = value;
        }
        else {
            Debug.LogWarningFormat("Variable {0} doesn't exist.", pName);
        }
    }

    public void Set(string pName, string value) {
        if (mNameMapping.ContainsKey(pName)) {
            int i = mNameMapping[pName];
            variables[i].type = VariableType.STRING;
            variables[i].s = value;
        }
        else {
            Debug.LogWarningFormat("Variable {0} doesn't exist.", pName);
        }
    }

    public int GetInt(string pName) {
        if (mNameMapping.ContainsKey(pName)) {
            int i = mNameMapping[pName];
            if (variables[i].type != VariableType.INT) {
                Debug.LogWarningFormat("Variable {0} is not int.", pName);
            }

            return variables[i].i;
        }
        else {
            Debug.LogWarningFormat("Variable {0} doesn't exist.", pName);
            return 0;
        }
    }

    public float GetFloat(string pName) {
        if (mNameMapping.ContainsKey(pName)) {
            int i = mNameMapping[pName];
            if (variables[i].type != VariableType.INT) {
                return variables[i].i;
            }
            if (variables[i].type != VariableType.FLOAT) {
                Debug.LogWarningFormat("Variable {0} is not float.", pName);
            }

            return variables[i].f;
        }
        else {
            Debug.LogWarningFormat("Variable {0} doesn't exist.", pName);
            return 0.0f;
        }
    }

    public bool GetBool(string pName) {
        if (mNameMapping.ContainsKey(pName)) {
            int i = mNameMapping[pName];
            if (variables[i].type != VariableType.BOOL) {
                Debug.LogWarningFormat("Variable {0} is not bool.", pName);
            }

            return variables[i].b;
        }
        else {
            Debug.LogWarningFormat("Variable {0} doesn't exist.", pName);
            return false;
        }
    }

    public string GetString(string pName) {
        if (mNameMapping.ContainsKey(pName)) {
            int i = mNameMapping[pName];
            if (variables[i].type != VariableType.STRING) {
                Debug.LogWarningFormat("Variable {0} is not string.", pName);
            }

            return variables[i].s;
        }
        else {
            Debug.LogWarningFormat("Variable {0} doesn't exist.", pName);
            return null;
        }
    }

    public string GetAny(string pName) {
        if (mNameMapping.ContainsKey(pName)) {
            int i = mNameMapping[pName];
            string res = null;
            switch (variables[i].type) {
                case VariableType.BOOL:
                    res = variables[i].b.ToString();
                    break;
                case VariableType.STRING:
                    res = variables[i].s;
                    break;
                case VariableType.INT:
                    res = variables[i].i.ToString();
                    break;
                case VariableType.FLOAT:
                    res = variables[i].f.ToString();
                    break;
            }

            return res;
        }
        else {
            Debug.LogWarningFormat("Variable {0} doesn't exist.", pName);
            return null;
        }
    }

    public VariableData GetNamedVariable(string pName) {
        if (mNameMapping.ContainsKey(pName)) {
            return variables[mNameMapping[pName]];
        }

        throw new Exception("Unknown variable.");
    }

    public VariableData GetTempVariable(int index) {
        if (mTempVariables.ContainsKey(index)) { 
            return mTempVariables[index];
        }

        throw new Exception("Unknown temp variable.");
    }

    public void SetTempVariable(int index, VariableData data) {
        mTempVariables[index] = data;
    }
    public int GetInt(int pIndex) {
        if (mTempVariables.ContainsKey(pIndex)) {
            if (mTempVariables[pIndex].type != VariableType.INT) {
                Debug.LogWarningFormat("Variable {0} is not int.", pIndex);
            }

            return mTempVariables[pIndex].i;
        }
        else {
            Debug.LogWarningFormat("Variable {0} doesn't exist.", pIndex);
            return 0;
        }
    }

    public float GetFloat(int pIndex) {
        if (mTempVariables.ContainsKey(pIndex)) {
            if (mTempVariables[pIndex].type != VariableType.INT) {
                return mTempVariables[pIndex].i;
            }
            if (mTempVariables[pIndex].type != VariableType.FLOAT) {
                Debug.LogWarningFormat("Variable {0} is not float.", pIndex);
            }

            return mTempVariables[pIndex].f;
        }
        else {
            Debug.LogWarningFormat("Variable {0} doesn't exist.", pIndex);
            return 0.0f;
        }
    }

    public bool GetBool(int pIndex) {
        if (mTempVariables.ContainsKey(pIndex)) {
            if (mTempVariables[pIndex].type != VariableType.BOOL) {
                Debug.LogWarningFormat("Variable {0} is not bool.", pIndex);
            }

            return mTempVariables[pIndex].b;
        }
        else {
            Debug.LogWarningFormat("Variable {0} doesn't exist.", pIndex);
            return false;
        }
    }

    public string GetString(int pIndex) {
        if (mTempVariables.ContainsKey(pIndex)) {
            if (mTempVariables[pIndex].type != VariableType.STRING) {
                Debug.LogWarningFormat("Variable {0} is not string.", pIndex);
            }

            return mTempVariables[pIndex].s;
        }
        else {
            Debug.LogWarningFormat("Variable {0} doesn't exist.", pIndex);
            return null;
        }
    }
}
