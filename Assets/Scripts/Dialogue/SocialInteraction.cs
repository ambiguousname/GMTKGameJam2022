using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class SocialInteraction : MonoBehaviour
{
    public string yarnNodeToStart = "Start";
    public bool triggersAutomatically = false;
    private bool _wasTriggered = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && _wasTriggered == false) {
            if (triggersAutomatically)
            {
                Trigger();
            }
            else {
                FindObjectOfType<FireToInteract>().Enable(Trigger);
            }
        }
    }

    void Trigger() {
        _wasTriggered = true;
        FindObjectOfType<DialogueRunner>().StartDialogue(yarnNodeToStart);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player") {
            FindObjectOfType<FireToInteract>().Disable();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
