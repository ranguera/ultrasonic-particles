using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

public class VFXManager : MonoBehaviour
{
    public OSC osc;

    private VisualEffect ball;
    private List<float> buffer;

    private int initial_buffer_counter;

    // Start is called before the first frame update
    void Start()
    {
        buffer = new List<float>(10);

        for (int i = 0; i < 10; i++)
        {
            buffer.Add(0f);
        }

        ball = GetComponent<VisualEffect>();
        osc.SetAddressHandler("/topc", OnReceiveVal);
        //osc.SetAllMessageHandler(OnReceiveVal);
    }

    // Update is called once per frame
    void Update()
    {
        //ProcessOSC(oscValue.intensity);

        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();
    }

    // Event triggered when OSC value received
    void OnReceiveVal(OscMessage message)
    {
        float val = message.GetFloat(0);

        // Add new element, remove first one, like a queue
        buffer.Add(val);
        buffer.RemoveAt(0);

        initial_buffer_counter++;

        if(initial_buffer_counter>=10)
            // Smooth and render
            ProcessOSC(Smooth());
    }

    // Sets VFX parameters mapped from the clean value
    private void ProcessOSC(float val)
    {
        ball.SetInt("NumParticles", Mathf.RoundToInt( MapInterval(val, 5f, 20f, 500f, 15000f)));
        ball.SetFloat("Radius", MapInterval(val, 5f, 20f, .5f, 3f));
        ball.SetFloat("Intensity", MapInterval(val, 5f, 20f, 1, 5f));
    }

    private float Smooth()
    {
        float avg = 0f;

        // calculate avg from the list
        for (int i = 0; i < buffer.Count; i++)
        {
            avg += buffer[i];
        }

        avg /= buffer.Count;

        return avg;
    }

    // Maps value from one range to another
    private float MapInterval(float val, float srcMin, float srcMax, float dstMin, float dstMax)
    {
        if (val >= srcMax) return dstMax;
        if (val <= srcMin) return dstMin;
        return dstMin + (val - srcMin) / (srcMax - srcMin) * (dstMax - dstMin);
    }

    // Return the median element of a list
    private float GetMedian(List<float> list)
    {
        float[] tempArray = list.ToArray();
        int count = tempArray.Length;
        Array.Sort(tempArray);
        float medianValue = 0f; 

        if (count % 2 == 0)
        {
            // count is even, need to get the middle two elements, add them together, then divide by 2
            float middleElement1 = tempArray[(count / 2) - 1];
            float middleElement2 = tempArray[(count / 2)];
            medianValue = (middleElement1 + middleElement2) / 2;
        }
        else
        {
            // count is odd, simply get the middle element.
            medianValue = tempArray[(count / 2)];
        }

        return medianValue;
    }
}
