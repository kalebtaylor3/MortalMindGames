using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compas : MonoBehaviour
{
    public RawImage compasImage;
    public Transform player;

    private void Update()
    {
        compasImage.uvRect = new Rect(player.localEulerAngles.y / 360f, 0f, 1f, 1f);
    }
}
