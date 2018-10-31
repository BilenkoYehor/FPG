using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Touchpad : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [Header("Sensitivities")]
    public float sensitivity = 20f;
    public float scopeSensitivity = 6f;

    bool isPressed;
    float horizontalValue;
    float vertivalValue;

    Player player;

    void Start()
    {
        if (!GetComponent<Image>().raycastTarget)
            Debug.LogError("Touchpad: UI gameObject raycast value has to be true.");

        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    public float HorizontalValue
    {
        get
        {
            if (previousHorizontalValue == horizontalValue)
            {
                return 0;
            }
            else
            {
                previousHorizontalValue = horizontalValue;

                return horizontalValue;
            }
        }
    }

    public float VertivalValue
    {
        get
        {
            if (previousVerticalValue == vertivalValue)
            {
                return 0;
            }
            else
            {
                previousVerticalValue = vertivalValue;

                return vertivalValue;
            }
        }
    }

    public bool IsPressed
    {
        get { return isPressed; }
    }

    // UI methods

    float previousHorizontalValue;
    float previousVerticalValue;

    public void OnDrag(PointerEventData eventData)
    {
        if (player.IsAiming)
        {
            horizontalValue = eventData.delta.x * 0.0061f * scopeSensitivity;
            vertivalValue = eventData.delta.y * 0.0061f * scopeSensitivity;
        }
        else
        {
            horizontalValue = eventData.delta.x * 0.0061f * sensitivity;
            vertivalValue = eventData.delta.y * 0.0061f * sensitivity;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;

        horizontalValue = 0;
        vertivalValue = 0;
    }

}