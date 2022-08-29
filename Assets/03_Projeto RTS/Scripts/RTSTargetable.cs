using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RTSTargetable : NetworkBehaviour
{
    [SerializeField] private Transform aimAtPoint = null;

    public Transform GetAimAtPoint()
    {
        return aimAtPoint;
    }
}
