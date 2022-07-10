using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour {
    [SerializeField] private Animator animator;
    [SerializeField] private bool isUserControlling = false;

    private Vector2 mPrevious = Vector2.zero;
    private Vector2 mForward = Vector2.down / 10.0f;

    private void Start() {
        mPrevious = transform.position;
    }

    private void FixedUpdate() {
        if (isUserControlling) {
            return;
        }

        Vector2 now = transform.position;
        var d = (now - mPrevious) / Time.fixedDeltaTime;
        if (d.magnitude < 0.001f) {
            SetVelocity(mForward, false);
        }
        else { 
            SetVelocity(d.normalized, true);
        }

        mPrevious = now;
    }

    private void SetVelocity(Vector2 pV, bool pUpdateForward) {
        if (pUpdateForward) {
            mForward = pV / 10.0f;
        }
       
        animator.SetFloat("BlendX", pV.x);
        animator.SetFloat("BlendY", pV.y);
    }

    public void SetVelocity(Vector2 pV) {
        SetVelocity(pV, true);
    }
}
