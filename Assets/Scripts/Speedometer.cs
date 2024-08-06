using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Speedometer : MonoBehaviour
{
    public Rigidbody car;
    public float maxSpeed = 260.0f; // in km/h
    public float minArrowAngle;
    public float maxArrowAngle;
    public TextMeshProUGUI speedText;
    public RectTransform speedArrow;

    // Update is called once per frame
    void Update()
    {
        float speed = car.velocity.magnitude * 3.6f; // m/s to km/h
        if (speedText != null)
        {
            speedText.text = ((int)speed) + " km/h";
        }
        if (speedArrow != null)
        {
            float normalizedSpeed = speed / maxSpeed;
            float angle = Mathf.Lerp(minArrowAngle, maxArrowAngle, normalizedSpeed);
            speedArrow.localEulerAngles = new Vector3(0, 0, angle);
        }
    }
}
