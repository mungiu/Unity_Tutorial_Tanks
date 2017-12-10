using UnityEngine;

public class UIDirectionControl : MonoBehaviour
{
    public bool m_UseRelativeRotation = true;  
    private Quaternion m_RelativeRotation;     

    private void Start()
    {
        //finding local rotation of canvas
        m_RelativeRotation = transform.parent.localRotation;
    }

    private void Update()
    {
        if (m_UseRelativeRotation)
            //setting health slider rotation to local rotation (canvas rotation) every update
            transform.rotation = m_RelativeRotation;
    }
}
