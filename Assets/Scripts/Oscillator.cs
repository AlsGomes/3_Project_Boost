using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector;
    [SerializeField] float period = 2f;
    [SerializeField] float movementFactor;
    Vector3 startingPos;

    void Start()
    {
        startingPos = transform.position;
    }

    void Update()
    {
        Oscillate();
    }

    private void Oscillate()
    {
        if (period <= Mathf.Epsilon)
        {
            return;
        }

        const float tau = Mathf.PI * 2f; //about 6.28
        float cycles = Time.time / period; //grows continually from 0
        float rawSinWave = Mathf.Sin(cycles * tau); // goes from -1 to +1
        movementFactor = rawSinWave / 2f + 0.5f;
        Vector3 offset = movementFactor * movementVector;
        transform.position = startingPos + offset;
    }
}
