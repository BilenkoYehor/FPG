using UnityEngine;

public class WeaponSway : MonoBehaviour, IScopeNotifier {

    public float amount;
    [Header("- Amount -")]
    public float maxAmount;
    public float maxScopeAmount;
    float currentMaxAmount;
    [Header("- Smooth -")]
    public float smoothAmount;
    public float smoothScopeAmount;
    float currentSmoothTime;

    Vector3 initialPosition;

    InputController inputController;

    public void ScopeOff()
    {
        currentSmoothTime = smoothAmount;
        currentMaxAmount = maxAmount;
    }

    public void ScopeOn()
    {
        currentSmoothTime = smoothScopeAmount;
        currentMaxAmount = maxScopeAmount;
    }

    private void Start()
    {
        initialPosition = transform.localPosition;

        inputController = GameObject.FindWithTag("InputController").GetComponent<InputController>();

        currentSmoothTime = smoothAmount;
        currentMaxAmount = maxAmount;
    }


    private void Update()
    {
        float movementX = -inputController.RotationHorizontal * amount;
        float movementY = -inputController.RotationVertical * amount;
        movementX = Mathf.Clamp(movementX, -currentMaxAmount, currentMaxAmount);
        movementY = Mathf.Clamp(movementY, -currentMaxAmount, currentMaxAmount);

        Vector3 finalPosition = new Vector3(movementX, movementY, 0);
        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initialPosition, Time.deltaTime * currentSmoothTime);
    }
}