﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    Transform cam;
    PlayerController playerController; // should move to delegate at some point
    Rigidbody rigidBody;

    Vector3 internalRotation = new Vector3(); // we have to keep track of it ourselves because of the unpredictable way transform.localEulerAngles returns
    public float movementSpeed = 10f;
    public float superSpeed = 1f;
    public Vector3 jumpForce = new Vector3(0f, 375f, 0f);
    public Vector3 jetpackForce = new Vector3(0f, 5f, 0f);
    float rotateSpeed = 5f;

    bool grounded = true;

    void Start()
    {
        cam = transform.GetChild(0);
        playerController = GetComponent<PlayerController>();
        rigidBody = GetComponent<Rigidbody>();
    }

    

    void Update()
    {

        if(playerController.paused || TimeKeeper.selectingActiveRounds) { return; }

        if (Input.GetKey(KeyCode.LeftShift)) { superSpeed = 3f; }
        else { superSpeed = 1f; }


        if (Input.GetAxis("Mouse X") > 0f || Input.GetAxis("Mouse X") < 0f)
        {
            transform.Rotate(transform.up, Input.GetAxis("Mouse X") * rotateSpeed);
        }

        if (Input.GetAxis("Mouse Y") > 0f && internalRotation.x > -88f)
        {
            internalRotation.x -= Input.GetAxis("Mouse Y") * rotateSpeed;
        }

        if (Input.GetAxis("Mouse Y") < 0f && internalRotation.x < 88f)
        {
            internalRotation.x -= Input.GetAxis("Mouse Y") * rotateSpeed;
        }

        cam.localEulerAngles = internalRotation;

        // movement
        if (Input.GetKey(KeyCode.W))
        {
            if(grounded)
            {
                transform.position += (transform.forward * movementSpeed * superSpeed * Time.deltaTime);
            }
            else
            {
                rigidBody.AddRelativeForce(Vector3.forward * jetpackForce.y);
            }            
        }

        if (Input.GetKey(KeyCode.A))
        {
            if (grounded)
            {
                transform.position += (-transform.right * movementSpeed * superSpeed * Time.deltaTime);
            }
            else
            {
                rigidBody.AddRelativeForce(Vector3.left * jetpackForce.y);
            }            
        }

        if (Input.GetKey(KeyCode.S))
        {
            if (grounded)
            {
                transform.position += (-transform.forward * movementSpeed * superSpeed * Time.deltaTime);
            }
            else
            {
                rigidBody.AddRelativeForce(Vector3.back * jetpackForce.y);
            }            
        }

        if (Input.GetKey(KeyCode.D))
        {
            if (grounded)
            {
                transform.position += (transform.right * movementSpeed * superSpeed * Time.deltaTime);
            }
            else
            {
                rigidBody.AddRelativeForce(Vector3.right * jetpackForce.y);
            }            
        }

        if (Input.GetKey(KeyCode.E))
        {
            rigidBody.AddForce(jetpackForce);
            if(grounded) { UngroundPlayer(); }
        }

        if (Input.GetKey(KeyCode.Q))
        {
            rigidBody.AddForce(-jetpackForce);
        }

        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            rigidBody.AddForce(jumpForce);
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        GroundPlayer();
    }

    private void OnCollisionExit(Collision collision)
    {

    }

    void GroundPlayer()
    {
        rigidBody.useGravity = true;
        grounded = true;
    }

    void UngroundPlayer()
    {
        rigidBody.useGravity = false;
        grounded = false;
    }
}
