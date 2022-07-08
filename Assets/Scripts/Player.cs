using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    private readonly float mVelocity = 5.0f;

    private void Update() {
        float dx = Time.deltaTime * mVelocity;
        if (Mathf.Abs(Input.GetAxis("MoveUp")) >= 0.1f) {
            transform.position = new Vector3(transform.position.x, transform.position.y + dx, transform.position.z);
        }
        else if (Mathf.Abs(Input.GetAxis("MoveDown")) >= 0.1f) {
            transform.position = new Vector3(transform.position.x, transform.position.y - dx, transform.position.z);
        }
        else if (Mathf.Abs(Input.GetAxis("MoveLeft")) >= 0.1f) {
            transform.position = new Vector3(transform.position.x - dx, transform.position.y, transform.position.z);
        }
        else if (Mathf.Abs(Input.GetAxis("MoveRight")) >= 0.1f) {
            transform.position = new Vector3(transform.position.x + dx, transform.position.y, transform.position.z);
        }
    }
}
