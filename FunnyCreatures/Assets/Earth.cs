using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Earth : MonoBehaviour {

    public string code; // = "LUIBRLBFL"
    //private GameObject creature;
    public float respawnTime = 1.0f;
    private float timeCount;
    private List<Node> nodes = new List<Node>();
    private List<bool> bs = new List<bool>();
    private List<float> fs = new List<float>();

    struct Node
    {
        public GameObject part;
        public int angle;
        public Vector3 axe;
    }

	void Start () {
        MakeCreature();
        SetCreature();
    }

    void MakeCreature()
    {
        //Destroy(creature);
        for (int i = 0; i <= code.Length-3; i+=3)
        {
            Node node = new Node();
            node.part = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            node.part.AddComponent<Rigidbody>();
            node.part.GetComponent<Rigidbody>().useGravity = false;
            //node.part.GetComponent<Rigidbody>().isKinematic = true;
            switch (code[i])
            {
                case 'L':
                    node.part.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    break;
                case 'B':
                    node.part.transform.localScale = new Vector3(1.0f, 2.0f, 1.0f);
                    break;
                default:
                    break;
            }
            switch (code[i+1])
            {
                case 'U':
                    node.axe = new Vector3(0, 1, 0);
                    break;
                case 'R':
                    node.axe = new Vector3(1, 0, 0);
                    break;
                case 'F':
                    node.axe = new Vector3(0, 0, 1);
                    break;
                default:
                    break;
            }
            switch (code[i + 2])
            {
                case 'I':
                    node.angle = 180;
                    break;
                case 'T':
                    node.angle = 90;
                    break;
                default:
                    break;
            }
            nodes.Add(node);
            fs.Add(0.0f);
            bs.Add(false);
        }
    }

    void SetCreature()
    {
        for (int i = 0; i < nodes.Count-1; i++)
        {
            nodes[i + 1].part.transform.Translate(new Vector3(0, nodes[i].part.transform.position.y + nodes[i].part.transform.localScale.y + nodes[i+1].part.transform.localScale.y -0.5f));
            nodes[i].part.AddComponent<CharacterJoint>();
            nodes[i].part.GetComponent<CharacterJoint>().connectedBody = nodes[i + 1].part.GetComponent<Rigidbody>();
        }

    }
	
	void Update () {
        timeCount += Time.deltaTime;
        if(timeCount >= respawnTime)
        {
            //MakeCreature();
            timeCount = 0;
        }
	}

    void FixedUpdate()
    {
        for (int i = 0; i < nodes.Count; i++) {
            if (bs[i] == false)
            {
                if (fs[i] > -nodes[i].angle)
                {
                    fs[i]--;
                    nodes[i].part.transform.Rotate(nodes[i].axe, Time.deltaTime * -200);
                    if (fs[i] <= -nodes[i].angle)
                    {
                        bs[i] = true;
                    }
                }
            }
            else
            {
                if (fs[i] < nodes[i].angle)
                {
                    fs[i]++;
                    nodes[i].part.transform.Rotate(nodes[i].axe, Time.deltaTime * 200);
                    if (fs[i] >= nodes[i].angle)
                    {
                        bs[i] = false;
                    }
                }
            }
        }
    }


    /*case 'E':
    GameObject eye = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    eye.GetComponent<Collider>().isTrigger = true;
    eye.GetComponent<Renderer>().enabled = false;
    eye.transform.parent = creature.transform;
    eye.AddComponent<FixedJoint>();
    eye.GetComponent<FixedJoint>().connectedBody = creature.GetComponent<Rigidbody>();
    break;
    */
}
