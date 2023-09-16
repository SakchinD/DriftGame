using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum InputType
{
    Gas,
    Brake,
    Left,
    Right
}

public class Mybutton : MonoBehaviour
{
    [SerializeField] private InputType inputType;
    private bool isPressed;

    public InputType InputType => inputType;
    public bool IsPressed => isPressed; 

    private void Start()
    {
        SetUpButton();
    }

    private void SetUpButton()
    {
        EventTrigger trigger = gameObject.AddComponent<EventTrigger>();
        
        var pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((e) => onClickDown());
        
        var pointerUp = new EventTrigger.Entry();
        pointerUp.eventID = EventTriggerType.PointerUp;
        pointerUp.callback.AddListener((e) => onClickUp());

        trigger.triggers.Add(pointerDown);
        trigger.triggers.Add(pointerUp);
    }

    private void onClickDown()
    {
        isPressed = true;
    }

    private void onClickUp()
    {
        isPressed = false;
    }
}
