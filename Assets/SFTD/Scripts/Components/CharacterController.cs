using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour {
    private string mName;
    [SerializeField] private Animator mAnimator;

    public string Name { 
        get { return mName; }
        set { mName = value; }
    }

    public virtual void PlayAnimation(string name) { 
        mAnimator.Play(name);
    }
}
