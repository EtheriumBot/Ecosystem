using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Ship : MonoBehaviour
{
    private GameManager gameManager;

    public enum State
    {
        Exploring,
        Attacking,
        Retreating
    }

    private State _currentState = State.Exploring;

    public float accel;
    public float maxSpeed;
    private Quaternion target;
    private Rigidbody2D rb;

    public GameObject enemy;
    public List<Transform> enemyPositions;
    public float enemyTargetDist;

    private int retreatingCount;


    private void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();

        // Rotate the ship by converting the angles into a quaternion.
        target = Quaternion.Euler(0, 0, Random.Range(0, 360));

        gameManager = GameManager.reference;

        // Array of all the enemy positions 
        //enemyPositions = enemy.transform.GetComponentsInChildren<Transform>();


        gameManager.predPositions.Add(this.transform); //adds my transform to the end

    }

    private void Update()
    {
        UpdateState();

        //Debug.Log(transform.up);

        // Array of all the enemy positions 
        //enemyPositions = enemy.transform.GetComponentsInChildren<Transform>();
        enemyPositions = gameManager.preyPositions;
    }

    private void UpdateState()
    {
        switch (_currentState)
        {
            case State.Exploring:
                //Exploring Code
                Move();
                ChangeRotation();
                
                //Swiching to Attacking
                if (Vector2.Distance(this.transform.position, GetClosestEnemy(enemyPositions).position) < enemyTargetDist)
                {
                    StartState(State.Attacking);
                }
                break;

            case State.Attacking:
                //Attacking Code
                Move();
                Targeting();

                //Switching to Exploring
                if (Vector2.Distance(this.transform.position, GetClosestEnemy(enemyPositions).position) >= enemyTargetDist + 1)
                {
                    StartState(State.Exploring);
                }

                //Switching to Retreating
                if (Time.frameCount % 2000 == 0) StartState(State.Retreating);

                break;

            case State.Retreating:
                //Retreating Code
                BackMove();

                retreatingCount++;

                if (retreatingCount > 250)
                {
                    StartState(State.Exploring);
                }
                


                break;

        }
    }

    public void StartState(State newState)
    {
        //First run the EndState of our old state
        //Clean up loose ends from whatever state needs it
        EndState(_currentState);

        //Run any code that should run once at the beginning of a new state
        switch (newState)
        {
            case State.Exploring:
                Debug.Log("Switching to Exploring");

                break;
            case State.Attacking:
                Debug.Log("Switching to Attacking");

                break;
            case State.Retreating:
                Debug.Log("Switching to Retreating");

                retreatingCount = 0;

                break;
        }

        _currentState = newState;
    }

    private void EndState(State oldState)
    {
        //Run any code that should run once at the ending of the previous state
        switch (oldState)
        {
            case State.Exploring:


                break;
            case State.Attacking:
                

                break;
            case State.Retreating:

                retreatingCount = 0;

                break;
        }
    }

    public void ChangeRotation()
    {
        //Debug.Log(target.z + " " + this.transform.rotation.z);
        if (Mathf.Abs(target.z - Mathf.Abs(this.transform.rotation.z)) < 0.05 && Random.Range(0, 500) == 0 ||
            Random.Range(0, 5000) == 0)
        {
            target = Quaternion.Euler(0, 0, Random.Range(0, 360));
            //Debug.Log("Switching Target");
        }

        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, target, Time.deltaTime * 2);
    }

    //Call every frame to walk the ship forward based on their facing
    private void Move()
    {
        //Debug.Log("Moving");
        rb.AddForce(this.transform.up * accel * Time.deltaTime * 1000);
        if (rb.velocity.magnitude > maxSpeed) rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
    }

    private void BackMove()
    {
        //Debug.Log("Moving");
        rb.AddForce(-this.transform.up * accel * Time.deltaTime * 2000);
        if (rb.velocity.magnitude > maxSpeed) rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
    }

    private void Targeting()
    {
        Transform target = GetClosestEnemy(enemyPositions);

        Vector3 vectorToTarget = target.position - transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 90;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 2);
    }

    private Transform GetClosestEnemy(List<Transform> enemies)
    { 
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (Transform potentialTarget in enemies)
        {
            Vector2 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        //float dist= //the distance between bestTarget and transform.position

        //if(dist>=the maximum distance that the triangle is able to detect the enenmy)
        //    return null;

        return bestTarget;
    }

    private void OnDestroy()
    {
        gameManager.predPositions.Remove(this.transform);
    }
}
