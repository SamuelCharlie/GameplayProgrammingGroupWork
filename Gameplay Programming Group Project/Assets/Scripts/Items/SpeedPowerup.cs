using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPowerup : MonoBehaviour
{
    private MeshRenderer _itemMeshRenderer;
    private Collider _itemCollider;
    
    public GameObject pickupEffect;

    public  float speedBonus = 1.5f;
    public float powerupDuration = 8f;
    public float respawnDelay = 10f;

    private void Awake()
    {
        if (_itemMeshRenderer == null || _itemCollider == null)
        {
            _itemMeshRenderer = GetComponent<MeshRenderer>();
            _itemCollider = GetComponent<Collider>();
        }

        _itemMeshRenderer.enabled = true;
        _itemCollider.enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(Pickup(other));
        }
    }

    IEnumerator Pickup(Collider player)
    {
        StartCoroutine(Respawn());
        
        Transform myTransform = transform;
        Instantiate(pickupEffect, myTransform.position, myTransform.rotation);

        _itemMeshRenderer.enabled = false;
        _itemCollider.enabled = false;

        //player.GetComponent<PlayerManager>().HandleSpeedBoost(true, speedBonus);
        yield return new WaitForSeconds(powerupDuration);
        //player.GetComponent<PlayerManager>().HandleSpeedBoost(false, 1f);
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnDelay);
        
        _itemMeshRenderer.enabled = true;
        _itemCollider.enabled = true;
    }
}
