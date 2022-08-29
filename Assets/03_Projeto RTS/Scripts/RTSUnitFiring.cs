using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class RTSUnitFiring : NetworkBehaviour
{
    [SerializeField] private RTSTargeter targeter;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private float fireRange = 5f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float rotationSpeed = 5f;

    private float lastTimeFire = 0;

    [ServerCallback]

    private void Update()
    {
        RTSTargetable target = targeter.GetTarget();

        if (target != null)
        {
            //Debug.Log("Entrei no projetil");
            if (CanFireAtTarget() is true)
            {
                Quaternion targetQuaternion = Quaternion.LookRotation(targeter.GetTarget().transform.position - transform.position);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetQuaternion, rotationSpeed * Time.deltaTime);
                
                //Debug.Log("Entrei no canfire");
                //Debug.Log("Time delta time: " + Time.deltaTime);
                //Debug.Log("Equacao fireRate: " + (1 / fireRate) + lastTimeFire);

                if (Time.time > ((1 / fireRate) + lastTimeFire))
                {
                    Quaternion projectileRotation = Quaternion.LookRotation(targeter.GetTarget().GetAimAtPoint().position - projectileSpawnPoint.position);
                    GameObject projectileInstance = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileRotation);

                    Debug.Log("Entrei no projetil");
                    NetworkServer.Spawn(projectileInstance, connectionToClient);

                    lastTimeFire = Time.time;
                }
            }
        }
    }

    [Server]
    private bool CanFireAtTarget()
    {
        //Debug.Log((targeter.GetTarget().transform.position - transform.position).sqrMagnitude);
        if ((targeter.GetTarget().transform.position - transform.position).sqrMagnitude <= fireRange * fireRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
