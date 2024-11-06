using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prey : MonoBehaviour
{

    private GameManager gameManager;
    public enum State
    {
        Wandering,
        Fleeing,
        Spawning
    }

    private State _currentState = State.Wandering;

    public float accel;
    public float maxSpeed;
    private Quaternion target;
    private Rigidbody2D rb;

    public GameObject enemy;
    public List<Transform> enemyPositions;
    public float enemyTargetDist;

    public GameObject blob;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();

        gameManager = GameManager.reference;

        gameManager.preyPositions.Add(this.transform); //adds my transform to the end
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

                //Switching to Attacking
                if (Vector2.Distance(this.transform.position, GetClosestEnemy(enemyPositions).position) < enemyTargetDist)
                {
                    StartState(State.Fleeing);
                }

                //Switching to Spawning
                if (Random.Range(0,4000) == 0)
                {
                    StartState(State.Spawning);
                }

                break;

            case State.Fleeing:
                //Attacking Code
                Avoiding();

                //Switching to Exploring
                if (Vector2.Distance(this.transform.position, GetClosestEnemy(enemyPositions).position) >= enemyTargetDist + 1)
                {
                    StartState(State.Wandering);
                }
                break;

            case State.Spawning:
                //Spawning Code
                if (Time.frameCount%300 == 0) GameObject.Instantiate(blob, new Vector3(this.transform.position.x-0.5f, this.transform.position.y-0.5f, -1), Quaternion.identity);

                //Switching to Exploring
                if (Vector2.Distance(this.transform.position, GetClosestEnemy(enemyPositions).position) < enemyTargetDist)
                {
                    StartState(State.Fleeing);
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
            case State.Wandering:
                Debug.Log("UFO Switching to Wandering");

                break;
            case State.Fleeing:
                Debug.Log("UFO Switching to Fleeing");

                break;
            case State.Spawning:
                Debug.Log("UFO Switching to Spawning");

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
            case State.Fleeing:


                break;
            case State.Spawning:


                break;
        }
    }

    //Call every frame to walk the ship forward based on their facing
    private void Move()
    {
        if (Random.Range(1, 1000) == 1)
        {
            //Debug.Log("Moving");
            int r = Random.Range(0, 4);
            if (r == 0) rb.AddForce(this.transform.up * accel * Time.deltaTime * 2000);
            else if (r == 1) rb.AddForce(this.transform.right * accel * Time.deltaTime * 2000);
            else if (r == 2) rb.AddForce(-this.transform.up * accel * Time.deltaTime * 2000);
            else rb.AddForce(-this.transform.right * accel * Time.deltaTime * 2000);

            if (rb.velocity.magnitude > maxSpeed) rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
        }
    }

    private void Avoiding()
    {
        Transform target = GetClosestEnemy(enemyPositions);

        Vector3 vectorToTarget = target.position - transform.position;
        rb.AddForce(new Vector3 (-vectorToTarget.x*0.01f, -vectorToTarget.y*0.01f, 0));
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
        gameManager.preyPositions.Remove(this.transform);
    }
}
