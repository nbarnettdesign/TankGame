using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Created By Liam Gates

    public FocusPoint FocusPoint;
    public List<TankController> players;

    // Update Speeds for depth, angle and position
    public float depthUpdateSpeed = 10f;
    public float angleUpdateSpeed = 50f;
    public float positionUpdateSpeed = 100f;

    // Zoom in/out, maxes
    public float depthMax = 10f;
    public float depthMin = 22f;

    //public float Zoom = 60.0f;

    // angles
    //public float angleMax = 11f;
    //public float angleMin = 3f;

    //camera position
    //private float cameraEulerX;
    //private Vector3 CameraPosition;

    //public List<GameObject> camerTrackedObjects = new List<GameObject>();

    private Camera cameraComponent;


    enum CamerDirection
    {
        Forward,
        Backward,
        None,
    };
    CamerDirection lastDirectionMoved = CamerDirection.None;
    float directionResetTimer = 0.0f;
    public float  DirectionResetTime = 0.5f;

    void Start()
    {
        //adds FocusePoint to the player list so it's position is taken into acount when calculations are done 
        //players.Add(FocusPoint.gameObject);
        //players = (MatchManager._pInstance._AliveTanks);

        cameraComponent = transform.GetChild(0).GetComponent<Camera>();
    }

    void Update()
    {
        players = (MatchManager._pInstance._AliveTanks);
    }

    void LateUpdate()
    {
        //CalculateCameraLocations();
        MoveCamera();
    }

    public bool IsVisibleFrom(Renderer renderer, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        Bounds b = renderer.bounds;
        b.Expand(-20.0f);
        return GeometryUtility.TestPlanesAABB(planes, b);
    }


    private void MoveCamera()
    {
        if(lastDirectionMoved != CamerDirection.None)
        {
            directionResetTimer += Time.deltaTime;
            if(directionResetTimer > DirectionResetTime)
            {
                lastDirectionMoved = CamerDirection.None;
                directionResetTimer = 0.0f;
            }
        }

        //Vector3 position = gameObject.transform.position;
        //if ((position - CameraPosition).sqrMagnitude > 0.01f)
        //{
        //    Vector3 TargetPosition = Vector3.zero;
        //    TargetPosition.x = Mathf.MoveTowards(position.x, CameraPosition.x, positionUpdateSpeed * Time.deltaTime);
        //    TargetPosition.y = Mathf.MoveTowards(position.y, CameraPosition.y, positionUpdateSpeed * Time.deltaTime);
        //    TargetPosition.z = Mathf.MoveTowards(position.z, CameraPosition.z, depthUpdateSpeed * Time.deltaTime);
        //
        //    //if (TargetPosition.y < 50)
        //    //{
        //    //    TargetPosition.y = 50.0f;
        //    //}
        //
        //    gameObject.transform.position = TargetPosition;
        //}

        bool canSeeAllPlayers = true;
        Vector3 midPoint = new Vector3(0, 0, 0);


        Bounds b = new Bounds();
        foreach(TankController go in players)
        {
            Renderer renderer = go.GetComponent<Renderer>();
            if (renderer)
            {
                canSeeAllPlayers = canSeeAllPlayers && IsVisibleFrom(renderer, cameraComponent);
                b.Encapsulate(renderer.bounds);
            }

            midPoint += go.transform.position;
        }
        midPoint /= players.Count;
       //Debug.Log(canSeeAllPlayers);


        cameraComponent.transform.LookAt(midPoint);
        Vector3 offset = FocusPoint.FocusBounds.center - transform.position;
        if (!canSeeAllPlayers)
        {
            if(offset.magnitude < depthMax && lastDirectionMoved != CamerDirection.Forward)
            {
                transform.Translate(-offset.normalized * depthUpdateSpeed * Time.deltaTime);
                lastDirectionMoved = CamerDirection.Backward;
            }
        }
        else
        {
            if(offset.magnitude > depthMin && lastDirectionMoved != CamerDirection.Backward)
            {
                transform.Translate(offset.normalized * depthUpdateSpeed * Time.deltaTime);
                lastDirectionMoved = CamerDirection.Forward;
            }
        }

        //Vector3 localEulerAngles = gameObject.transform.localEulerAngles;
        //if (localEulerAngles.x != cameraEulerX)
        //{
        //    Vector3 targetEulerAngles = new Vector3(cameraEulerX, localEulerAngles.x, localEulerAngles.z);
        //    //gameObject.transform.localEulerAngles = Vector3.MoveTowards(localEulerAngles, targetEulerAngles, angleUpdateSpeed);
        //}
    }

    //private void CalculateCameraLocations()
    //{
    //    Vector3 averageCenter = Vector3.zero;
    //    Vector3 totalPositons = Vector3.zero;
    //    Bounds playerBounds = new Bounds();

    //    for (int i = 0; i < players.Count; i++)
    //    {
    //        Vector3 playerPosition = players[i].transform.position;

    //        if(!FocusPoint.FocusBounds.Contains(playerPosition))
    //        {
    //            float PlayerX = Mathf.Clamp(playerPosition.x, FocusPoint.FocusBounds.min.x, FocusPoint.FocusBounds.max.x);
    //            float PlayerY = Mathf.Clamp(playerPosition.y, FocusPoint.FocusBounds.min.y, FocusPoint.FocusBounds.max.y);
    //            //float PlayerY = Mathf.Clamp(playerPosition.y, Zoom, FocusPoint.FocusBounds.max.y);
    //            float PlayerZ = Mathf.Clamp(playerPosition.z, FocusPoint.FocusBounds.min.z, FocusPoint.FocusBounds.max.z);
    //            playerPosition = new Vector3(PlayerX, PlayerY, PlayerZ);
    //        }

    //        totalPositons += playerPosition;
    //        playerBounds.Encapsulate(playerPosition);
    //    }

    //    averageCenter = (totalPositons / (float)players.Count);

    //    float extens = (playerBounds.extents.x + playerBounds.extents.y);
    //    float lerpPercent = Mathf.InverseLerp(0, (FocusPoint.HalfX + FocusPoint.HalfY) / 2.0f, extens);

    //    float depth = Mathf.Lerp(depthMax, depthMin, lerpPercent);
    //    float angle = Mathf.Lerp(angleMax, angleMin, lerpPercent);

    //    cameraEulerX = angle;
    //    CameraPosition = new Vector3(averageCenter.x, transform.forward.y * depth, transform.forward.z * depth);
    //    //CameraPosition = new Vector3(averageCenter.x, averageCenter.y, transform.forward.z * depth);
    //}
}
