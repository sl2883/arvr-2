using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARTapToPlaceObject : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public GameObject ShadowPrefab;
    public GameObject ARSessionOrigin;

    public float speed = 1.0f;

    public ARRaycastManager raycastManager;
    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();


    private GameObject player;
    private GameObject shadow;
    
    private Vector3 newPos;
    private Quaternion newQuaternion;

   
     // Start is called before the first frame update
    void Start()
    {
        player = Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
        shadow = Instantiate(ShadowPrefab, Vector3.zero, Quaternion.identity);

        Debug.Log("SUNNY player instantiated");

    }

    void Awake()
    {
        raycastManager = ARSessionOrigin.GetComponent<ARRaycastManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Debug.Log("SUNNY touch detected");
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (raycastManager.Raycast(touch.position, s_Hits, TrackableType.PlaneWithinPolygon))
                {
                    // Raycast hits are sorted by distance, so the first one
                    // will be the closest hit.
                    var hitPose = s_Hits[0].pose;

                    Debug.Log("SUNNY plane detected");

                    newPos = hitPose.position;
                    newQuaternion = hitPose.rotation;
                    
                }
            }
        }

        moveObject();
    }

    public void moveObject()
    {

        float step = speed * Time.deltaTime; // calculate distance to move
        player.transform.position = Vector3.MoveTowards(player.transform.position, newPos, step);
        shadow.transform.position = Vector3.MoveTowards(player.transform.position, newPos, step);

        player.transform.LookAt(2 * player.transform.position - newPos);
        // Check if the position of the cube and sphere are approximately equal.
        if (Vector3.Distance(transform.position, newPos) < 0.001f)
        {
            // Swap the position of the cylinder.
            newPos *= -1.0f;
        }
        Debug.Log("SUNNY player position updated");
        //playerRigidbody.AddForce(position * 500); 
        //player.transform.position = position;

        //shadow.transform.position = new Vector3(player.transform.position.x, newPos.y + 0.01f, player.transform.position.z);

    }
}
