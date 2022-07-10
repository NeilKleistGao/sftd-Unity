using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    [SerializeField] private MovementController movementController;
    private readonly float mVelocity = 5.0f;
    private Vector2 mForward = Vector2.down / 10;

    private void FixedUpdate() {
        float dx = Time.fixedDeltaTime * mVelocity;
        if (Mathf.Abs(Input.GetAxis("MoveUp")) >= 0.1f) {
            transform.position = new Vector3(transform.position.x, transform.position.y + dx, transform.position.z);
            movementController.SetVelocity(new Vector2(0, 1)); mForward = Vector2.up / 10;
        }
        else if (Mathf.Abs(Input.GetAxis("MoveDown")) >= 0.1f) {
            transform.position = new Vector3(transform.position.x, transform.position.y - dx, transform.position.z);
            movementController.SetVelocity(new Vector2(0, -1)); mForward = Vector2.down / 10;
        }
        else if (Mathf.Abs(Input.GetAxis("MoveLeft")) >= 0.1f) {
            transform.position = new Vector3(transform.position.x - dx, transform.position.y, transform.position.z);
            movementController.SetVelocity(new Vector2(-1, 0)); mForward = Vector2.left / 10;
        }
        else if (Mathf.Abs(Input.GetAxis("MoveRight")) >= 0.1f) {
            transform.position = new Vector3(transform.position.x + dx, transform.position.y, transform.position.z);
            movementController.SetVelocity(new Vector2(1, 0)); mForward = Vector2.right / 10;
        }
        else {
            movementController.SetVelocity(mForward);
        }
    }
}
