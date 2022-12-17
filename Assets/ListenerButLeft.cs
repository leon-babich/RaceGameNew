using UnityEngine;
using UnityEngine.EventSystems;

public class ListenerButLeft : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        PlayerMovement.IsClickTurnLeft = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PlayerMovement.IsClickTurnLeft = false;
    }
}
