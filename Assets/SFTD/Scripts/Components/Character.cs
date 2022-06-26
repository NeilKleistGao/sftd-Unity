using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "chara", menuName = "SFTD/Character")]
public class Character : ScriptableObject {
    public new string name;
    public AvatarInfo defaultInfo;
    public AvatarInfo[] states;
}
