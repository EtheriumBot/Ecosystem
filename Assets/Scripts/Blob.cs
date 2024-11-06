using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class Blob : MonoBehaviour
{

    private GameManager gameManager;
    public enum State
    {
        Wandering,
        Chasing,
        Killing
    }

    private State _currentState = State.Wandering;

    public float accel;
    public float maxSpeed;
    private Quaternion target;
    private Rigidbody2D rb;

    public GameObject enemy;
    public List<Transform> enemyPositions;
    public float enemyTargetDist;

    public GameObject killzone;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();

        gameManager = GameManager.reference;

    }

    // Update is called once per frame
    void Update()
    {

        // Array of all the enemy positions 
        //enemyPositions = enemy.transform.GetComponentsInChildren<Transform>();
        enemyPositions = gameManager.predPositions;

        UpdateState();

    }

    private void UpdateState()
    {
        switch (_currentState)
        {
            case State.Wandering:
                //Exploring Code
                Move();

                //Switching to Chasing
                if (Vector2.Distance(this.transform.position, GetClosestEnemy(enemyPositions).position) < enemyTargetDist)
                {
                    StartState(State.Chasing);
                }

                //KILLING MYSELF
                if (Random.Range(0, 5000) == 0) GameObject.Destroy(this.gameObject);

                break;

            case State.Chasing:
                //Attacking Code
                Attacking();

                //Switching to Wandering
                if (Vector2.Distance(this.transform.position, GetClosestEnemy(enemyPositions).position) >= enemyTargetDist)
                {
                    StartState(State.Wandering);
                }

                //Switching to Killing
                if (Vector2.Distance(this.transform.position, GetClosestEnemy(enemyPositions).position) < enemyTargetDist/4)
                {
                    StartState(State.Killing);
                }

                break;

            case State.Killing:
                if (Time.frameCount % 300 == 0) GameObject.Instantiate(killzone, this.transform.position, Quaternion.identity);

                if (Time.frameCount % 1200 == 0) GameObject.Destroy(this.gameObject);

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
            case State.Wandering:
                Debug.Log("Blob Switching to Wandering");

                break;
            case State.Chasing:
                Debug.Log("Blob Switching to Chasing");

                break;
            case State.Killing:
                Debug.Log("Blob Switching to Killing");

                break;
        }

        _currentState = newState;
    }

    private void EndState(State oldState)
    {
        //Run any code that should run once at the ending of the previous state
        switch (oldState)
        {
            case State.Wandering:


                break;
            case State.Chasing:


                break;
            case State.Killing:


                break;
        }
    }

    //Call every frame to walk the ship forward based on their facing
    private void Move()
    {

        //Debug.Log("Moving");
        int r = Random.Range(0, 4);
        if (r == 0) rb.AddForce(this.transform.up * accel * Time.deltaTime * 500);
        else if (r == 1) rb.AddForce(this.transform.right * accel * Time.deltaTime * 500);
        else if (r == 2) rb.AddForce(-this.transform.up * accel * Time.deltaTime * 500);
        else rb.AddForce(-this.transform.right * accel * Time.deltaTime * 500);

        if (rb.velocity.magnitude > maxSpeed) rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
    }

    private void Attacking()
    {
        Transform target = GetClosestEnemy(enemyPositions);

        Vector3 vectorToTarget = target.position - transform.position;
        rb.AddForce(new Vector3(vectorToTarget.x * 0.01f, vectorToTarget.y * 0.01f, 0));
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
}
