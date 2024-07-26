using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField]
    private Vector3 _rotate;

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null && rb.isKinematic)
        {
            StartCoroutine(RotateObject(rb));
        }
        else
        {
            Debug.Log("Error: no rigid body or the object is not kinematic " + gameObject.name);
        }
    }

    private IEnumerator RotateObject(Rigidbody rb)
    {
        while (true)
        {
            rb.MoveRotation(rb.rotation * Quaternion.Euler(_rotate * Time.deltaTime));
            yield return null; 
        }
    }
}
