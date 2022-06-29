using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDatabase : MonoBehaviour {
    [SerializeField] private Character[] characters;
    [SerializeField] private CharacterController[] controllers;

    private Dictionary<string, int> mCharacterIndex = new Dictionary<string, int>();
    private Dictionary<string, int> mControllerIndex = new Dictionary<string, int>();

    private static CharacterDatabase instance = null;

    public static CharacterDatabase Instance { 
        get { return instance; }
    }

    private void Awake() {
        instance = this;
        if (characters != null) {
            for (int i = 0; i < characters.Length; ++i) {
                mCharacterIndex.Add(characters[i].name, i);
            }
        }

        if (controllers != null) {
            for (int i = 0; i < controllers.Length; ++i) {
                mControllerIndex.Add(controllers[i].Name, i);
            }
        }
    }

    public Character GetCharacter(string name) {
        if (!mCharacterIndex.ContainsKey(name)) {
            Debug.LogErrorFormat("Character {0} not found.", name);
            return null;
        }

        return characters[mCharacterIndex[name]];
    }

    public CharacterController GetController(string name) {
        if (!mControllerIndex.ContainsKey(name)) {
            Debug.LogErrorFormat("Controller {0} not found.", name);
            return null;
        }

        return controllers[mControllerIndex[name]];
    }
}
