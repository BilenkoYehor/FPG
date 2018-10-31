using UnityEngine;

public class RadarableObject : MonoBehaviour {

    public GameObject radarableImagePrefab;
    GameObject createdImage;

    public Vector3 ObjectPos
    {
        get { return transform.position; }
    }

    public GameObject CreatedImageObj
    {
        get { return createdImage; }
    }

    Radar radar;

    void Start()
    {
        radar = GameObject.FindWithTag("Radar").GetComponent<Radar>();

        InitializeDot();
    }

    void InitializeDot()
    {
        createdImage = Instantiate(radarableImagePrefab);

        radar.AddDot(this);
    }

    public void DestroyDot()
    {
        Destroy(createdImage);
        
        radar.RemoveDot(this);
    }

}