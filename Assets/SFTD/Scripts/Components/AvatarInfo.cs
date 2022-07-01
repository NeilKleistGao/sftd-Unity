using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "avatar", menuName = "SFTD/Avatar Info")]
public class AvatarInfo : ScriptableObject {
    public new string name;
    public Sprite avatar;
    public TMP_FontAsset font;
    public AudioClip sound;
}
