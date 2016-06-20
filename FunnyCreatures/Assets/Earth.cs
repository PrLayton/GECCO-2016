using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Earth : MonoBehaviour {

    public string code; // = "LUI0BRL0BFL0"
    //private GameObject creature;
    public float respawnTime = 1.0f;
    public int numberMutation = 5;
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
    
    private int nbGeneration;
    public GameObject[] monstersArray;
    private List<string> codes = new List<string>();
    private List<Creature> monsters = new List<Creature>();


    public float[] debugFitness;

    struct Node
    {
        public GameObject part;
        public int angle;
        public Vector3 axe;
        public int numberFriend;
    }

    struct Creature
    {
        public int id;
        public List<Node> member;
        public string code;
        public float fitness;
        public Transform target;
    }


    void Start () {
        if(generateCode)
            GenerateCode();
        MakeCreature();
        nbGeneration = 1;
        //SetCreature();
        debugFitness = new float[8];
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
                    node.part.transform.Translate(new Vector3(0, nodes2[node.numberFriend].part.transform.localScale.y + 0.5f));
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
            else
                code = Mutate(code, numberMutation);

            Creature creature = new Creature();
            creature.id = j;
            creature.member = nodes2;
            creature.code = code;
            creature.id = 0;
            monsters.Add(creature);

            codes.Add(code);
            Debug.Log(code);
        }
    }

    /*void SetCreature()
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
	*/

	void Update () {
        timeCount += Time.deltaTime;
        if(timeCount >= respawnTime)
        {
            //MakeCreature();
            newGeneration();
            timeCount = 0;
        }

        if (allnodes.Count > 0)
            CalculateHead();
    }

    void FixedUpdate()
    {
        for (int j = 0; j < monsters.Count; ++j)
        {
            for (int i = 1; i < monsters[j].member.Count; i++)
            {
                if (bs[i] == false)
                {
                    if (fs[i] > -monsters[j].member[i].angle)
                    {
                        fs[i]--;
                        monsters[j].member[i].part.transform.Rotate(monsters[j].member[i].axe, Time.deltaTime * -50);
                        if (fs[i] <= -monsters[j].member[i].angle)
                        {
                            bs[i] = true;
                        }
                    }
                }
                else
                {
                    if (fs[i] < monsters[j].member[i].angle)
                    {
                        fs[i]++;
                        monsters[j].member[i].part.transform.Rotate(monsters[j].member[i].axe, Time.deltaTime * 50);
                        if (fs[i] >= monsters[j].member[i].angle)
                        {
                            bs[i] = false;
                        }
                    }
                }
            }


            if (monsters[j].member.Count > 0 && monsters[j].target != null)
                MoveHead();
        }
    }

    void CalculateHead()
    {
        for (int j = 0; j < monsters.Count; j++)
        {
            for (int i = 0; i < itemsArray.Length; i++)
            {
                if (Vector3.Distance(itemsArray[i].transform.position, monsters[j].member[0].part.transform.position) < 50f && itemsArray[i].gameObject.activeSelf)
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
                        {
                            Creature crea = monsters[j];
                            crea.target = null;
                            monsters[j] = crea;
                        }
                    }
                }
            }
            float minDist = 1000.0f;
            //foreach (var i in items)
            Creature c = monsters[j];
            for(int i = 0; i < items.Count; i ++)
            {
                if (Vector3.Distance(items[i].transform.position, monsters[j].member[0].part.transform.position) < minDist)
                {
                    minDist = Vector3.Distance(items[i].transform.position, monsters[j].member[0].part.transform.position);
                    c.target = items[i].transform;
                }
            }
            if (j == 0)
                destination = c.target;
            monsters[j] = c;
        }
    }

    void MoveHead()
    {
        for (int j = 0; j < monsters.Count; j++)
        {
            bool miam = false;
            brainTimer += Time.deltaTime;
            if (brainTimer > Random.Range(brainTimerMin, brainTimerMax))
            {
                for (int i = 0; i < monstersArray.Length; i++)
                {
                    Vector3 direction = (monsters[j].target.position - monsters[j].member[0].part.transform.position).normalized;
                    monsters[j].member[0].part.GetComponent<Rigidbody>().AddForce(direction, ForceMode.Impulse);
                    brainTimer = 0;
                }
                if (!miam)
                {
                    Creature c = monsters[j];
                    c.fitness ++;
                    monsters[j] = c;
                }
            }
            debugFitness[j] = monsters[j].fitness;

            Camera.main.transform.LookAt(monsters[j].member[0].part.transform);
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
        
        for (int i = 0; i < nbMutation; i ++)
        {
            int posrdm = Random.Range(0, newDna.Length);
            //mutation du gène en fonction de la position
            //Mutation du membre auquel est rataché
            if (posrdm > 0 && (posrdm + 1)% 4 == 0)
            {
                int rdm = Random.Range(0, newDna.Length%4);
                
                if (rdm == 0)
                    newDna[posrdm] = '0';
                else
                    newDna[posrdm] = (char)rdm;
            }
            //Muration de la taille
            else if (posrdm == 0 || (posrdm > 4 && (posrdm + 1) % 4 == 1))
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
            else if (posrdm == 1 || (posrdm > 4 && (posrdm + 1) % 4 == 2))
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
            else if (posrdm == 2 || (posrdm > 4 && (posrdm + 1) % 4 == 3))
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
        string final = new string(newDna);
        return final;
    }

    private List<string> Cross(int idPapa, int idMaman)
    {
        List<string> parent = new List<string>();
        string papa = codes[idPapa];
        string maman = codes[idMaman];

        int lenghtPapa = papa.Length;
        int lenghtMaman = maman.Length;

        if (lenghtPapa < lenghtMaman)
        {
            int rdm = Random.Range(0, lenghtPapa);

            string temp = papa.Substring(rdm);
            string temp2 = maman.Substring(rdm);

            papa = papa.Remove(rdm);
            maman = maman.Remove(rdm);

            papa += temp2;
            maman += temp;

            parent.Add(papa);
            parent.Add(maman);
        }
        else
        {
            int rdm = Random.Range(0, lenghtMaman);

            string temp = papa.Substring(rdm);
            string temp2 = maman.Substring(rdm);

            papa = papa.Remove(rdm);
            maman = maman.Remove(rdm);

            papa += temp2;
            maman += temp;

            parent.Add(papa);
            parent.Add(maman);
        }

        return parent;
    }

    public void newGeneration()
    {
        List<string> newGen = new List<string>();
        List<int> ids = new List<int>();
        ids.Add(0); ids.Add(0); ids.Add(0); ids.Add(0);

        //faire condition pour prendre les 4 meilleurs et faire deux mélanges avec
        for (int i = 0; i < monsters.Count; i ++)
        {
            int val = 0;
            for(int j = 0; j < monsters.Count; j++)
            {
                if (monsters[i].fitness > monsters[j].fitness && i != j)
                {
                    val++;
                }
            }
            if(val<3)
                ids[val] = i;
        }


        newGen.AddRange(Cross(0, 1));
        newGen.AddRange(Cross(2, 3));

        /*
        newGen.AddRange(Cross(ids[0], ids[2]));
        newGen.AddRange(Cross(ids[1], ids[3]));*/

        //mutation des enfants
        Mutate(newGen[0], numberMutation);
        Mutate(newGen[1], numberMutation);
        Mutate(newGen[2], numberMutation);
        Mutate(newGen[3], numberMutation);

        //ajouter les 4 parents dans codes
        newGen.Add(monsters[0].code);
        newGen.Add(monsters[1].code);
        newGen.Add(monsters[2].code);
        newGen.Add(monsters[3].code);

        //mise a jour des individus
        codes = newGen;
    }


}
