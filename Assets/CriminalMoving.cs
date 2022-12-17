using UnityEngine;
using System;
using System.Collections.Generic;

public class CriminalMoving : MonoBehaviour
{
    static public GameObject explosionPrefab;
    private GameObject lastCollisionObj = null;

    private Rigidbody body;
    private float speed = 30f;

    bool isLeftPos;
    bool isTurnLeft;
    bool isTurnRight;
    bool isDecision;

    float speedTurn = 0.5f;
    float force = 10f;
    float radForce = 3f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Track") {
            GameObject obj = collision.gameObject;

            if (obj == lastCollisionObj) return;

            lastCollisionObj = collision.gameObject;

            ContactPoint contact = collision.contacts[0];
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 position = contact.point;
            Instantiate(explosionPrefab, position, rotation);

            //if (CanvasSlots.IsSound)
            //{
            //    GetComponent<AudioSource>().time = 3.4f;
            //    GetComponent<AudioSource>().Play();
            //}

            float d = 2f;
            Transform car = collision.gameObject.transform;
            float xDist = body.position.x - car.position.x;
            float xShift = xDist > 0 ? -(d - xDist) : (d - Math.Abs(xDist));

            if (xDist < d)
            {
                transform.position = new Vector3(transform.position.x - xShift / 2, transform.position.y, transform.position.z);
                car.position = new Vector3(car.position.x + xShift / 2, car.position.y, car.position.z);

                Vector3 vec = body.transform.rotation.eulerAngles;
                body.transform.rotation = Quaternion.Euler(vec.x, vec.y - xShift * 10, vec.z);
            }

            body.AddExplosionForce(force, position, radForce);
        }
    }

    void Start()
    {
        body = GetComponent<Rigidbody>();
        isLeftPos = transform.position.x < 0 ? true : false;
    }

    void Update()
    {
        criminalThinking();

        if(isTurnLeft && transform.position.x <= -1.7) {
            body.transform.rotation = Quaternion.Euler(0, 0, 0);
            isTurnLeft = false;
            isLeftPos = true;
            isDecision = false;
        }

        if (isTurnRight && transform.position.x >= 1.7) {
            body.transform.rotation = Quaternion.Euler(0, 0, 0);
            isTurnRight = false;
            isLeftPos = false;
            isDecision = false;
        }

        if(transform.position.y > 1.0f || transform.position.y < 0)
        {
            Debug.Log("Happened something with 'y'. Position: " + transform.position.y);
            body.velocity = Vector3.zero;
            transform.position = new Vector3(transform.position.x, 1.0f, transform.position.z);
        }
    }

    private void FixedUpdate()
    {
        float t = Time.fixedDeltaTime;

        if (isTurnLeft) {
            Quaternion turnTo = Quaternion.Euler(0, -45, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, turnTo, speedTurn * t);
        }

        if (isTurnRight) {
            Quaternion turnTo = Quaternion.Euler(0, 45, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, turnTo, speedTurn * t);
        }

        body.MovePosition(transform.position + transform.TransformDirection(Vector3.forward * speed * t));
    }

    void criminalThinking()
    {
        List<GameObject> listCars = new List<GameObject>();

        //foreach (var car in GameController.listTraffic)
        //{
        //    if (car.transform.position.z - transform.position.z < 30)
        //    {
        //    }
        //}


        foreach (var car in GameController.listTraffic)
        {
            float dist = car.transform.position.z - transform.position.z;

            if (dist < 30 && dist > 0 && Math.Abs(car.transform.position.x - transform.position.x) < 0.5f && !isDecision)
            {
                if (isLeftPos) isTurnRight = true;
                else isTurnLeft = true;
                isDecision = true;

                string dir = isTurnLeft ? "left" : "right";
                Debug.Log("Criminal decided to turn: " + dir);
            }
        }
    }
}
