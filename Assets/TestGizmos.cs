using UnityEngine;

public class TestGizmos : MonoBehaviour
{
    public Transform[] pathElements;
    private void OnDrawGizmos()
    {
        if (pathElements == null) return;

        Gizmos.color = Color.red;
        string tagMarker = "Untagged";
        int s = 2;
        foreach(var element in pathElements) {
            if(element.tag != tagMarker) {
                if (element.tag == "TurnMarker" || element.tag == "EndTurn") {
                    Gizmos.color = Color.green;
                    s = 2;
                }
                else if (element.tag == "UnusedMarker") {
                    Gizmos.color = Color.black;
                    s = 1;
                }
                else {
                    Gizmos.color = Color.red;
                    s = 2;
                }

                tagMarker = element.tag;
            }
            Gizmos.DrawSphere(element.position, s);
        }

        //if (pathElements.Length < 2) return;

        //Gizmos.color = Color.white;
        //for (int i=1; i<pathElements.Length; i++) {
        //    Gizmos.DrawLine(pathElements[i-1].position, pathElements[i].position);
        //}
    }
}
