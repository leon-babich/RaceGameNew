using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassController : MonoBehaviour
{
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
        if (targets.Count != 0) targetPos = targets[countTarget];

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
        }
        else {
            turn = Turn.Right;
        }

        targetAngle = curAng + tarAng;

        if (targetAngle >= 360 || targetAngle < 0) {
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

        if (isTransitionLeft && ang > lastAng) {
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
        if (n == markersTrack.Length - 1) currentMarker = markersTrack[currentPath][nextMarker];
    }

    public void setCurrentPath(int n)
    {
        if (n >= 0 && n < markersTrack.Length) {
            currentPath = n;
        }
    }

    public bool checkDistance(Vector3 carPos)
    {
        bool isNext = false;
        float dist = Vector3.Distance(carPos, currentMarker);

        if (pastDistance == -1f) pastDistance = dist;

        if ((dist < critDist || dist > (pastDistance + critDist)) && nextMarker < (markersTrack[currentPath].Count - 1)) {
            currentMarker = markersTrack[currentPath][++nextMarker];
            pastDistance = Vector3.Distance(carPos, currentMarker); ;
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
        }
        else {
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
        }
        else {
            return true;
        }
    }
}