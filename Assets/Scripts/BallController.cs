﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 15.0f;

    private bool isMoving;
    private Vector3 travelDirection;
    private Vector3 nextCollisionPosition;

    public int minSwipeDirection = 500;
    private Vector2 swipePosLastFrame;
    private Vector2 swipePosCurrentFrame;
    private Vector2 currentSwipe;

    private Color solveColor;

    private void Start()
    {
        solveColor = Random.ColorHSV(0.5f, 1.0f);
        GetComponent<MeshRenderer>().material.color = solveColor;
    }

    private void FixedUpdate()
    {
        if(isMoving)
        {
            rb.velocity = speed * travelDirection;
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position - (Vector3.up / 2), 0.5f);
        int i = 0;

        while(i < hitColliders.Length)
        {
            GroundController ground = hitColliders[i].transform.GetComponent<GroundController>();
            if(ground && !ground.isColored)
            {
                ground.ChangeColor(solveColor);
            }
            i++;
        }

        if(nextCollisionPosition != Vector3.zero)
        {
            if(Vector3.Distance(transform.position, nextCollisionPosition) < 1)
            {
                isMoving = false;
                travelDirection = Vector3.zero;
                nextCollisionPosition = Vector3.zero;
            }
        }

        if(isMoving)
            return;

        
        if(Input.GetMouseButton(0))
        {
            swipePosCurrentFrame = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            if(swipePosLastFrame != Vector2.zero)
            {
                currentSwipe = swipePosCurrentFrame - swipePosLastFrame;

                if(currentSwipe.sqrMagnitude < minSwipeDirection)
                {
                    return;
                }

                currentSwipe.Normalize();

                //Up/Down
                if(currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
                {
                    //Go Up/Down
                    SetDestination(currentSwipe.y > 0 ? Vector3.forward : Vector3.back);
                }

                if(currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
                {
                    //Go Left/Right
                    SetDestination(currentSwipe.x > 0 ? Vector3.right : Vector3.left);
                }

            }
            swipePosLastFrame = swipePosCurrentFrame;
        }
        if(Input.GetMouseButtonUp(0))
        {
            swipePosLastFrame = Vector2.zero;
            currentSwipe = Vector2.zero;
        }
    }
    private void SetDestination(Vector3 direction)
    {
        travelDirection = direction;

        RaycastHit hit;
        if(Physics.Raycast(transform.position, direction, out hit, 100f))
        {
            nextCollisionPosition = hit.point;
        }

        isMoving = true;
    }

}
