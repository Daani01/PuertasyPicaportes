using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoCabeza : MonoBehaviour
{
    [Range(0.001f, 0.01f)]
    public float amount;

    [Range(1f, 30f)]
    public float frequency;

    [Range(10f, 100f)]
    public float smooth;

    Vector3 startPos;

    void Update()
    {
        //checkForHeadbobTrigger();
        //stopHeadbob();
        startHeadbob();
    }

    void checkForHeadbobTrigger()
    {
        float inputMagnitude = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).magnitude;
        if(inputMagnitude > 0)
        {
            startHeadbob();
        }
    }

    Vector3 startHeadbob()
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Lerp(pos.y, Mathf.Sin(Time.time * frequency) * amount * 1.4f, smooth * Time.deltaTime);
        pos.x += Mathf.Lerp(pos.x, Mathf.Cos(Time.time * frequency / 2f) * amount * 1.6f, smooth * Time.deltaTime);
        transform.localPosition += pos;
        return pos;
    }

    void stopHeadbob()
    {
        if (transform.localPosition == startPos) return;
        transform.localPosition = Vector3.Lerp(transform.localPosition, startPos, 1* Time.deltaTime);
    }
}
