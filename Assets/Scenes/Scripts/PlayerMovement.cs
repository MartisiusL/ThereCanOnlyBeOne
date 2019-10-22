using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float WalkVelocity = 16f;
    public float JumpVelocity = 28f;
    public float Gravity = 96f;
    public float Acceleration = 80f;

    private CharacterController _controller;
    private Vector3 _currentVelocity = Vector3.zero;
    private Vector3 _previousVelocity = Vector3.zero;
    
    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // Get velocity from last frame
        _currentVelocity = _previousVelocity;

        // Calculate new velocity
        _currentVelocity.x = GetHorizontalVelocity(_currentVelocity.x, GetHorizontalIntendedDirection());
        _currentVelocity.y = GetVerticalVelocity(_currentVelocity.y, GetVerticalIntendedDirection());

        // Save velocity for next frame
        _previousVelocity = _currentVelocity;

        // Move character controller
        _controller.Move(_currentVelocity * Time.deltaTime);
    }

    private int GetHorizontalIntendedDirection()
    {
        var direction = 0;
        
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            direction += 1;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            direction -= 1;

        return direction;
    }
    
    private int GetVerticalIntendedDirection()
    {
        var direction = 0;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space))
            direction = 1;

        return direction;
    }

    private float GetHorizontalVelocity(float currentVelocityX, int directionX)
    {
        var maxVelocity = WalkVelocity * directionX;

        if (_controller.collisionFlags == CollisionFlags.Sides)
            currentVelocityX = 0f;

        // If no acceleration or maxVelocity is reached
        if (Acceleration == 0f || (currentVelocityX == maxVelocity))
            return currentVelocityX;

        // Allows local variable to be inverted
        var acceleration = Acceleration;

        // If velocity should be decremented
        if(maxVelocity < 0 || (currentVelocityX > 0f && maxVelocity == 0f))
            acceleration *= -1f;

        // Change velocity
        currentVelocityX += acceleration * Time.deltaTime;

        // If overshoot maxVelocity of 0f
        if((maxVelocity == 0f) &&
            ((acceleration < 0f && currentVelocityX < 0f) ||
                (acceleration > 0f && currentVelocityX > 0f)))
            return 0f;
        
        // If overshoot negative maxVelocity
        if(maxVelocity < 0f)
        {
            if (currentVelocityX <= maxVelocity)
                return maxVelocity;
        }
        
        // If overshoot positive maxVelocity
        if (maxVelocity > 0f)
        {
            if (currentVelocityX >= maxVelocity)
                return maxVelocity;
        }

        return currentVelocityX;
    }

    private float GetVerticalVelocity(float currentVelocityY, int directionY)
    {
        // Resets velocity when on a surface
        // Set as negative to prevent bumping when moving down slopes
        if (_controller.collisionFlags == CollisionFlags.Below ||
            _controller.collisionFlags == CollisionFlags.Above)
            currentVelocityY = -4f;

        // Apply gravity
        currentVelocityY -= Gravity * Time.deltaTime;

        // Apply jump
        if (directionY > 0f)
            currentVelocityY = JumpVelocity;

        return currentVelocityY;
    }
}