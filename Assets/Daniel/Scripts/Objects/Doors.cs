using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    public float rotationAngle;
    public float rotationSpeed;
    public Transform Axis;

    private bool isMoving = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isMoving)
        {
            isMoving = true;
            gameObject.GetComponent<BoxCollider>().enabled = false;
            StartCoroutine(OpenDoor());
        }
    }

    IEnumerator OpenDoor()
    {
        Quaternion initialRotation = Axis.rotation;
        Quaternion targetRotation = initialRotation * Quaternion.Euler(0, rotationAngle, 0);
        yield return RotateDoor(initialRotation, targetRotation);
    }

    public IEnumerator CloseDoor()
    {
        Quaternion initialRotation = Axis.rotation;
        Quaternion targetRotation = initialRotation * Quaternion.Euler(0, -rotationAngle, 0);
        yield return RotateDoor(initialRotation, targetRotation);
    }

    IEnumerator RotateDoor(Quaternion from, Quaternion to)
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * rotationSpeed;
            Axis.rotation = Quaternion.Slerp(from, to, t);
            yield return null;
        }
        isMoving = false;
    }
}
