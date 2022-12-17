using UnityEngine;
using UnityEngine.EventSystems;

public class ListenerButRight : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        PlayerMovement.IsClickTurnRight = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PlayerMovement.IsClickTurnRight = false;
    }
}
