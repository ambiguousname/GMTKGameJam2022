using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dice", menuName = "ScriptableObjects/Dice")]
public class Dice : ScriptableObject
{
    public Sprite attachedSprite;
    public List<int> faces;
    public string attribute;

    public int Roll() {
        int result = Random.Range(0, faces.Count);
        return faces[result];
    }
}
