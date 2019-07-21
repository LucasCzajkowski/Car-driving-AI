﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookAtTarget : MonoBehaviour
{
    public Transform Target;

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Target);
    }
}
