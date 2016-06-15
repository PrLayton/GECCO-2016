using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Earth : MonoBehaviour {

    public string code; // = "LUIBRLBFL"
    //private GameObject creature;
    public float respawnTime = 1.0f;
    private float timeCount;
    private List<Node> nodes = new List<Node>();
    private List<List<Node>> allnodes = new List<List<Node>>();
    private List<bool> bs = new List<bool>();
    private List<float> fs = new List<float>();
    public Material mat;

    public List<Item> items = new List<Item>();
    public Item[] itemsArray;

    public Transform destination;

    public float brainTimer;
    public float brainTimerMin = 0;
    public float brainTimerMax = 2;

    public bool generateCode;

    public Material headMat;
    
    public GameObject[] monstersArray;

    struct Node
    {
        public GameObject part;
        public int angle;
        public Vector3 axe;
        public int numberFriend;
    }

	void Start () {
        if(generateCode)
            GenerateCode();
        MakeCreature();
        //SetCreature();
    }

    void MakeCreature()
    {
        //Destroy(creature);
        for (int j = 0; j < monstersArray.Length; j++)
        {
            List<Node> nodes2 = new List<Node>();

            for (int i = 0; i <= code.Length - 4; i += 4)
            {
                Node node = new Node();
                node.part = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                if (i == 0)
                {
                    node.part.tag = "head";
                    if (mat != null)
                        node.part.GetComponent<Renderer>().material = headMat;
                    node.part.transform.parent = monstersArray[j].transform;
                    node.part.transform.position = monstersArray[j].transform.position;
                }
                else
                {
                    node.part.GetComponent<Renderer>().material = mat;
                    node.part.transform.parent = monstersArray[j].transform;
                }
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
                switch (code[i + 1])
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
                        node.angle = 90;
                        break;
                    case 'T':
                        node.angle = 45;
                        break;
                    default:
                        break;
                }
                if (int.Parse(code[i + 3].ToString()) < i / 4)
                {
                    node.numberFriend = int.Parse(code[i + 3].ToString());
                }
                else
                {
                    node.numberFriend = nodes2.Count - 1; ;
                }
                
                if (i > 0)
                {
                    node.part.transform.position = nodes2[node.numberFriend].part.transform.position;
                    //node.part.transform.Translate(new Vector3(0, nodes2[node.numberFriend].part.transform.position.y + nodes2[node.numberFriend].part.transform.localScale.y + node.part.transform.localScale.y - 0.5f));
                    node.part.transform.position = nodes2[node.numberFriend].part.transform.position;
                    node.part.transform.Translate(new Vector3(0, nodes2[node.numberFriend].part.transform.localScale.y));
                    node.part.AddComponent<CharacterJoint>();
                    node.part.GetComponent<CharacterJoint>().anchor = new Vector3(0, -1, 0);
                    node.part.GetComponent<CharacterJoint>().axis = new Vector3(-1, 0, 0);
                    node.part.GetComponent<CharacterJoint>().connectedBody = nodes2[node.numberFriend].part.GetComponent<Rigidbody>();
                }

                nodes2.Add(node);
                fs.Add(0.0f);
                bs.Add(false);

            }
            allnodes.Add(nodes2);

            if (generateCode)
                GenerateCode();
        }
    }

    void SetCreature()
    {
        for (int i = 1; i < nodes.Count; i++)
        {
            nodes[i].part.transform.Translate(new Vector3(0, nodes[nodes[i].numberFriend].part.transform.position.y + nodes[nodes[i].numberFriend].part.transform.localScale.y + nodes[i].part.transform.localScale.y -0.5f));
            nodes[i].part.AddComponent<CharacterJoint>();
            nodes[i].part.GetComponent<CharacterJoint>().anchor = new Vector3(0, -1, 0);
            nodes[i].part.GetComponent<CharacterJoint>().axis = new Vector3(-1, 0, 0);
            nodes[i].part.GetComponent<CharacterJoint>().connectedBody = nodes[nodes[i].numberFriend].part.GetComponent<Rigidbody>();
        }
    }
	
	void Update () {
        timeCount += Time.deltaTime;
        if(timeCount >= respawnTime)
        {
            //MakeCreature();
            timeCount = 0;
        }

        if (allnodes.Count > 0)
            CalculateHead();
    }

    void FixedUpdate()
    {
        for (int j = 0; j < allnodes.Count; ++j)
        {
            for (int i = 1; i < allnodes[j].Count; i++)
            {
                if (bs[i] == false)
                {
                    if (fs[i] > -allnodes[j][i].angle)
                    {
                        fs[i]--;
                        allnodes[j][i].part.transform.Rotate(allnodes[j][i].axe, Time.deltaTime * -50);
                        if (fs[i] <= -allnodes[j][i].angle)
                        {
                            bs[i] = true;
                        }
                    }
                }
                else
                {
                    if (fs[i] < allnodes[j][i].angle)
                    {
                        fs[i]++;
                        allnodes[j][i].part.transform.Rotate(allnodes[j][i].axe, Time.deltaTime * 50);
                        if (fs[i] >= allnodes[j][i].angle)
                        {
                            bs[i] = false;
                        }
                    }
                }
            }


            if (allnodes[j].Count > 0 && destination != null)
                MoveHead();
        }
    }

    void CalculateHead()
    {
        for (int j = 0; j < allnodes.Count; j++)
        {
            for (int i = 0; i < itemsArray.Length; i++)
            {
                if (Vector3.Distance(itemsArray[i].transform.position, allnodes[j][0].part.transform.position) < 50f && itemsArray[i].gameObject.activeSelf)
                {
                    if (!items.Contains(itemsArray[i]))
                    {
                        items.Add(itemsArray[i]);
                    }
                }
                else
                {
                    if (items.Contains(itemsArray[i]))
                    {
                        items.Remove(itemsArray[i]);
                        if (items.Count == 0)
                            destination = null;
                    }
                }
            }
            float minDist = 1000.0f;
            foreach (var i in items)
            {
                if (Vector3.Distance(i.transform.position, allnodes[j][0].part.transform.position) < minDist)
                {
                    minDist = Vector3.Distance(i.transform.position, allnodes[j][0].part.transform.position);
                    destination = i.transform;
                }
            }
        }
    }

    void MoveHead()
    {
        for (int j = 0; j < allnodes.Count; j++)
        {
            brainTimer += Time.deltaTime;
            if (brainTimer > Random.Range(brainTimerMin, brainTimerMax))
            {
                for (int i = 0; i < monstersArray.Length; i++)
                {
                    Vector3 direction = (destination.position - allnodes[j][0].part.transform.position).normalized;
                    allnodes[j][0].part.GetComponent<Rigidbody>().AddForce(direction, ForceMode.Impulse);
                    brainTimer = 0;
                }
            }

            Camera.main.transform.LookAt(allnodes[j][0].part.transform);
        }
    }

    void GenerateCode()
    {
        code = "";
        int max = Random.Range(4, 41);
        for (int i = 0; i < max; i+=4)
        {
            if (Random.Range(0.0f,1.0f) > 0.5f)
            {
                code += 'L';
            }
            else
            {
                code += 'B';
            }

            float value = Random.Range(0.0f, 1.0f);
            if (value > 0.33f)
            {
                code += 'U';
            }
            else if (value > 0.66f)
            {
                code += 'R';
            }
            else
            {
                code += 'F';
            }

            if (Random.Range(0.0f, 1.0f) > 0.5f)
            {
                code += 'I';
            }
            else
            {
                code += 'T';
            }
                code += Random.Range(0,i/4);
        }
        Debug.Log(code);
    }

    public string Mutate(string adn,int nbMutation)
    {
        char[] newDna = adn.ToCharArray();

        for(int i = 0; i < nbMutation; i ++)
        {
            int posrdm = Random.Range(0, newDna.Length);

            //mutation du gène en fonction de la position
            //Mutation du membre auquel est rataché
            if(posrdm%4 == 0)
            {
                int rdm = Random.Range(0, newDna.Length%4);
                newDna[posrdm] = (char)rdm;
            }
            //Muration de la taille
            else if (posrdm%4 == 1)
            {
                if (newDna[posrdm] == 'B')
                {
                    newDna[posrdm] = 'L';
                }
                else
                {
                    newDna[posrdm] = 'B';
                }
            }
            //Mutation de la rotation
            else if (posrdm%4 == 2)
            {
                bool ok = false;
                while (!ok)
                {
                    float value = Random.Range(0.0f, 1.0f);
                    if (value > 0.33f && newDna[posrdm] != 'U')
                    {
                        newDna[posrdm] = 'U';
                        ok = true;
                    }
                    else if (value > 0.66f && newDna[posrdm] != 'R')
                    {
                        newDna[posrdm] = 'R';
                        ok = true;
                    }
                    else if (newDna[posrdm] != 'F')
                    {
                        newDna[posrdm] = 'F';
                        ok = true;
                    }
                }
            }
            //Mutation de l'angle
            else if (posrdm % 4 == 3)
            {
                if (newDna[posrdm] == 'T')
                {
                    newDna[posrdm] = 'I';
                }
                else
                {
                    newDna[posrdm] = 'T';
                }
            }
        }

        return new string(newDna);
    }
}
