using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingAnim : MonoBehaviour
{
    public bool shouldKeepRolling = true;
    public void RollFinished() {
        GetComponentInParent<DiceDragAndDrop>().StopRolling();
    }

    public void ContinueRoll() {
        if (shouldKeepRolling) {
            GetComponent<Animator>().Play(GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name,0,0.1f);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
