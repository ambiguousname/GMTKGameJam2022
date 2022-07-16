using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class YarnCommands : MonoBehaviour
{
    [YarnCommand]
    void GiveDie(string name) {
        FindObjectOfType<RollerManager>().AddDie(Resources.Load<Dice>(name));
    }
}
