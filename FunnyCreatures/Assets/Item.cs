using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Item : MonoBehaviour
{

    public bool dangerous;
    public int stronger;

    [SerializeField]
    Earth earth;

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
                print("true");
                for (int i = 0; i < earth.monsters.Count; i++)
                {
                    for (int j = 0; j < earth.monsters[i].member.Count; j++)
                    {
                        if(earth.monsters[i].member[j].part == collision.gameObject)
                        {
                            Earth.Creature c = earth.monsters[i];
                            c.miam = true;
                            earth.monsters[i] = c;
                            break;
                        }
                    }
                }
                print("end");
                gameObject.SetActive(false);
            }
        }
    }
}
