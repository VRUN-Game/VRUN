﻿using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UiButtonCtrl: MonoBehaviour
{
    private BoxCollider _boxCollider;
    private RectTransform _rectTransform;

    private void OnEnable()
    {
        ValidateCollider();
    }

    private void OnValidate()
    {
        ValidateCollider();
    }

    private void ValidateCollider()
    {
        _rectTransform = GetComponent<RectTransform>();

        _boxCollider = GetComponent<BoxCollider>();
        if (_boxCollider == null)
        {
            _boxCollider = gameObject.AddComponent<BoxCollider>();
        }

        _boxCollider.size = _rectTransform.sizeDelta;
        _boxCollider.center = new Vector3(_boxCollider.center.x, _rectTransform.sizeDelta.y / 2, _boxCollider.center.z);
    }
}