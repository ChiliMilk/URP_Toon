using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SetSDFShadowProperty : MonoBehaviour
{
    [SerializeField] Material material;
    [SerializeField] Light mainLight;
    [SerializeField] Vector3 forward;
    [SerializeField] Vector3 left;
 
    private void LateUpdate()
    {
        if (material && mainLight)
        {
            Vector3 direction = -mainLight.transform.forward;
            Vector2 directionXZ = new Vector2(direction.x, direction.z);
            Vector3 forwardWS = transform.TransformDirection(forward);
            Vector3 leftWS = transform.TransformDirection(left);
            Vector2 forwardXZ = new Vector2(forwardWS.x, forwardWS.z);
            Vector2 leftXZ = new Vector2(leftWS.x, leftWS.z);
            directionXZ.Normalize();
            forwardXZ.Normalize();
            leftXZ.Normalize();
            material.SetVector("_LdotFL", new Vector2(Vector2.Dot(directionXZ,forwardXZ),Vector2.Dot(directionXZ,leftXZ)));
        }   
    }
}
