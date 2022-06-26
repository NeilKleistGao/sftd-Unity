using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour {
    private string mName;

    public string Name { 
        get { return mName; }
        set { mName = value; }
    }
}
