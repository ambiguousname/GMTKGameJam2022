using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DiceDragAndDrop : MonoBehaviour, IDragHandler, IEndDragHandler
{
    //Dice Properties
    public Dice attachedDie;

    //Drag and Drop Stuff
    [SerializeField]
    private float dampingSpeed = .05f;

    private RectTransform dragTransform;
    private Vector2 startingPos;
    private Vector3 velocity = Vector3.zero;
    private RectTransform _targetTransform;

    private bool _isBeingRolled = false;

    private bool _firstMove = false;

    public bool canBeDragged = true;

    [HideInInspector]
    public int face = -1;

    private int _index = -1;

    private void Awake()
    {
        dragTransform = transform as RectTransform;
    }

    private void Start()
    {
        if (GameObject.Find("Drag Here Box"))
        {
            _targetTransform = GameObject.Find("Drag Here Box").GetComponent<RectTransform>();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canBeDragged)
        {
            if (_firstMove == false)
            {
                startingPos = dragTransform.position;
                _firstMove = true;
            }
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(dragTransform, eventData.
                position, eventData.pressEventCamera, out var globalMousePosition))
            {
                dragTransform.position = Vector3.SmoothDamp(dragTransform.position,
                    globalMousePosition, ref velocity, dampingSpeed);
            }
        }
    }

    private Action _callback;

    public void Roll(string attribute, Action callback) {
        this.transform.GetChild(2).GetComponent<Animator>().SetTrigger((attribute != "") ? attribute : "Basic");
        this.transform.GetChild(2).gameObject.SetActive(true);
        this.transform.GetChild(1).gameObject.SetActive(false);
        this.transform.GetChild(2).GetComponent<RollingAnim>().shouldKeepRolling = false;
        this.transform.GetChild(2).GetComponent<Animator>().Play(attribute + "Roll");
        _callback = callback;
    }

    public void StopRolling() {
        this.transform.GetChild(2).gameObject.SetActive(false);
        this.transform.GetChild(1).gameObject.SetActive(true);
        this.transform.GetChild(0).gameObject.SetActive(true);
        this.transform.GetChild(0).GetComponent<Animator>().Play("LandVFX");
        face = attachedDie.Roll();
        this.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Dice Assets/" + (attachedDie.attribute != "" ? attachedDie.attribute : "white") + "_Die_" + face);
    }

    public void LandEnd() {
        this.transform.GetChild(0).gameObject.SetActive(false);
        _callback();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canBeDragged)
        {
            SlotGrid containedSlot = null;
            var slots = FindObjectsOfType<SlotGrid>();
            foreach (var slot in slots)
            {
                if (slot.gameObject.activeInHierarchy && RectTransformUtility.RectangleContainsScreenPoint(slot.GetComponent<RectTransform>(), dragTransform.position))
                {
                    containedSlot = slot;
                    break;
                }
            }

            if (containedSlot != null || (_targetTransform != null && RectTransformUtility.RectangleContainsScreenPoint(_targetTransform, dragTransform.position)))
            {
                if (containedSlot != null)
                {
                    dragTransform.position = containedSlot.transform.position;
                }
                if (!_isBeingRolled)
                {
                    attachedDie.attachedDragAndDrop = this;
                    _isBeingRolled = true;
                    if (containedSlot != null)
                    {
                        if (_index != -1)
                        {
                            FindObjectOfType<RollerManager>().RemoveDiceFromRoll(attachedDie, _index);
                        }
                        FindObjectOfType<RollerManager>().AddDieToRoll(attachedDie, containedSlot.slot);
                        _index = containedSlot.slot;
                    }
                    else
                    {
                        FindObjectOfType<RollerManager>().AddDieToRoll(attachedDie);
                    }
                }
            }
            else
            {
                _index = -1;
                dragTransform.position = startingPos;
                if (_isBeingRolled)
                {
                    _isBeingRolled = false;
                    FindObjectOfType<RollerManager>().RemoveDiceFromRoll(attachedDie);
                }
            }
        }
    }
}
