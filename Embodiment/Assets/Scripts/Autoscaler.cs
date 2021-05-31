using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Autoscaler : MonoBehaviour
{
    public float defaultHeight = 1.7f;
    public Camera camera;

    private void Resize()
    {
        float headHeight = camera.transform.localPosition.y;
        float scale = defaultHeight / headHeight;
        transform.localScale = Vector3.one * scale;
    }

    void OnEnable()
    {
        Resize();
    }


}
