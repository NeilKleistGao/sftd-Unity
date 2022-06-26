using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "avatar", menuName = "SFTD/Avatar Info")]
public class AvatarInfo : ScriptableObject {
    public new string name;
    public Sprite avatar;
    public Font font;
    public AudioClip sound;
}
