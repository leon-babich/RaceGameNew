using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

//[ExecuteAlways]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody body;
    private Transform m_transform;
    public GameObject explosionPrefab;
    public Transform track;
    private List<Transform> pathElementsShow = new List<Transform>();
    private Transform targetPoint;

    public AudioClip soundMove;
    private AudioSource audioMove;

    public Slider progressBar;

    public RoadArchitect.Road architectRoad;

    private List<TimerCollision> listTimerCollisions = new List<TimerCollision>();
    private ControlerTrack trackController = new ControlerTrack();
    private Turner turner = new Turner();
    private SimpleTimer timer = new SimpleTimer();
    private MovingProccessor movingProccessor;

    static public float speed = 10f;
    private float speedMax = 50f;
    private float speedStart = 15f;
    public float speedIncrease = 2.0f;
    public float speedDecrease = 4.0f;
    private float speedTurnMax = 2f;
    private float speedTurn = 2f;
    public float leftPos = -2f;
    public float rightPos = 2f;
    public float force = 10f;
    public float radForce = 3f;
    public float timerCollision;

    static public bool IsClickTurnLeft { get; set; }
    static public bool IsClickTurnRight { get; set; }
    static public bool IsClickAccelerator { get; set; }
    static public bool IsClickBrekes { get; set; }

    bool isTurnLeft;
    bool isTurnRight;
    public enum TypeProcess { TestMoving, TestMarkers, TestGame };
    public TypeProcess typeProcess = TypeProcess.TestMoving;

    private void OnCollisionEnter(Collision collision)
    {
        //Vector3 sizeObj = collision.transform.GetComponent<Collider>().bounds.size;
        return;
        if (collision.gameObject.tag == "Track") {
            return;
        }

        GameObject collisonObj = collision.gameObject;
        foreach (var timer in listTimerCollisions) {
            if (timer.collisionObj == collisonObj) {
                return;
            }
        }
        listTimerCollisions.Add(new TimerCollision(collisonObj));

        if (collision.gameObject.tag == "Target") {
            ContactPoint contact = collision.contacts[0];
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 position = contact.point;
            Instantiate(explosionPrefab, position, rotation);
            progressBar.value += 0.1f;

            //float d = 2f;
            //Transform car = collision.gameObject.transform;
            //float xDist = body.position.x - car.position.x;
            //float xShift = xDist > 0 ? -(d - xDist) : (d - Math.Abs(xDist));

            //if (xDist < d) {
            //    transform.position = new Vector3(transform.position.x - xShift, transform.position.y, transform.position.z);
            //    //car.position = new Vector3(car.position.x + xShift / 2, car.position.y, car.position.z);

            //    Vector3 vec = body.transform.rotation.eulerAngles;
            //    body.transform.rotation = Quaternion.Euler(vec.x, vec.y - xShift * 10, vec.z);
            //}

            if (CanvasSlots.IsSound) {
                GetComponent<AudioSource>().time = 3.4f;
                GetComponent<AudioSource>().Play();
            }
        }
        else {
            ContactPoint contact = collision.contacts[0];
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 position = contact.point;
            Instantiate(explosionPrefab, position, rotation);

            if (CanvasSlots.IsSound) {
                GetComponent<AudioSource>().time = 3.4f;
                GetComponent<AudioSource>().Play();
            }

            //GameController.IsLose = true;
            //speed = speedStart;
            //body.velocity = Vector3.zero;
            //audioMove.Stop();

            float d = 2f;
            Transform car = collision.gameObject.transform;
            float xDist = body.position.x - car.position.x;
            float xShift = xDist > 0 ? -(d - xDist) : (d - Math.Abs(xDist));

            if (xDist < d) {
                transform.position = new Vector3(transform.position.x - xShift / 2, transform.position.y, transform.position.z);
                car.position = new Vector3(car.position.x + xShift / 2, car.position.y, car.position.z);

                Vector3 vec = body.transform.rotation.eulerAngles;
                body.transform.rotation = Quaternion.Euler(vec.x, vec.y - xShift * 10, vec.z);
            }

            body.AddExplosionForce(force, position, radForce);

            //speed *= 0.8f;
            //Debug.Log("Crash: " + sizeObj.x + " " + sizeObj.y + " " + sizeObj.z + ". Tag: " + collision.gameObject.tag);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        foreach (var element in pathElementsShow) {
            Gizmos.DrawSphere(element.position, 2);
        }

        if (targetPoint) {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(targetPoint.position, 2);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(m_transform.position, targetPoint.position);
        }
    }

    void Start()
    {
        body = GetComponent<Rigidbody>();
        m_transform = GetComponent<Transform>();

        //audioMove = gameObject.AddComponent<AudioSource>();
        //audioMove.clip = soundMove;
        //audioMove.Play();

        speed = speedStart;

        targetPoint = Instantiate(track.GetChild(0), m_transform.position, Quaternion.identity);

        movingProccessor = new MovingProccessor(m_transform.position, TrackCreator.pathElementsMain);
    }

    bool isSetTestMarkers = true;

    void Update()
    {
        if(typeProcess == TypeProcess.TestGame) {
            if (Input.GetKey("left") || IsClickTurnLeft)
                isTurnLeft = true;
            else isTurnLeft = false;

            if (Input.GetKey("right") || IsClickTurnRight)
                isTurnRight = true;
            else isTurnRight = false;

            if (IsClickAccelerator || Input.GetKey("f")) {
                increaseSpeed();
            }
            else if (speed > 5) speed -= 1 * Time.deltaTime;

            if (IsClickBrekes || Input.GetKey("d")) {
                decreaseSpeed();
            }
        }
        else if(typeProcess == TypeProcess.TestMoving) {
            if (Input.GetKeyDown("q")) {
                movingProccessor.changeTrafficLane(m_transform.position, false);
                targetPoint.position = movingProccessor.getTarget();
            }
            else if (Input.GetKeyDown("e")) {
                movingProccessor.changeTrafficLane(m_transform.position, true);
                targetPoint.position = movingProccessor.getTarget();
            }
        }
        else if(typeProcess == TypeProcess.TestMarkers) {
            if (Input.GetKeyDown("q")) {
                if (typeProcess == TypeProcess.TestMarkers) clearShowList();
                movingProccessor.setLeftTrack();
            }
            else if (Input.GetKeyDown("e")) {
                if (typeProcess == TypeProcess.TestMarkers) clearShowList();
                movingProccessor.setRightTrack();
            }

            if (isSetTestMarkers) {
                do {
                    Vector3[] targets = movingProccessor.getAllTargets();

                    for (int i = 0; i < targets.Length; i++) {
                        Transform newTransform = Instantiate(track.GetChild(0), targets[i], Quaternion.identity);
                        pathElementsShow.Add(newTransform);
                    }
                }
                while (movingProccessor.setNext());

                isSetTestMarkers = false;
            }
        }

        //testMoving();
        //testDraw();

        for (int i = 0; i < listTimerCollisions.Count; i++) {
            if(listTimerCollisions[i].isProcess()) {
                listTimerCollisions.RemoveAt(i);
                i--;
            }
        }

        if (GetComponent<AudioSource>().isPlaying && GetComponent<AudioSource>().time >= 5.0f)
            GetComponent<AudioSource>().Stop();

        //if (audioMove.isPlaying && audioMove.time >= 9.0f)
        //    audioMove.time = 1.5f;
    }

    void clearShowList()
    {
        //foreach (var element in pathElementsShow) {
        //    Destroy(element);
        //}
        pathElementsShow.Clear();
        movingProccessor.restart();
        isSetTestMarkers = true;
    }

    private void FixedUpdate()
    {
        float t = Time.fixedDeltaTime;

        if (typeProcess == TypeProcess.TestMoving) testMoving();
        else if(typeProcess == TypeProcess.TestGame) {
            float curAng, tarAng;

            if (isTurnLeft) {
                curAng = m_transform.rotation.eulerAngles.y;
                tarAng = curAng - 90;
                tarAng = tarAng < 0 ? 360 + tarAng : tarAng;
                Quaternion turnTo = Quaternion.Euler(0, tarAng, 0);
                body.transform.rotation = Quaternion.Lerp(transform.rotation, turnTo, speedTurn * t);
            }
            else if (isTurnRight) {
                curAng = m_transform.rotation.eulerAngles.y;
                tarAng = curAng + 90;
                tarAng = tarAng >= 360 ? tarAng - 360 : tarAng;
                Quaternion turnTo = Quaternion.Euler(0, tarAng, 0);
                body.transform.rotation = Quaternion.Lerp(transform.rotation, turnTo, speedTurn * t);
            }

            body.MovePosition(transform.position + transform.TransformDirection(Vector3.forward * speed * t));
        }

        //body.AddForce(Vector3.forward * speed * t, ForceMode.VelocityChange);
    }

    float m_graund = 0;

    void testMoving()
    {
        float t = Time.fixedDeltaTime;

        Vector3 dir = movingProccessor.getTarget() - m_transform.position;
        if(movingProccessor.getTarget().y >= m_transform.position.y) dir.y = m_graund;
        body.MovePosition(m_transform.position + dir.normalized * speed * t);

        Quaternion turnTo = Quaternion.LookRotation(dir);
        m_transform.rotation = Quaternion.Lerp(m_transform.rotation, turnTo, speed / 4 * t);

        if (dir.sqrMagnitude < 1f * 1f) {
            movingProccessor.setNext();
            targetPoint.position = movingProccessor.getTarget();
            //if ((movingProccessor.getTarget().y > 0) && (movingProccessor.getTarget().y < m_transform.position.y)) m_graund = (movingProccessor.getTarget().y - m_transform.position.y) / 2;
            //else m_graund = 0;

            //Test
            Vector3[] targets = movingProccessor.getAllTargets();
            for (int i = 0; i < targets.Length; i++) {
                Transform newTransform = Instantiate(track.GetChild(0), targets[i], Quaternion.identity);
                pathElementsShow.Add(newTransform);
            }
            //
        }
    }

    private void testDraw()
    {
        Vector3 dir = movingProccessor.getTarget() - m_transform.position;
        Debug.DrawRay(m_transform.position, dir, Color.green);
    }

    private void increaseSpeed()
    {
        if (speed >= speedMax) return;

        float speed1 = speedMax * 0.3f, speed2 = speedMax * 0.6f, speed3 = speedMax * 0.8f, speed4 = speedMax * 0.9f;
        float spInc = 0;

        if (speed < speed1) spInc = speedIncrease * 2;
        else if (speed < speed2) spInc = speedIncrease;
        else if (speed < speed3) spInc = speedIncrease * 0.5f;
        else spInc = speedIncrease * 0.2f;

        speed += spInc * Time.deltaTime;
    }
    private void decreaseSpeed()
    {
        if (speed <= 0) return;

        speed -= speedDecrease * Time.deltaTime;
    }
}

class MovingProccessor
{
    enum TypeMove { Directly, Sinuous, Free };
    TypeMove typeMove;
    Vector3 currentPos;
    Vector3 targetPos;
    Vector3 startPos;
    List<Transform>[] listMarker = new List<Transform>[4];
    List<Vector3> targets = new List<Vector3>();
    int countDiv = 15;
    int counterMarker = 0;
    int countMarkersRemember = 0;
    int countTarget = 0;
    int track = 1;
    bool isStop = false;

    public MovingProccessor(Vector3 pos, List<Transform>[] list)
    {
        listMarker = list;
        typeMove = TypeMove.Directly;

        startPos = currentPos = pos;
        startProcess();
    }

    void startProcess()
    {
        setTargets();

        targetPos = targets[countTarget];
    }

    public Vector3 getTarget()
    {
        return targetPos;
    }

    public Vector3[] getAllTargets()
    {
        return targets.ToArray();
    }

    public Vector3 getStart()
    {
        return currentPos;
    }

    public void setLeftTrack()
    {
        if (track > 0) track--;
    }

    public void setRightTrack()
    {
        if (track < 3) track++;
    }

    public void changeTrafficLane(Vector3 pos, bool isRight)
    {
        if ((isRight && track >= 3) || (!isRight && track <= 0)) {
            return;
        }

        track += isRight ? 1 : -1;
        recountTargetsChangeTraffic();
        currentPos = pos;
        targetPos = targets[countTarget];
    }

    public void setTrack(int n)
    {
        if (n >= 0 && n < listMarker.Length) track = n;
    }

    public bool setNext()
    {
        if (isStop) return false;

        if (++countTarget >= targets.Count) {
            countTarget = 0;
            setTargets();

            if (targets.Count == 0) {
                Debug.Log("Warning! The track is over");
                isStop = true;
                return false;
            }
        }

        currentPos = targetPos;
        if(targets.Count != 0) targetPos = targets[countTarget];

        return true;
    }

    void recountTargetsChangeTraffic()
    {
        targets.Clear();

        if (typeMove == TypeMove.Directly) {
            for (int i = countMarkersRemember; i < counterMarker; i++) {
                if (listMarker[track][i].tag != "UnusedMarker") targets.Add(listMarker[track][i].position);
            }
        }
        else if (typeMove == TypeMove.Sinuous) {
            List<Vector3> tempList = new List<Vector3>();

            for (int i = countMarkersRemember; i < counterMarker; i++) {
                if (listMarker[track][i].tag != "UnusedMarker") tempList.Add(listMarker[track][i].position);
            }

            if (tempList.Count < 10 && tempList.Count >= 3) {
                float t = 0f;
                for (int i = 0; i < countDiv; i++, t += 1.0f / countDiv) {
                    targets.Add(Bezier.GetPoint(t, tempList.ToArray()));
                }
                targets.Add(Bezier.GetPoint(1f, tempList.ToArray()));
            }
            else {
                for (int i = 0; i < tempList.Count; i++) {
                    targets.Add(tempList[i]);
                }
            }
        }
    }

    void setTargets()
    {
        targets.Clear();
        countMarkersRemember = counterMarker;

        if (listMarker[track].Count <= counterMarker) return;

        typeMove = listMarker[track][counterMarker].tag == "TurnMarker" ? TypeMove.Sinuous : TypeMove.Directly;

        if (typeMove == TypeMove.Directly) {
            while (counterMarker < listMarker[track].Count && listMarker[track][counterMarker].tag != "TurnMarker") {
                if (listMarker[track][counterMarker].tag != "UnusedMarker") targets.Add(listMarker[track][counterMarker].position);
                counterMarker++;
            }
        }
        else if (typeMove == TypeMove.Sinuous) {
            countSinous();
        }
    }

    void countSinous()
    {
        List<Vector3> tempList = new List<Vector3>();

        if (counterMarker > 0 && listMarker[track][counterMarker - 1].tag == "StartTurn") tempList.Add(listMarker[track][counterMarker - 1].position);

        while (counterMarker < listMarker[track].Count && listMarker[track][counterMarker].tag != "EndTurn" && listMarker[track][counterMarker].tag != "StartTurn" && listMarker[track][counterMarker].tag != "Untagged") {
            if (listMarker[track][counterMarker].tag != "UnusedMarker") tempList.Add(listMarker[track][counterMarker].position);
            counterMarker++;
        }

        if (counterMarker < listMarker[track].Count && (listMarker[track][counterMarker].tag == "EndTurn" || listMarker[track][counterMarker].tag == "StartTurn")) tempList.Add(listMarker[track][counterMarker++].position);

        if (tempList.Count < 10 && tempList.Count >= 3) {
            float t = 0f;

            for (int i = 0; i < countDiv; i++, t += 1.0f / countDiv) {
                targets.Add(Bezier.GetPoint(t, tempList.ToArray()));
            }
            targets.Add(Bezier.GetPoint(1f, tempList.ToArray()));
        }
        else {
            for (int i = 0; i < tempList.Count; i++) {
                targets.Add(tempList[i]);
            }
        }
    }

    public void restart()
    {
        typeMove = TypeMove.Directly;
        counterMarker = 0;
        countTarget = 0;
        isStop = false;

        setTargets();

        currentPos = startPos;
        targetPos = targets[countTarget];
    }
}

class ControlerTrack
{
    int nextMarker = 0;
    int currentPath = 2;
    float critDist = 3f;
    float pastDistance = -1;
    Vector3 currentMarker = new Vector3();

    List<Vector3>[] markersTrack = new List<Vector3>[4];

    public void setMarkersTrack(List<Vector3> markers, int n)
    {
        if (n < 0 || n >= markersTrack.Length || markers == null) return;
        markersTrack[n] = markers;
        if(n == markersTrack.Length - 1) currentMarker = markersTrack[currentPath][nextMarker];
    }

    public void setCurrentPath(int n)
    {
        if(n >= 0 && n < markersTrack.Length) {
            currentPath = n;
        }
    }

    public bool checkDistance(Vector3 carPos)
    {
        bool isNext = false;
        float dist = Vector3.Distance(carPos, currentMarker);

        if(pastDistance == -1f) pastDistance = dist;

        if ((dist < critDist || dist > (pastDistance + critDist)) && nextMarker < (markersTrack[currentPath].Count - 1)) {
            currentMarker = markersTrack[currentPath][++nextMarker];
            pastDistance = Vector3.Distance(carPos, currentMarker);;
            isNext = true;
        }

        if (dist < pastDistance && !isNext) pastDistance = dist;

        return isNext;
    }

    public Vector3 getCurrentMarker()
    {
        return currentMarker;
    }
}

class Turner
{
    public enum Turn { None, Left, Right };

    public Turn turn = Turn.None;
    float maxSpeed = 12f;
    public float turnSpeed = 1f;
    float targetAngle = 0;
    public float currentAngle;
    bool isTransitionLeft = false;
    bool isTransitionRight = false;
    float lastAng;

    public void setTarget(float tarAng, float curAng)
    {
        if (tarAng < 0) {
            turn = Turn.Left;
        } else {
            turn = Turn.Right;
        }

        targetAngle = curAng + tarAng;

        if(targetAngle >= 360 || targetAngle < 0) {
            if (targetAngle >= 360) {
                targetAngle -= 360;
                isTransitionRight = true;
            }
            if (targetAngle < 0) {
                targetAngle += 360;
                isTransitionLeft = true;
            }
        }

        float dif = Mathf.Abs(tarAng);
        turnSpeed = (dif / 90) * maxSpeed;
        //Debug.Log("Set: " + turn + " : " + tarAng + " : " + curAng + ". Target: " + targetAngle + " : " + turnSpeed);
    }

    public void setStartAngle(float ang)
    {
        targetAngle = ang;
        lastAng = ang;
    }

    public float getTarget()
    {
        return currentAngle;
    }

    public bool checkReady(float ang)
    {
        bool isReady = false;

        if(isTransitionLeft && ang > lastAng) {
            isTransitionLeft = false;
        }
        else if (isTransitionRight && ang < lastAng) {
            isTransitionRight = false;
        }
        lastAng = ang;

        if (turn == Turn.Left) {
            if (ang <= targetAngle && !isTransitionLeft) {
                isReady = true;
                turn = Turn.None;
                //Debug.Log("Ready: " + ang + " : " + targetAngle);
            } 
            else currentAngle = ang - 20;
        }
        else if (turn == Turn.Right) {
            if (ang >= targetAngle && !isTransitionRight) {
                isReady = true;
                turn = Turn.None;
                //Debug.Log("Ready: " + targetAngle);
            } 
            else currentAngle = ang + 20;
        }

        if (currentAngle >= 360 || currentAngle < 0) {
            if (currentAngle >= 360) currentAngle -= 360;
            if (currentAngle < 0) currentAngle += 360;
        }

        return isReady;
    }
}

class SimpleTimer
{
    float timer = 1.0f;
    bool isStart = false;

    public void start(float time)
    {
        timer = time;
        isStart = true;
    }

    public bool isStarting()
    {
        return isStart;
    }

    public bool isProcess()
    {
        if (!isStart) return false;

        if (timer > 0) {
            timer -= Time.deltaTime;
            return false;
        } else {
            isStart = false;
            return true;
        }
    }
}

class TimerCollision
{
    float timer = 1.0f;
    public GameObject collisionObj = null;

    public TimerCollision(GameObject collObj)
    {
        collisionObj = collObj;
    }

    public bool isProcess()
    {
        if (timer > 0) {
            timer -= Time.deltaTime;
            return false;
        } else {
            return true;
        }
    }
}
