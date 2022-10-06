using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connector : MonoBehaviour
{
    public Vector2 size = Vector2.one * 5f;
    public bool isConnected;

    private bool isPlaying;

    private void Start()
    {
        isPlaying = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isConnected ? Color.green : Color.red;
        if(!isPlaying) 
        {
            Gizmos.color = Color.cyan;
        }
        Vector2 halfSize = size * .5f; //magic nuuuumbers
        Vector3 offset = transform.position + transform.up * halfSize.y;
        Gizmos.DrawLine(offset, offset + transform.forward * 2.5f);
        //define top & side vectors
        Vector3 top = transform.up * size.y;
        Vector3 side = transform.right * halfSize.x;

        //define corner vectors
        Vector3 topRight = transform.position + top + side;
        Vector3 topLeft = transform.position + top - side;
        Vector3 bottomRight = transform.position + side;
        Vector3 bottomLeft = transform.position - side;
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);

        //draw diagonal lines
        Gizmos.color *= .5f;
        Gizmos.DrawLine(topRight, offset);
        Gizmos.DrawLine(topLeft, offset);
        Gizmos.DrawLine(bottomRight, offset);
        Gizmos.DrawLine(bottomLeft, offset);

    }
}
