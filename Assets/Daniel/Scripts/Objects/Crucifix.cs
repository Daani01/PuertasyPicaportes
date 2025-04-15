using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static IUsable;

public class Crucifix : MonoBehaviour, IUsable
{
    public float detectionRadius = 5f;
    public GameObject CrucifixParticle;
    GameObject allRooms;

    void Awake()
    {
        if (allRooms == null)
        {
            allRooms = GameObject.Find("ALLROOMS");
        }
    }

    ItemType IUsable.GetName()
    {
        return ItemType.Crucifix;
    }

    public void GetObjPlayer(Transform position, Transform lookat)
    {
        // Establecer la posición del objeto actual
        gameObject.transform.position = position.position;

        // Configurar la rotación para que mire hacia el eje Z del transform pasado
        Vector3 direction = position.forward; // Dirección del eje Z del transform pasado
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up); // Calcular la rotación
        gameObject.transform.rotation = rotation;

        // Establecer el transform actual como hijo del transform pasado
        gameObject.transform.SetParent(position);

        // Desactivar el objeto
        gameObject.SetActive(false);
    }


    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void DesActivate()
    {
        gameObject.SetActive(false);
    }

    public void Destroy()
    {
        FirstPersonController player = FindObjectOfType<FirstPersonController>();

        if (player != null)
        {
            player.RemoveItem(this);
            DesActivate();
        }
    }

    public void Use()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                GameObject particle = Instantiate(CrucifixParticle, hit.transform.position, Quaternion.identity);
                particle.transform.SetParent(allRooms.transform, true);

                EnemyPool.Instance.ReturnEnemy(hit.gameObject);

                Destroy();
                return;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }


    public bool Energy()
    {
        return false;
    }

    public float getEnergy()
    {
        return 0.0f;
    }

    public float getMaxEnergy()
    {
        return 0.0f;
    }
}
