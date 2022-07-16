using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RollerManager : MonoBehaviour
{
    public GameObject diePrefab;

    public List<Dice> inventory = new List<Dice>();
    private HashSet<Dice> _uniqueDice = new HashSet<Dice>();
    private List<Dice> _diceToRoll;

    private GridLayoutGroup _rollBox;

    private Action<int, string> _endCallback;
    // Start is called before the first frame update
    void Start()
    {
        _rollBox = this.gameObject.FindChildWithName("DieRollBox").GetComponent<GridLayoutGroup>();
        foreach (var die in inventory) {
            if (!_uniqueDice.Contains(die)) {
                _uniqueDice.Add(die);
            }
        }
    }

    public void AddDie(Dice dice) {
        inventory.Add(dice);
        if (!_uniqueDice.Contains(dice)) {
            _uniqueDice.Add(dice);
        }
    }

    public void RemoveDie(Dice dice) {
        if (inventory.Contains(dice))
        {
            inventory.Remove(dice);
            if (!inventory.Contains(dice)) {
                _uniqueDice.Remove(dice);
            }
        }
    }

    public void RemoveDie(int index) {
        if (index < inventory.Count) {
            var dieToRemove = inventory[index];
            inventory.Remove(dieToRemove);
            if (!inventory.Contains(dieToRemove)) {
                _uniqueDice.Remove(dieToRemove);
            }
        }
    }

    public void RenderDice() {
        // Easier to tweak manually:
        if (_uniqueDice.Count <= 6)
        {
            _rollBox.constraint = GridLayoutGroup.Constraint.Flexible;
            _rollBox.cellSize = new Vector2(130, 130);
        }
        else if (_uniqueDice.Count <= 9)
        {
            _rollBox.constraint = GridLayoutGroup.Constraint.Flexible;
            _rollBox.cellSize = new Vector2(80, 80);
        }
        else if (_uniqueDice.Count <= 11)
        {
            _rollBox.constraint = GridLayoutGroup.Constraint.Flexible;
            _rollBox.cellSize = new Vector2(60, 60);
        }
        else if (_uniqueDice.Count <= 14)
        {
            _rollBox.constraint = GridLayoutGroup.Constraint.Flexible;
            _rollBox.cellSize = new Vector2(50, 50);
        }
        else if (_uniqueDice.Count <= 28)
        {
            _rollBox.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            _rollBox.constraintCount = 2;
            _rollBox.cellSize = new Vector2(60, 60);
        }
        else {
            Debug.LogError("Uhh, way too many unique dice.");
        }

        foreach (Dice dice in _uniqueDice) {
            var newDie = Instantiate(diePrefab);
            newDie.GetComponent<Image>().sprite = dice.attachedSprite;
            newDie.GetComponent<DiceDragAndDrop>().attachedDie = dice;
            newDie.transform.SetParent(_rollBox.transform, false);

            // Look for duplicates:
            var allDice = inventory.FindAll((d) => {
                return d.Equals(dice);
            });
            for (int i = 0; i < allDice.Count - 1; i++) {
                var subDie = Instantiate(diePrefab);
                subDie.GetComponent<Image>().sprite = dice.attachedSprite;
                subDie.GetComponent<DiceDragAndDrop>().attachedDie = dice;
                subDie.transform.parent = transform;
                subDie.GetComponent<RectTransform>().sizeDelta = new Vector2(_rollBox.cellSize.x, _rollBox.cellSize.y);
                StartCoroutine(SetDieParent(subDie.transform, new Vector3(-(i + 1) * 5, -(i + 1) * 5), newDie.transform));
            }
        }
    }

    IEnumerator SetDieParent(Transform obj, Vector3 offset, Transform parent) {
        yield return new WaitForEndOfFrame();
        obj.position = parent.position + offset;
    }

    public void HideDice() {
        for (int i = 0; i < _rollBox.transform.childCount; i++) {
            Destroy(_rollBox.transform.GetChild(i).gameObject);
        }
    }

    public void EnableRolling(Action<int, string> callback)
    {
        _diceToRoll = new List<Dice>();
        RenderDice();
        transform.GetChild(0).gameObject.SetActive(true);
        _endCallback = callback;
    }

    public void AddDieToRoll(Dice die) {
        _diceToRoll.Add(die);
    }

    public void RemoveDiceFromRoll(Dice die) {
        _diceToRoll.Remove(die);
    }

    public void Roll() {
        var sum = 0;
        foreach (var die in _diceToRoll) {
            sum += die.Roll();
        }
        var dieToPick = UnityEngine.Random.Range(0, _diceToRoll.Count);
        var attr = _diceToRoll[dieToPick];
        EndRolling(sum, attr.attribute);
    }

    public void EndRolling(int outcome, string attribute)
    {
        _diceToRoll.Clear();
        transform.GetChild(0).gameObject.SetActive(false);
        _endCallback(outcome, attribute);
    }
}
