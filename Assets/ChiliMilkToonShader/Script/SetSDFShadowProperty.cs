using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SetSDFShadowProperty : MonoBehaviour
{
    [SerializeField] Material material;
    [SerializeField] Vector3 forward;
    [SerializeField] Vector3 left;
 
    private void LateUpdate()
    {
        if (material)
        {
            material.SetVector("_ForwardDirWS", transform.TransformDirection(forward));
            material.SetVector("_LeftDirWS", transform.TransformDirection(left));
        }   
    }
}
