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

    public void Set(string name, int value) {
        if (mNameMapping.ContainsKey(name)) {
            int i = mNameMapping[name];
            variables[i].type = VariableType.INT;
            variables[i].i = value;
        }
        else {
            Debug.LogWarningFormat("Variable {0} doesn't exist.", name);
        }
    }

    public void Set(string name, float value) {
        if (mNameMapping.ContainsKey(name)) {
            int i = mNameMapping[name];
            variables[i].type = VariableType.FLOAT;
            variables[i].f = value;
        }
        else {
            Debug.LogWarningFormat("Variable {0} doesn't exist.", name);
        }
    }

    public void Set(string name, bool value) {
        if (mNameMapping.ContainsKey(name)) {
            int i = mNameMapping[name];
            variables[i].type = VariableType.BOOL;
            variables[i].b = value;
        }
        else {
            Debug.LogWarningFormat("Variable {0} doesn't exist.", name);
        }
    }

    public void Set(string name, string value) {
        if (mNameMapping.ContainsKey(name)) {
            int i = mNameMapping[name];
            variables[i].type = VariableType.STRING;
            variables[i].s = value;
        }
        else {
            Debug.LogWarningFormat("Variable {0} doesn't exist.", name);
        }
    }

    public int GetInt(string name) {
        if (mNameMapping.ContainsKey(name)) {
            int i = mNameMapping[name];
            if (variables[i].type != VariableType.INT) {
                Debug.LogWarningFormat("Variable {0} is not int.", name);
            }

            return variables[i].i;
        }
        else {
            Debug.LogWarningFormat("Variable {0} doesn't exist.", name);
            return 0;
        }
    }

    public float GetFloat(string name) {
        if (mNameMapping.ContainsKey(name)) {
            int i = mNameMapping[name];
            if (variables[i].type != VariableType.FLOAT) {
                Debug.LogWarningFormat("Variable {0} is not float.", name);
            }

            return variables[i].f;
        }
        else {
            Debug.LogWarningFormat("Variable {0} doesn't exist.", name);
            return 0.0f;
        }
    }

    public bool GetBool(string name) {
        if (mNameMapping.ContainsKey(name)) {
            int i = mNameMapping[name];
            if (variables[i].type != VariableType.BOOL) {
                Debug.LogWarningFormat("Variable {0} is not bool.", name);
            }

            return variables[i].b;
        }
        else {
            Debug.LogWarningFormat("Variable {0} doesn't exist.", name);
            return false;
        }
    }

    public string GetString(string name) {
        if (mNameMapping.ContainsKey(name)) {
            int i = mNameMapping[name];
            if (variables[i].type != VariableType.STRING) {
                Debug.LogWarningFormat("Variable {0} is not string.", name);
            }

            return variables[i].s;
        }
        else {
            Debug.LogWarningFormat("Variable {0} doesn't exist.", name);
            return null;
        }
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
}
