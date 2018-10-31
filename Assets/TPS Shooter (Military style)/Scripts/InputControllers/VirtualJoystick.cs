using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TPSShooter
{

    /// <summary>
    /// Simple joystick.
    /// </summary>

    public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {

        [Header("Joystick images")]
        public Image ImgBackground;
        public Image ImgJoystick;

        Vector3 inputVector;

        float width;
        float height;

        float minWidth;
        float minHeight;

        float minRunWidth;
        float minRunHeight;

        void Start()
        {
            if (!ImgBackground.raycastTarget)
                Debug.LogError("VirtualJoystick (ImgBackground): UI gameObject raycastTarger value has to be true.");
            if (!ImgJoystick.raycastTarget)
                Debug.LogError("VirtualJoystick (ImgJoystick): UI gameObject raycastTarget value has to be true.");

            width = ImgBackground.rectTransform.sizeDelta.x;
            height = ImgBackground.rectTransform.sizeDelta.y;

            minWidth = width / 8;
            minHeight = height / 8;

            minRunWidth = width / 2 + width / 4;
            minRunHeight = height + height / 6;
        }

        public void OnPointerDown(PointerEventData e)
        {
            OnDrag(e);
        }

        float GetNormalizedValue(float a)
        {
            if (a > 0)
                return 1;
            return -1;
        }

        float x;
        float y;

        public void OnDrag(PointerEventData e)
        {
            Vector2 pos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(ImgBackground.rectTransform,
                                                                        e.position,
                                                                        e.pressEventCamera,
                                                                        out pos))
            {
                // transform to local position;
                x = pos.x + width / 2;
                y = pos.y - height / 2;

                if (y > minRunHeight)
                {
                    isRun = true;

                    if (Mathf.Abs(x) < minRunWidth / 2)
                    {
                        y = 1;

                        x = x / (minRunWidth / 2);
                    }
                    else
                    {
                        y = (minRunWidth - Mathf.Abs(x)) / (minRunWidth / 2);

                        if (y < 0)
                            y = 0;

                        x = GetNormalizedValue(x);
                    }
                }
                else
                {
                    isRun = false;

                    if (Mathf.Abs(x) > minWidth && Mathf.Abs(y) > minHeight)
                    {
                        if (Mathf.Abs(y) > Mathf.Abs(x))
                        {
                            x = x / Mathf.Abs(y);
                            y = GetNormalizedValue(y);
                        }
                        else
                        {
                            y = y / Mathf.Abs(x);
                            x = GetNormalizedValue(x);
                        }
                    }
                    else if (Mathf.Abs(x) > minWidth && Mathf.Abs(y) < minHeight)
                    {
                        x = GetNormalizedValue(x);
                        y = 0;
                    }
                    else
                    {
                        y = GetNormalizedValue(y);
                        x = 0;
                    }
                }

                pos.x = (pos.x / ImgBackground.rectTransform.sizeDelta.x);
                pos.y = (pos.y / ImgBackground.rectTransform.sizeDelta.y);

                inputVector = new Vector3(pos.x * 2 + 1, 0, pos.y * 2 - 1);
                inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

                ImgJoystick.rectTransform.anchoredPosition = new Vector3(inputVector.x * (ImgBackground.rectTransform.sizeDelta.x * .4f),
                                                                         inputVector.z * (ImgBackground.rectTransform.sizeDelta.y * .4f));

            }
        }

        public void OnPointerUp(PointerEventData e)
        {
            inputVector = Vector3.zero;
            ImgJoystick.rectTransform.anchoredPosition = Vector3.zero;

            x = 0;
            y = 0;

            isRun = false;
        }

        bool isRun;

        public float HorizontalValue()
        {
            return x;
        }

        public float VerticalValue()
        {
            return y;
        }

        public bool IsRun()
        {
            return isRun;
        }

    }
}