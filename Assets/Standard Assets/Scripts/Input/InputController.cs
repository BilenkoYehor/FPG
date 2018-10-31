using UnityEngine;

public class InputController : MonoBehaviour
{

    [Header("- Layers -")]
    public LayerMask shootingLayers;
    public Camera cam;

    protected float movementVertical;
    protected float movementHorizontal;

    protected float rotationVertical;
    protected float rotationHorizontal;

    protected bool isFirePressed;
    protected bool isReloadPressed;
    protected bool isScopePressed;
    protected bool isJumpPressed;

    protected bool isWeaponChanged;

    protected bool isRunning;

    protected Vector3 hitPos;
    protected string hitObjectTag;

    // Update is called once per frame
    void Update()
    {
        UpdateVariables();
    }

    public virtual void UpdateVariables()
    {
        if (cam.enabled)
        {
            // Hit object Position, hit object Name, hit object Tag, hit GameObject
            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, shootingLayers))
            {
                hitPos = hit.point;
                hitObjectTag = hit.collider.tag;
            }
            else
            {
                hitPos = Vector3.zero;
                hitObjectTag = "";
            }
        }
    }

    public float MovementVertical
    {
        get { return movementVertical; }
    }

    public float MovementHorizontal
    {
        get { return movementHorizontal; }
    }

    public float RotationVertical
    {
        get { return rotationVertical; }
    }

    public float RotationHorizontal
    {
        get { return rotationHorizontal; }
    }

    public bool IsFirePressed
    {
        get { return isFirePressed; }
    }

    public bool IsWeaponChanged
    {
        get 
        {
            if(isWeaponChanged)
            {
                isWeaponChanged = false;
                return true;
            }

            return false;
        }
    }

    public bool IsReloadPressed
    {
        get
        {
            if (isReloadPressed)
            {
                isReloadPressed = false;
                return true;
            }

            return false;
        }
    }

    public bool IsScopePressed
    {
        get
        {
            if (isScopePressed)
            {
                isScopePressed = false;
                return true;
            }

            return false;
        }
    }

    public bool IsJumpPressed
    {
        get
        {
            if (isJumpPressed)
            {
                isJumpPressed = false;
                return true;
            }

            return false;
        }
    }

    public bool IsRunning
    {
        get { return isRunning; }
    }

    public Vector3 HitPos
    {
        get { return hitPos; }
    }

    public string HitObjectTag
    {
        get { return hitObjectTag; }
    }
}

