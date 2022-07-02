using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour {
    protected string mName;
    protected Vector2 mVelocity = Vector2.zero;
    protected float mTimer = 0.0f;
    [SerializeField] protected Animator mAnimator;

    public string Name { 
        get { return mName; }
        set { mName = value; }
    }

    public virtual void PlayAnimation(string pName) {
        if (mAnimator == null) {
            Debug.LogErrorFormat("{0} does not have an animator component.");
            return;
        }
        mAnimator.Play(pName);
    }

    public virtual void MoveBy(Vector2 pDis, float pTime) {
        mTimer = pTime;
        mVelocity = pDis / pTime;
    }

    private void Update() {
        if (mTimer > 0.0f) {
            float m = Mathf.Min(Time.deltaTime, mTimer);
            transform.position = transform.position + new Vector3(mVelocity.x, mVelocity.y, 0) * m;
            mTimer -= m;

            if (mTimer < 0.0f) { 
                mTimer = 0.0f;
            } 
        }
    }

    public virtual bool HasAnimationEnded() {
        AnimatorStateInfo info = mAnimator.GetCurrentAnimatorStateInfo(0);
        return info.normalizedTime > 0.99f;
    }

    public virtual bool HasMovementEnded() {
        return !(mTimer > 0);
    }
}
