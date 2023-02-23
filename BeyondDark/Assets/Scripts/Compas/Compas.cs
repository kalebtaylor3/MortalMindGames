using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compas : MonoBehaviour
{

    public GameObject markerPrefab;
    List<HouseMarkers> houseMarkers = new List<HouseMarkers>();

    public RawImage compasImage;
    public Transform player;

    public HouseMarkers house1;
    public HouseMarkers house2;
    public HouseMarkers house3;

    float compassUnit;

    private void Start()
    {
        compassUnit = compasImage.rectTransform.rect.width / 360f;
        AddHoseMarker(house1);
        AddHoseMarker(house2);
        AddHoseMarker(house3);
    }

    private void Update()
    {
        compasImage.uvRect = new Rect(player.localEulerAngles.y / 360f, 0f, 1f, 1f);

        foreach(HouseMarkers marker in houseMarkers)
        {
            marker.image.rectTransform.anchoredPosition = GetPosOnCompass(marker);
        }

    }

    public void AddHoseMarker(HouseMarkers marker)
    {
        GameObject newMarker = Instantiate(markerPrefab, compasImage.transform);
        marker.image = newMarker.GetComponent<Image>();
        marker.image.sprite = marker.icon;

        houseMarkers.Add(marker);
    }

    Vector2 GetPosOnCompass(HouseMarkers marker)
    {
        Vector2 playerPosition = new Vector2(player.transform.position.x, player.transform.position.z);
        Vector2 playerFwd = new Vector2(player.transform.forward.x, player.transform.forward.z);

        float angle = Vector2.SignedAngle(marker.position - playerPosition, playerFwd);


        return new Vector2(compassUnit * angle, 0);
    }
}
