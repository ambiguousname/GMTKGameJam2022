using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DiceDragAndDrop : MonoBehaviour, IDragHandler, IEndDragHandler
{
    //Dice Properties
    public int minRange;
    public int maxRange;
    public string attribute;

    //Drag and Drop Stuff
    [SerializeField]
    private float dampingSpeed = .05f;

    private RectTransform dragTransform;
    private Vector2 startingPos;
    private Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        dragTransform = transform as RectTransform;
        startingPos = dragTransform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(dragTransform, eventData.
            position, eventData.pressEventCamera, out var globalMousePosition))
        {
            dragTransform.position = Vector3.SmoothDamp(dragTransform.position,
                globalMousePosition, ref velocity, dampingSpeed);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragTransform.position.y >= 100)
        {
            int dieResult = Random.Range(minRange, maxRange);
            print("You rolled a " + dieResult + "!");
            FindObjectOfType<RollerManager>().EndRolling(dieResult, attribute);
            dragTransform.position = startingPos;
        }
    }
}
