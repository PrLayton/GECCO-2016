using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Item : MonoBehaviour
{

    public bool dangerous;
    public int stronger;
    public bool actived = true;

    [SerializeField]
    Earth earth;

    Transform startPosition;

    // Use this for initialization
    void Start()
    {
        startPosition = transform;
    }

    // Update is called once per frame
    void Update()
    {

    }

    int numberOfCollisions;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "head" || collision.gameObject.tag == "head0" || collision.gameObject.tag == "head1" || collision.gameObject.tag == "head2" || collision.gameObject.tag == "head3" || collision.gameObject.tag == "head4"
            || collision.gameObject.tag == "head5" || collision.gameObject.tag == "head6" || collision.gameObject.tag == "head7" || collision.gameObject.tag == "head8" || collision.gameObject.tag == "head9")
        {
            numberOfCollisions++;
            if (numberOfCollisions > 2)
            {
                //print("true");
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
                //print("end");
                actived = false;
                GetComponent<MeshRenderer>().enabled = false;
                GetComponent<SphereCollider>().enabled = false;
                StartCoroutine(Respawn());
            }
        }
    }

    IEnumerator Respawn()
    {
       // gameObject.SetActive(false);
        yield return new WaitForSeconds(5.0f);
        transform.position = new Vector3(startPosition.position.x, 1, startPosition.position.z);
        GetComponent<SphereCollider>().enabled = true;
        GetComponent<MeshRenderer>().enabled = true;
        //transform.rotation = startPosition.rotation;
        actived = true;
        //gameObject.SetActive(true);
    }
}
