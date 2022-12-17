using UnityEngine;
using TMPro;

public class ArrowSpeedometerMoving : MonoBehaviour
{
    public GameObject arrow;

    public TextMeshProUGUI infoTxt;

    void Update()
    {
        float speed = PlayerMovement.speed;
        float ang = speed * 2.5f;
        arrow.transform.rotation = Quaternion.Euler(0, 0, -ang);
        infoTxt.text = ((int)speed).ToString();
    }
}
