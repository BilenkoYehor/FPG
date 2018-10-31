using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

namespace TPSShooter
{

    /// <summary>
    /// Simple touchpad script.
    /// </summary>

    public class Touchpad : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        [Header("Sensitivities")]
        public float sensitivity = 20f;
        public float scopeSensitivity = 6f;

        float currentSensitivity;

        bool isPressed;
        float horizontalValue;
        float vertivalValue;

        void Start()
        {
            if (!GetComponent<Image>().raycastTarget)
                Debug.LogError("Touchpad: UI gameObject raycast value has to be true.");

            if (!GameObject.FindWithTag(Tags.Player))
                Debug.LogError("Touchpad: No Player found in the scene.");

         //   GameObject.FindWithTag(Tags.Player).GetComponent<PlayerBehaviour>().AddScopeNotifier(this);

            sensitivity = SaveLoad.TouchpadSensitivity;
            scopeSensitivity = SaveLoad.TouchpadAimingSensitivity;

            currentSensitivity = sensitivity;
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
            horizontalValue = eventData.delta.x * 0.0061f * currentSensitivity;
            vertivalValue = eventData.delta.y * 0.0061f * currentSensitivity;
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
}