using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Radar : MonoBehaviour {

    public float minDistance = 15f;
    
    static List<RadarableObject> radarableObjects = new List<RadarableObject>();

    Transform playerTransform;

    float height;

    void Awake()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;

        height = GetComponent<Image>().rectTransform.rect.height / 2;

        radarableObjects = new List<RadarableObject>();
    }

    // Update is called once per frame
    void Update () {
        UpdateDotsPositions();
	}

    void UpdateDotsPositions()
    {
        foreach (RadarableObject obj in radarableObjects)
        {
            Vector3 radarPos = (obj.ObjectPos - playerTransform.position);
            float distToObject = Vector3.Distance(playerTransform.position, obj.ObjectPos);

            if (distToObject > minDistance)
            {
                distToObject = (height - obj.CreatedImageObj.GetComponent<Image>().rectTransform.rect.height/2);
            }
            else
            {
                distToObject *= (height - obj.CreatedImageObj.GetComponent<Image>().rectTransform.rect.height / 2) / minDistance;
            }

            float deltay = Mathf.Atan2(radarPos.x, radarPos.z) * Mathf.Rad2Deg - 270 - playerTransform.eulerAngles.y;
            radarPos.x = distToObject * Mathf.Cos(deltay * Mathf.Deg2Rad) * -1;
            radarPos.y = distToObject * Mathf.Sin(deltay * Mathf.Deg2Rad);

            obj.CreatedImageObj.GetComponent<RectTransform>().localPosition = radarPos;

        }
    }

    public void AddDot(RadarableObject obj)
    {
        radarableObjects.Add(obj);
        obj.CreatedImageObj.transform.SetParent(transform);
    }

    public void RemoveDot(RadarableObject obj)
    {
        radarableObjects.Remove(obj);
    }

}
