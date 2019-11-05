using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using UnityEngine.EventSystems;
using Helpers;
using UnityEngine.XR.ARCore;


public class SceneController_Part_2 : SceneController_Part_1
{
    private GameObject m_lineHandle;

    [SerializeField]
    public GameObject ShadowPrefab;

    
    [SerializeField]
    public GameObject cubeHandle;


    public float MaximumScrolls;

    private LineRenderer m_lineHandleRenderer;
    private List<ARRaycastHit> m_hits_handle = new List<ARRaycastHit>();

    private Transform currentPos;

    private int SEGMENT_COUNT = 50;
    
    private float currentScroll;
    
    public float smoothTime = 0.5F;
    private Vector3 velocity = Vector3.zero;
    private Vector3 velocity2 = Vector3.zero;

    private ARPlaneManager m_ARPlaneManager;
    private GameObject shadow;

    Camera camera1;
    Vector3 newPos;

    Vector3 cameraPos;

    public bool isStart = true;


    public override void startInternal()
    {
        currentScroll = MaximumScrolls;
        spawnedObjects = new Stack();
        distances = new Stack<GameObject>();

        shadow = Instantiate(ShadowPrefab, Vector3.zero, Quaternion.identity);

        camera1 = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        newPos = cubeHandle.transform.position;

        MyTrace.trace(new List<string> { "Start" }, true);
        MyTrace.tracePoint("newPos", newPos);
    }

    public override void awakeInternal()
    {
        base.awakeInternal();
        m_ARPlaneManager = ARSessionOrigin.GetComponent<ARPlaneManager>();
        m_lineHandle = Instantiate(DistanceVisualizerPrefab, Vector3.zero, Quaternion.identity);
        m_lineHandleRenderer = m_lineHandle.GetComponent<LineRenderer>();
        m_lineHandleRenderer.startWidth = 0.005f;
        m_lineHandleRenderer.endWidth = 0.01f;
        m_lineHandleRenderer.positionCount = 0;
        m_lineHandleRenderer.sortingOrder = 1;
        m_lineHandleRenderer.material = new Material(Shader.Find("Sprites/Default"));
        m_lineHandleRenderer.material.color = Color.yellow;

        camera1 = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        newPos = cubeHandle.transform.position;
        MyTrace.trace(new List<string> { "Awake" }, true);
        MyTrace.tracePoint("newPos", newPos);

        cameraPos = camera1.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, camera1.nearClipPlane));
        
    }

    public override void updateInternal()
    {
        MyTrace.trace(new List<string> { "Update" }, true);
        
        raycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), m_hits_handle, TrackableType.Planes);

        Vector3 newCameraPos = camera1.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, camera1.nearClipPlane));

        //cameraPos = Vector3.SmoothDamp(cameraPos, newCameraPos, ref velocity, smoothTime);

        cameraPos = newCameraPos;
        cameraPos = newCameraPos - camera1.transform.up * 0.1f;
        //cameraPos = newCameraPos + Vector3.back * 0.1f;

        float newShadowYpos = shadow.transform.position.y;

        if (m_hits_handle.Count > 0)
        {

            newShadowYpos = m_hits_handle[0].pose.position.y + 0.001f;
            newPos = ((currentScroll / MaximumScrolls) * m_hits_handle[0].pose.position) + ((1.0f - (currentScroll / MaximumScrolls)) * cameraPos);
        }

        Vector3 newShadowPos = new Vector3(newPos.x, newShadowYpos, newPos.z);

        cubeHandle.transform.position = Vector3.SmoothDamp(cubeHandle.transform.position, newPos, ref velocity, smoothTime);
        shadow.transform.position = Vector3.SmoothDamp(shadow.transform.position, newShadowPos, ref velocity2, smoothTime);

        Vector3 midpoint = cameraPos + ((cubeHandle.transform.position - cameraPos) / 2.0f) + (velocity * 0.1f);
        
        drawCurve(cameraPos, midpoint, cubeHandle.transform.position);

        currentPos = cubeHandle.transform;

    }

    public void place()
    {
        tapAction(currentPos.position, currentPos.rotation);
        MyTrace.trace(new List<string>{ "place function executed"}, true);
    }

    void drawCurve(Vector3 point1, Vector3 point2, Vector3 point3) 
    {
        m_lineHandleRenderer.positionCount = 0;
        
        for (int i = 1; i <= SEGMENT_COUNT; i++)
        {
            float t = i / (float)SEGMENT_COUNT;
            Vector3 pixel = CalculateQuadBezierPoint(t, point1, point2, point3);

            m_lineHandleRenderer.positionCount = i;
            m_lineHandleRenderer.SetPosition((i - 1), pixel);
        }
    }

    Vector3 CalculateQuadBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1.0f - t;
        float uu = u * u;
        float uuu = uu * u;

        //Vector3 p = (1 - t) * p0 + t * p1;
        Vector3 p = uu * uu * p0 + 2.0f * uu * t * p1 + t * t * p2;
        return p;
    }

    Vector3 CalculateLinearBezierPoint(float t, Vector3 p0, Vector3 p1)
    {
        float u = 1 - t;
        
        Vector3 p = (1 - t) * p0 + t * p1;
        return p;
    }

    public void updateScroll(float currentVal)
    {
        currentScroll = currentVal % MaximumScrolls;
    }
}
