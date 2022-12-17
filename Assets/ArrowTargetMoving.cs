using UnityEngine;
using Unity;

public class ArrowTargetMoving : MonoBehaviour
{
    static public Transform target;
    public Transform canvasTransform;
    public Canvas canvas;
    float normSizeArrow = 30f;
    public float normShiftArrow = 0.07f;
    public float normDistance = 70f;
    float shiftAnimation = 0f;
    bool riseAnimation = true;
    float maxAnimation = 0.02f;
    float stepAnimation = 0.0001f;
    Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (!target) {
            return;
        }

        if (riseAnimation) shiftAnimation += stepAnimation;
        else shiftAnimation -= stepAnimation;

        if (riseAnimation && shiftAnimation >= maxAnimation) riseAnimation = false;
        else if (!riseAnimation && shiftAnimation < 0) riseAnimation = true;

        float distance = Vector3.Distance(target.position, mainCamera.transform.position);
        Vector3 screenPosTarg = mainCamera.WorldToViewportPoint(target.position);
        float widthScreen = canvas.GetComponent<RectTransform>().rect.width;
        float heightScreen = canvas.GetComponent<RectTransform>().rect.height;
        float sizeArrow = (widthScreen + heightScreen) / 50;
        float shift = normShiftArrow * sizeArrow / normSizeArrow * normDistance / distance;
        shift = shift > normShiftArrow ? normShiftArrow : shift;
        //float shift = normShiftArrow;
        float posX = screenPosTarg.x;
        float posY = screenPosTarg.y + shift + shiftAnimation;

        RectTransform rectArrow = gameObject.GetComponent<RectTransform>();
        rectArrow.sizeDelta = new Vector2(sizeArrow, sizeArrow);
        rectArrow.anchoredPosition = new Vector2(widthScreen * posX, heightScreen * posY);
        //Debug.Log(widthScreen + " " + heightScreen);
        Debug.Log(distance);
    }
}
