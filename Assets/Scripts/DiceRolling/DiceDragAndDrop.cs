using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
        if (_firstMove == false) {
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

    public void OnEndDrag(PointerEventData eventData)
    {
        SlotGrid containedSlot = null;
        var slots = FindObjectsOfType<SlotGrid>();
        foreach (var slot in slots) {
            if (slot.gameObject.activeInHierarchy && RectTransformUtility.RectangleContainsScreenPoint(slot.GetComponent<RectTransform>(), dragTransform.position)) {
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
                _isBeingRolled = true;
                if (containedSlot != null)
                {
                    FindObjectOfType<RollerManager>().AddDieToRoll(attachedDie, containedSlot.slot);
                }
                else
                {
                    FindObjectOfType<RollerManager>().AddDieToRoll(attachedDie);
                }
            }
        }
        else {
            dragTransform.position = startingPos;
            if (_isBeingRolled) {
                _isBeingRolled = false;
                FindObjectOfType<RollerManager>().RemoveDiceFromRoll(attachedDie);
            }
        }
    }
}
