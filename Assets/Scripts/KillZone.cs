using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class KillZone : MonoBehaviour
{

    private int timePassed;

    // Start is called before the first frame update
    void Start()
    {
        timePassed = 0;
    }

    private void Update()
    {
        Debug.Log(timePassed);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timePassed++;

        if (timePassed >= 500)
        {
            GameObject.Destroy(this);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (timePassed >= 100)
        {
            GameObject.Destroy(collision.gameObject);
            GameObject.Destroy(this);
        }
    }
}
