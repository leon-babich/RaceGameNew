using UnityEngine;

public class TrafficMoving : MonoBehaviour
{
    private Rigidbody body;
    private float speed = 15f;

    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    //void Update()
    //{
        
    //}

    private void FixedUpdate()
    {
        float t = Time.fixedDeltaTime;
        body.MovePosition(transform.position + Vector3.forward * speed * t);
    }
}
