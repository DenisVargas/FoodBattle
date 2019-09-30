using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickCard : MonoBehaviour
{
    public bool mousePressed;
    public GameObject cardPicked;
    public Transform playerBase;
    public Vector3 newPos;

    void Start()
    {
        
    }


    void Update()
    {
    
    }


    public void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawFrustum(Vector3.zero, Camera.main.fieldOfView, Camera.main.farClipPlane,
            Camera.main.nearClipPlane, Camera.main.aspect);
    }
}
