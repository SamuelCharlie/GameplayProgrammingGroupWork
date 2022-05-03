using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    private MeshRenderer _itemMeshRenderer;
    private Collider _itemCollider;
    
    public GameObject pickupEffect;
    
    public bool canRespawn;
    public float respawnDelay = 20f;
    public int healthRestored = 10;

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
            other.gameObject.GetComponent<PlayerController1>().DamageHP(-healthRestored);
            
            Transform myTransform = transform;
            Instantiate(pickupEffect, myTransform.position, myTransform.rotation);
        
            _itemMeshRenderer.enabled = false;
            _itemCollider.enabled = false;

            if (canRespawn)
            {
                StartCoroutine(Respawn());
            }

            else
            {
                Destroy(gameObject);
            }
        }
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnDelay);
        
        _itemMeshRenderer.enabled = true;
        _itemCollider.enabled = true;
    }
}
