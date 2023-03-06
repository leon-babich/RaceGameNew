using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCreator : MonoBehaviour
{
    public Transform track;

    static public List<Transform>[] pathElementsMain = new List<Transform>[4];

    void Start()
    {
        for (int i = 0; i < pathElementsMain.Length; i++) {
            pathElementsMain[i] = new List<Transform>();
        }

        for (int i = 0; i < track.childCount; i++) {
            Vector3 newVec1 = RotationCounter.getShiftMarker(track.GetChild(i).position, track.GetChild(i).rotation.eulerAngles.y, 7.5f, -1);
            Transform newTransform1 = Instantiate(track.GetChild(i), newVec1, Quaternion.identity);
            pathElementsMain[0].Add(newTransform1);

            Vector3 newVec2 = RotationCounter.getShiftMarker(track.GetChild(i).position, track.GetChild(i).rotation.eulerAngles.y, 2.5f, -1);
            Transform newTransform2 = Instantiate(track.GetChild(i), newVec2, Quaternion.identity);
            pathElementsMain[1].Add(newTransform2);

            Vector3 newVec3 = RotationCounter.getShiftMarker(track.GetChild(i).position, track.GetChild(i).rotation.eulerAngles.y, 2.5f, 1);
            Transform newTransform3 = Instantiate(track.GetChild(i), newVec3, Quaternion.identity);
            pathElementsMain[2].Add(newTransform3);

            Vector3 newVec4 = RotationCounter.getShiftMarker(track.GetChild(i).position, track.GetChild(i).rotation.eulerAngles.y, 7.5f, 1);
            Transform newTransform4 = Instantiate(track.GetChild(i), newVec4, Quaternion.identity);
            pathElementsMain[3].Add(newTransform4);

            //markersTrack.Add(newVec3);
        }

    }


    void Update()
    {
        
    }
}
