﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField]
    private float flyForceMultiplier;
    [SerializeField]
    private float flapForceMultiplier;
    [SerializeField]
    private float windForceMultiplier;
    [SerializeField]
    private float torqueMultiplier;
    [SerializeField]
    private float speedDecay;
    [SerializeField]
    private float maxSpeed;
    [SerializeField]
    private Animator animator;

    private Rigidbody rb;
    private Vector2 movementInput;
    private float speed;
    private bool flapping = false;
    private int maxFlaps = 1;
    private int flaps = 1;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        speed = 25f;
    }

    // Update is called once per frame
    void Update() {
        // Get movement inputs
        movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        // Flap wings input
        if (Input.GetButtonDown("Jump")) {
            flapping = true;
        }
    }

    void FixedUpdate() {
        // Rotate player
        if (movementInput.y != 0f) {
            Quaternion rotation = rb.rotation;
            Vector3 eulerRotation = rotation.eulerAngles;
            eulerRotation.x += torqueMultiplier * movementInput.y;
            rotation.eulerAngles = eulerRotation;
            rb.rotation = rotation;
        }

        if (movementInput.x != 0f) {
            Quaternion rotation = rb.rotation;
            Vector3 eulerRotation = rotation.eulerAngles;
            eulerRotation.y += torqueMultiplier * movementInput.x;
            rotation.eulerAngles = eulerRotation;
            rb.rotation = rotation;
        }

        // Gliding physics
        Debug.Log(speed);
        speed += Mathf.Clamp(((transform.rotation.eulerAngles.x > 90f ? 0f : transform.rotation.eulerAngles.x + 1f) / 90f), 0f, 1f) * flyForceMultiplier;
        rb.AddForce(transform.forward * speed);
        speed -= speedDecay * (transform.rotation.eulerAngles.x > 90f ? Mathf.Clamp(360f - transform.rotation.eulerAngles.x, 1f, 90f) : 0f);
        speed = Mathf.Clamp(speed, 0f, maxSpeed);
        if (speed < 5f) {
            rb.AddForce(transform.up * -2f * flyForceMultiplier);
        }

        // Flap wings physics
        if (flapping) {
            if (flaps > 0) {
                animator.SetTrigger("FlyFlap");
                rb.AddForce(Vector3.up * flapForceMultiplier);
                rb.AddForce(transform.forward * flapForceMultiplier / 4);
                flaps--;
            }
            flapping = false;
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Fries") {
            flaps = maxFlaps;
            Destroy(other.gameObject);
        }
    }

    void OnTriggerStay(Collider other) {
        if (other.tag == "Wind") {
            rb.AddForce(Vector3.up * windForceMultiplier);
        }
    }
}
