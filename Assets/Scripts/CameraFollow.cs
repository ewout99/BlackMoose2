using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

    public Transform target;

    [SerializeField]
    private float smooth = 2.0f;
    private float zPos = -10f;

    // Use this for initialization
    void Start()
    {
        //zPos = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            float smoothX = Mathf.Lerp(transform.position.x, target.position.x, Time.deltaTime * smooth);
            float smoothY = Mathf.Lerp(transform.position.y, target.position.y, Time.deltaTime * smooth);
            transform.position = new Vector3(smoothX, smoothY, zPos);
        }
    }
}
