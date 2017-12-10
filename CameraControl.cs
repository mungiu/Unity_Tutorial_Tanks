using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float m_DampTime = 0.2f;                 
    public float m_ScreenEdgeBuffer = 4f;           
    public float m_MinSize = 6.5f;      
    //this is how you hide a public variable from the inspector
    [HideInInspector] public Transform[] m_Targets;


    private Camera m_Camera;                        
    private float m_ZoomSpeed;                      
    private Vector3 m_MoveVelocity;                 
    private Vector3 m_DesiredPosition;              


    private void Awake()
    {
        m_Camera = GetComponentInChildren<Camera>();
    }


    private void FixedUpdate()
    {
        Move();
        Zoom();
    }


    private void Move()
    {
        FindAveragePosition();

        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    }


    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;

        for (int i = 0; i < m_Targets.Length; i++)
        {
            //checking if current tank is active (alive)
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            averagePos += m_Targets[i].position;
            numTargets++;
        }

        //calc average position of all active tanks
        if (numTargets > 0)
            averagePos /= numTargets;

        //moving camera on y-axis
        averagePos.y = transform.position.y;

        m_DesiredPosition = averagePos;
    }


    private void Zoom()
    {
        float requiredSize = FindRequiredSize();
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
    }

    //must be found from "desired camera position" not "current camera position"
    private float FindRequiredSize()
    {
        //finding desired position of CameraRig in CameraRig local space
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);

        //camera size
        float size = 0f;

        //finding the largest size the camera could be
        for (int i = 0; i < m_Targets.Length; i++)
        {
            //if target to not active continue to next target
            if (!m_Targets[i].gameObject.activeSelf)
                continue;
            //finding tank in local position of CameraRig
            Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);
            //finding Vector from CameraRig position to the tank
            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

            //comparing current camera size with current desired camera size
            //picking whichever one is bigger
            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.y));
            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.x) / m_Camera.aspect);
        }
        
        //screen edge buffer
        size += m_ScreenEdgeBuffer;

        size = Mathf.Max(size, m_MinSize);

        return size;
    }

    //reseting camera position every round
    public void SetStartPositionAndSize()
    {
        FindAveragePosition();

        transform.position = m_DesiredPosition;

        m_Camera.orthographicSize = FindRequiredSize();
    }
}