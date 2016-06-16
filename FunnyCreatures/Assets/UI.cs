using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI : MonoBehaviour {

    [SerializeField]
    Text mutation;
    [SerializeField]
    GameObject[] pannelsWitness;
    [SerializeField]
    Earth terre;


    // Use this for initialization
    void Start () {
        mutation.text = terre.numberMutation.ToString();
	}
	
	public void UpdateNumberMutation ()
    {
        mutation.text = terre.numberMutation.ToString();
    }
}
