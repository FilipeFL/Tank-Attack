using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RTSUnitProjectile : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float forcaDisparo = 10f;

    [SerializeField] private float tempoTrajetoria = 5f;

    [SerializeField] private int damageToDeal = 20;

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        NetworkIdentity networkIdentity;

        if(other.TryGetComponent<NetworkIdentity>(out networkIdentity))
        {
            if(networkIdentity.connectionToClient != connectionToClient)
            {
                RTSHealth health;

                if(other.TryGetComponent<RTSHealth>(out health))
                {
                    health.dealDamage(damageToDeal);
                }

                DestroySelf();
            }
        }
    }

    void Start()
    {
        FindObjectOfType<AudioManager>().Play("Fire");
        rb.velocity = transform.forward * forcaDisparo;
    }

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), tempoTrajetoria);
    }

    [Server]
    public void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
