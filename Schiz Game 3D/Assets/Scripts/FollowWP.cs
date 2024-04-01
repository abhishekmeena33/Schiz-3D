using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowWP : MonoBehaviour
{
    public Transform middleWaypoint; // Middle waypoint of the "X" shape
    public Transform[] waypoints; // Waypoints representing the corners of the "X" shape
    private int currentWaypointIndex = 0;
    private float movementSpeed = 5f;
    //private bool moveToMiddleWaypoint = true;
    public bool canClick = true; // Flag to indicate if clicking on waypoints is allowed
    private enum MovementState { Idle, MovingToMiddle, MovingToWaypoint };
    private MovementState movementState = MovementState.Idle;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("IsWalking", false);
    }

    private void Update()
    {
        

        switch (movementState)
        {

            case MovementState.Idle:
                
                // Check for mouse click only if clicking is allowed
                if (canClick && Input.GetMouseButtonDown(0))
                {
                    // Cast a ray from the mouse position to the world
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    // Check if the ray hits something
                    if (Physics.Raycast(ray, out hit))
                    {
                        // Check if the object hit is a waypoint
                        for (int i = 0; i < waypoints.Length; i++)
                        {
                            if (hit.collider.transform == waypoints[i])
                            {
                                SetDestination(waypoints[i]);
                                return;
                            }
                            else
                            {
                                movementSpeed = 0f;
                            }
                        }
                    }
                }
                break;

            case MovementState.MovingToMiddle:
                movementSpeed = 5f;
                // Move towards the middle waypoint
                MoveToWaypoint(middleWaypoint.position);

                // Check if reached the middle waypoint
                if (Vector3.Distance(transform.position, middleWaypoint.position) < 0.1f)
                {
                    //moveToMiddleWaypoint = false;
                    canClick = true; // Allow clicking on waypoints
                    movementState = MovementState.Idle;
                }
                break;

            case MovementState.MovingToWaypoint:
                movementSpeed = 5f;
                // Move towards the current waypoint
                MoveToWaypoint(waypoints[currentWaypointIndex].position);

                // Check if reached the current waypoint
                if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < 0.1f)
                {
                    // Increment waypoint index to move to the next waypoint
                    currentWaypointIndex++;
                    canClick = false; // Disallow clicking on waypoints until returning to the middle waypoint

                    // Loop back to the first waypoint if reached the last one
                    if (currentWaypointIndex >= waypoints.Length)
                    {
                        currentWaypointIndex = 0;
                    }
                    movementState = MovementState.MovingToMiddle;
                }
                break;
        }

        // Update animator parameters
        UpdateAnimatorParameters();
    }

    private void MoveToWaypoint(Vector3 targetPosition)
    {
        // Calculate direction vector towards the target position
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Move towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);

        // Rotate to face the direction of movement
        transform.rotation = Quaternion.LookRotation(direction);
    }

    // Method to set the destination waypoint
    public void SetDestination(Transform destination)
    {
        // Reset the waypoint index and enable movement towards the destination
        currentWaypointIndex = 0;
        canClick = false; // Disallow clicking on waypoints until returning to the middle waypoint
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (destination == waypoints[i])
            {
                currentWaypointIndex = i;
                movementState = MovementState.MovingToWaypoint;
                break;
            }
        }
    }

    private void UpdateAnimatorParameters()
    {

        animator.SetFloat("Speed", movementSpeed * Time.deltaTime);
        float speed = animator.GetFloat("Speed");
        

        if (speed > 0)
        {
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }
    }
}
