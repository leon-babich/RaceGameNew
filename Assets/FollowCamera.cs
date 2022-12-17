using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform player;

    public Vector3 offSet;

    public float speedRotate = 5.0f;
    private float distance;
    public float cameraHeight = 5.0f;

    private void Start()
    {
        transform.position = new Vector3(player.position.x, player.position.y + offSet.y, player.position.z + offSet.z);
        transform.rotation = Quaternion.Euler(20, 0, 0);
        distance = Vector3.Distance(player.position, transform.position);
    }

    void Update()
    {   //transform.position = new Vector3(player.position.x, player.position.y + offSet.y, player.position.z + offSet.z);
        //transform.RotateAround(player.position, Vector3.up, speedRotate * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        transform.position = player.position + distance * (transform.position - player.position).normalized;
        transform.position = new Vector3(transform.position.x, player.position.y + cameraHeight, transform.position.z);

        if (transform.rotation != player.rotation) {
            float angPlayer = player.rotation.y, angCamera = transform.rotation.y;
            float ang = (angPlayer - angCamera) * 180;
            transform.RotateAround(player.position, Vector3.up, ang);
        }
    }
}
