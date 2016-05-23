using UnityEngine;
using System.Collections;

public class PickUp : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Weapon")
        {
            // Get index of the weapon about to be picked up
            
            //Send it to the inputclass
            GetComponent<InputPlayer>().WeaponSwitch(0);
        }

        if (other.tag == "Flux")
        {

        }
    }
}
