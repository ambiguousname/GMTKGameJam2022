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
    private List<GameObject> _invRender;

    private GridLayoutGroup _rollBox;

    private Action<List<Dice>> _endCallback;
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
        if (_uniqueDice.Count == 0) {
            YarnCommands.SetDie("Basic");
        }
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
            _invRender.Add(newDie);
            newDie.transform.GetChild(1).GetComponent<Image>().sprite = dice.attachedSprite;
            newDie.GetComponent<DiceDragAndDrop>().attachedDie = new Dice(dice);
            newDie.transform.SetParent(_rollBox.transform, false);

            bool _onceEncountered = false;
            int i = 0;
            foreach (var d in inventory) {
                if (d.Equals(dice) && !_onceEncountered)
                {
                    _onceEncountered = true;
                }
                else if (d.Equals(dice) && _onceEncountered) {
                    var subDie = Instantiate(diePrefab);
                    _invRender.Add(subDie);
                    subDie.transform.GetChild(1).GetComponent<Image>().sprite = d.attachedSprite;
                    subDie.GetComponent<DiceDragAndDrop>().attachedDie = new Dice(d);
                    subDie.transform.parent = transform;
                    subDie.GetComponent<RectTransform>().sizeDelta = new Vector2(_rollBox.cellSize.x, _rollBox.cellSize.y);
                    subDie.transform.localScale = Vector3.one;
                    StartCoroutine(SetDieParent(subDie.transform, new Vector3(-(i + 1) * 5, -(i + 1) * 5), newDie.transform));
                    i++;
                }
            }
        }
    }

    IEnumerator SetDieParent(Transform obj, Vector3 offset, Transform parent)
    {
        // This is annoying as hell but it works:
        obj.position = parent.position + offset;
        yield return new WaitForEndOfFrame();
        obj.position = parent.position + offset;
        yield return new WaitForEndOfFrame();
        obj.position = parent.position + offset;
        yield return new WaitForEndOfFrame();
        obj.position = parent.position + offset;
    }

    public void HideDice() {
        for (int i = 0; i < _rollBox.transform.childCount; i++) {
            Destroy(_rollBox.transform.GetChild(i).gameObject);
        }
    }

    public void EnableRolling(Action<List<Dice>> callback, bool showDragBox = true, int size=-1)
    {
        _diceToRoll = new List<Dice>();
        if (size > 0)
        {
            for (int i = 0; i < size; i++) {
                _diceToRoll.Add(new Dice());
            }
        }
        _invRender = new List<GameObject>();
        RenderDice();
        transform.GetChild(0).GetChild(1).gameObject.SetActive(showDragBox);
        transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        transform.GetChild(0).gameObject.SetActive(true);
        _endCallback = callback;
    }

    public void AddDieToRoll(Dice die, int slot=-1) {
        if (slot >= 0)
        {
            _diceToRoll[slot] = die;
        }
        else
        {
            _diceToRoll.Add(die);
        }
    }

    public void RemoveDiceFromRoll(Dice die) {
        _diceToRoll.Remove(die);
    }

    public void RemoveDiceFromRoll(Dice die, int index) {
        _diceToRoll[index] = new Dice();
    }

    private int _diceRollSum = 0;
    private int _targetSum = 0;
    private void DieRollDone() {
        _diceRollSum++;
        if (_diceRollSum >= _targetSum)
        {
            // Remove from the inventory:
            foreach (var dice in _diceToRoll) {
                RemoveDie(dice);
            }
            _endCallback(_diceToRoll);
        }
    }

    public void Roll() {
        if (_diceToRoll.Count > 0)
        {
            _diceRollSum = 0;
            _targetSum = 0;
            var atLeastOne = _diceToRoll.Exists((dice) => {
                return dice.faces != null;
            });
            if (atLeastOne)
            {
                GameObject.Find("RollButton").SetActive(false);
                foreach (var dice in _diceToRoll)
                {
                    if (dice.faces != null)
                    {
                        _targetSum++;
                        Debug.Log(dice.attachedDragAndDrop.name);
                        dice.attachedDragAndDrop.Roll((dice.attribute != null) ? dice.attribute : "", DieRollDone);
                    }
                }
            }
        }
    }

    public void EndRolling()
    {
        foreach (var item in _invRender) {
            Destroy(item);
        }
        _invRender.Clear();
        _diceToRoll.Clear();
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
    }
}
