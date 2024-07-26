using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NewMover : MonoBehaviour
{
    [SerializeField]
    private Vector3 _start;
    [SerializeField]
    private Vector3 _end;
    [SerializeField]
    private float _speed = 1f;
    [SerializeField]
    private float _delay = 1f;

    private Rigidbody rb;
    private Vector3 _globalStart;
    private Vector3 _globalEnd;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null || !rb.isKinematic)
        {
            Debug.Log("Error: no rigid body or the object is not kinematic " + gameObject.name);
            return;
        }

        _globalStart = transform.TransformPoint(_start);
        _globalEnd = transform.TransformPoint(_end);

        StartCoroutine(MoveBetweenPoints());
    }

    private IEnumerator MoveToPosition(Vector3 start, Vector3 end)
    {
        float time = 0f;
        float moveTime = Vector3.Distance(start, end) / _speed; 

        while (true)
        {
            rb.MovePosition(Vector3.Lerp(start, end, time / moveTime)); 

            time += Time.deltaTime;

            if (time >= moveTime)
            {
                rb.MovePosition(end); 
                yield break; 
            }

            yield return new WaitForFixedUpdate(); 
        }
    }

        private IEnumerator MoveBetweenPoints()
    {
        while (true)
        {
            yield return MoveToPosition(_globalStart, _globalEnd);
            yield return new WaitForSeconds(_delay);

            yield return MoveToPosition(_globalEnd, _globalStart);
            yield return new WaitForSeconds(_delay);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Vector3 gizmosStart = transform.TransformPoint(_start);
        Vector3 gizmosEnd = transform.TransformPoint(_end);

        Gizmos.DrawLine(gizmosStart, gizmosEnd);
    }
}

