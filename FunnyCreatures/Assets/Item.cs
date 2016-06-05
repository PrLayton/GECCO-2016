using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour
{

    public bool dangerous;
    public int stronger;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    int numberOfCollisions;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "head")
        {
            numberOfCollisions++;
            if (numberOfCollisions > 2)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
