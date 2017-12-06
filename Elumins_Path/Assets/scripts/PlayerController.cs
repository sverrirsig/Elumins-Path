﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CameraSmooth2D;

public class PlayerController : MonoBehaviour
{

    enum Gem_Types { NONE = 0, ROTATE, MOVE, BOTH };
    public float speed;             //Floating point variable to store the player's movement speed.

    private Vector3 PortalPosition;
    private bool IsCrystal;
    private bool EnterableCrystal;
    private Rigidbody2D rb2d;       //Store a reference to the Rigidbody2D component required to use 2D Physics.

    private Rigidbody2D Reflected_body;

    private Collider2D reflect_collider;

    private bool SpacePressed = false;

    private Gem_Types CurrentCollidedGem;
    // Use this for initialization
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        PortalPosition.x = 2.0f;
        PortalPosition.y = 2.0f;
        PortalPosition.z = 0f;
        IsCrystal = false;
        EnterableCrystal = false;
        CurrentCollidedGem = Gem_Types.NONE;
    }
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            SpacePressed = true;
        }
    }
    void FixedUpdate()
    {
        CrystalIteractionExit();
        MovePlayer();
    }
    void OnTriggerStay2D(Collider2D col)
    {
        SetCurrentGemType(col.gameObject.tag);
        if (col.transform.parent == null && SpacePressed == true)
        {

            gameObject.GetComponentInChildren<ParticleSystem>().Stop();
            col.gameObject.transform.parent = transform;
            IsCrystal = true;
            col.isTrigger = false;
            Reflected_body = col.GetComponent<Rigidbody2D>();
            Reflected_body.mass = gameObject.GetComponent<Rigidbody2D>().mass;
            gameObject.GetComponent<Collider2D>().enabled = false;
            gameObject.GetComponent<Collider2D>().isTrigger = false;
            gameObject.GetComponent<Rigidbody2D>().isKinematic = true;

            Transform[] children = transform.GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                if (child.parent == transform) child.parent = col.gameObject.transform;
            }
            GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
            camera.GetComponent<CameraManager>().focusObject = col.gameObject;
            SpacePressed = false;

        }
    }
    void SetCurrentGemType(string tag)
    {
        switch (tag)
        {
            case ("Reflective_Gem"):
                CurrentCollidedGem = Gem_Types.ROTATE;
                reflect_collider = GameObject.FindGameObjectWithTag("Reflective_Gem").GetComponent<PolygonCollider2D>();
                break;
            default:
                CurrentCollidedGem = Gem_Types.NONE;
                break;

        }
    }
    void CrystalIteractionExit()
    {
        if (IsCrystal && SpacePressed == true)
        {
            reflect_collider.gameObject.transform.parent = null;
            IsCrystal = false;
            reflect_collider.isTrigger = true;
            Reflected_body.mass = 10000;
            gameObject.transform.position = reflect_collider.gameObject.transform.position;
            Transform[] children = reflect_collider.transform.GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                if (child.parent == reflect_collider.transform) child.parent = transform;
            }
            Collider2D temp_col = gameObject.GetComponent<Collider2D>();
            temp_col.enabled = true;
            temp_col.isTrigger = false;
            gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
            GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
            camera.GetComponent<CameraManager>().focusObject = gameObject;
            gameObject.GetComponentInChildren<ParticleSystem>().Play();
            SpacePressed = false;
        }
    }
    Vector2 GetMovement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(moveHorizontal, moveVertical);
        return movement;
    }
    void MovePlayer()
    {
        Vector2 movement = GetMovement();
        rb2d.AddForce(movement * speed);
        if (IsCrystal)
        {
            switch (CurrentCollidedGem)
            {
                case (Gem_Types.MOVE):
                    Reflected_body.AddForce(movement * speed);
                    break;
                case (Gem_Types.ROTATE):
                    RotateCrystal();
                    break;
                case (Gem_Types.BOTH):
                    Reflected_body.AddForce(movement * speed);
                    RotateCrystal();
                    break;
            }
        }
    }
    void RotateCrystal()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            reflect_collider.gameObject.transform.Rotate(new Vector3(0, 0, 90) * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            reflect_collider.gameObject.transform.Rotate(new Vector3(0, 0, -90) * Time.deltaTime);
        }
    }
}

