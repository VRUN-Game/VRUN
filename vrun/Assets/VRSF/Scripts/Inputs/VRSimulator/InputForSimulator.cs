using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using VRSF.Utils;

public class InputForSimulator : MonoBehaviour
{
    // Use this for initialization
    private Transform _parentTransform;
    public float InitHeight = 3.9f;
    private AudioListener audioListener;

    void Start()
    {
        audioListener = GetComponent<AudioListener>();
        audioListener.enabled = false;
        if (VRSF_Components.DeviceLoaded != EDevice.SIMULATOR) return;
        audioListener.enabled = true;
        _parentTransform = transform.parent.transform;
        _parentTransform.position = new Vector3(0, InitHeight, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (VRSF_Components.DeviceLoaded != EDevice.SIMULATOR) return;
        audioListener.enabled = true;
        var _input = Vector3.zero;
        _input.x = Input.GetAxis("Horizontal");
        _input.y = Input.GetAxis("Vertical");
        _parentTransform.position += _input * GlobalDataHandler.GetPlayerspeed() * Time.deltaTime;
    }
}