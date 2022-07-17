using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoDisplay : MonoBehaviour
{
    public Text ammoText;
    public PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player.ammo == 0)
        {
            ammoText.text = "RELOADING...";
        }
        else
        {
            ammoText.text = "AMMO: " + player.ammo;
        }
    }
}
