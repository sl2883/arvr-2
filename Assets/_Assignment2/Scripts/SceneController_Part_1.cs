using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using UnityEngine.EventSystems;

public class SceneController_Part_1 : MonoBehaviour
{
    [SerializeField]
    public GameObject ARSessionOrigin;

    [SerializeField]
    public GameObject PlacedObjectPrefab;

    [SerializeField]
    public GameObject DistanceVisualizerPrefab;

    [SerializeField]
    public GameObject DistanceTextPrefab;

    [SerializeField]
    public Camera ARCamera;

    public Stack spawnedObjects;
    public Stack<GameObject> distances;

    public ARRaycastManager raycastManager;
    private LineRenderer m_lineRenderer;
    private GameObject m_lineVisualizer;

    private List<GameObject> distanceTexts = new List<GameObject>();
    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    void Awake()
    {
        awakeInternal();
    }

    public virtual void awakeInternal()
    {
        m_lineVisualizer = Instantiate(DistanceVisualizerPrefab, Vector3.zero, Quaternion.identity);
        raycastManager = ARSessionOrigin.GetComponent<ARRaycastManager>();
        m_lineRenderer = m_lineVisualizer.GetComponent<LineRenderer>();
        m_lineRenderer.startWidth = 0.02f;
        m_lineRenderer.endWidth = 0.02f;
        m_lineRenderer.positionCount = 0;
        m_lineRenderer.sortingOrder = 1;
        m_lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        m_lineRenderer.material.color = Color.blue;
    }

    // Start is called before the first frame update
    void Start()
    {
        startInternal();
    }

    public virtual void startInternal()
    {
        spawnedObjects = new Stack();
        distances = new Stack<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        updateFaceOfDistanceTexts();

        updateInternal();
                
    }

    public virtual void updateInternal()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if (raycastManager.Raycast(touch.position, s_Hits, TrackableType.PlaneWithinPolygon))
                {
                    // Raycast hits are sorted by distance, so the first one
                    // will be the closest hit.
                    var hitPose = s_Hits[0].pose;

                    tapAction(hitPose.position, hitPose.rotation);
                }
            }
        }
    }

    public void tapAction(Vector3 position, Quaternion rotation)
    {
        GameObject cube = Instantiate(PlacedObjectPrefab, position, rotation);
        spawnedObjects.Push(cube);

        m_lineRenderer.positionCount++;

        if (m_lineRenderer.positionCount > 1)
            StartCoroutine(LineDraw(cube.transform.position, m_lineRenderer.positionCount - 2, m_lineRenderer.positionCount - 1));
        else
            m_lineRenderer.SetPosition(m_lineRenderer.positionCount - 1, cube.transform.position);
    }

    public IEnumerator LineDraw(Vector3 pos2, int pos_n_1, int pos_n_2)
    {
        float t = 0;
        float time = 0.5f;
        Vector3 orig = m_lineRenderer.GetPosition(pos_n_1);
        Vector3 orig2 = pos2;
        m_lineRenderer.SetPosition(pos_n_2, orig);
        Vector3 newpos;
        for (; t < time; t += Time.deltaTime)
        {
            newpos = Vector3.Lerp(orig, orig2, t / time);
            m_lineRenderer.SetPosition(pos_n_2, newpos);
            yield return null;
        }
        m_lineRenderer.SetPosition(pos_n_2, orig2);
        
        drawDistance();
    }

    private void drawDistance()
    {
        if (m_lineRenderer.positionCount > 1)
        {
            Vector3 orig1 = transform.TransformVector(m_lineRenderer.GetPosition(m_lineRenderer.positionCount - 2));
            Vector3 orig2 = transform.TransformVector(m_lineRenderer.GetPosition(m_lineRenderer.positionCount - 1));

            Vector3 midPoint = orig1 + ((orig2 - orig1) / 2.0f);
            string distance = (Vector3.Distance(orig1, orig2) * 100.0f).ToString("#.00") + "cms";

            GameObject textMeshObj = Instantiate(DistanceTextPrefab, midPoint, Quaternion.Euler(0, 180, 0));
            TextMesh textMesh = textMeshObj.GetComponentInChildren<TextMesh>();
            
            textMesh.text = distance;
            textMesh.fontSize = 30;

            distanceTexts.Add(textMeshObj);

        }
    }

    private void updateFaceOfDistanceTexts()
    {
        for (int i = 0; i < distanceTexts.Count; i++)
        {
            if(!distanceTexts[i].transform.position.Equals(ARCamera.transform.position))
                distanceTexts[i].transform.rotation = Quaternion.LookRotation(distanceTexts[i].transform.position - ARCamera.transform.position);
        }
    }

    public void undo()
    {
        m_lineRenderer.positionCount = (m_lineRenderer.positionCount > 0) ? m_lineRenderer.positionCount - 1 : 0;

        if (distanceTexts.Count > 0)
        {
            GameObject textObj = distanceTexts[distanceTexts.Count - 1];
            distanceTexts.Remove(textObj);
            Destroy(textObj);
        }

        if(spawnedObjects.Count > 0)
        {
            GameObject cube = (GameObject)spawnedObjects.Pop();
            Destroy(cube);
        }
    }

    public void reset()
    {
        m_lineRenderer.positionCount = 0;

        int totalDistances = distanceTexts.Count;
        for (int i = totalDistances - 1; i >= 0; i--)
        {
            GameObject textObj = distanceTexts[i];
            distanceTexts.Remove(textObj);
            Destroy(textObj);
        }

        while (spawnedObjects.Count > 0)
        {
            GameObject cube = (GameObject)spawnedObjects.Pop();
            Destroy(cube);
        }
    }
}
