using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RollerManager : MonoBehaviour
{
    public GameObject diePrefab;

    public List<DiceDragAndDrop> inventory;

    private LayoutGroup _rollBox;

    private Action<int, string> _endCallback;
    // Start is called before the first frame update
    void Start()
    {
        inventory = new List<DiceDragAndDrop>();
        _rollBox = this.gameObject.FindChildWithName("DieRollBox").GetComponent<LayoutGroup>();
    }

    public void AddDie(DiceDragAndDrop dice) {
        inventory.Add(dice);
    }

    public void RenderDice() {
        foreach (DiceDragAndDrop dice in inventory) {
            var newDie = Instantiate(diePrefab);
            newDie.AddComponent<DiceDragAndDrop>();
            newDie.transform.parent = _rollBox.transform.parent;
        }
    }

    public void HideDice() {
        for (int i = 0; i < _rollBox.transform.childCount; i++) {
            Destroy(_rollBox.transform.GetChild(i).gameObject);
        }
    }

    public void EnableRolling(Action<int, string> callback) {
        transform.GetChild(0).gameObject.SetActive(true);
        _endCallback = callback;
    }

    public void EndRolling(int outcome, string attribute)
    {
        transform.GetChild(0).gameObject.SetActive(false);
        _endCallback(outcome, attribute);
    }
}
