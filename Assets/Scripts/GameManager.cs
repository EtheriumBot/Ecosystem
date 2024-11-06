using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{

    public static GameManager reference;

    public List<Transform> preyPositions;
    public List<Transform> predPositions;

    public GameObject pred;
    public GameObject prey;

    private int spawnTimer;


    // Start is called before the first frame update
    void Awake()
    {
        reference = this;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        spawnTimer++;

        if (spawnTimer % 900 == 0)
        {
            GameObject.Instantiate(pred, new Vector3(Random.Range(-8, 8), Random.Range(-4, 4), -1), Quaternion.identity);
        } else if (spawnTimer % 1500 == 0)
        {
            GameObject.Instantiate(prey, new Vector3(Random.Range(-8, 8), Random.Range(-4, 4), -1), Quaternion.identity);
        }
    }
}
