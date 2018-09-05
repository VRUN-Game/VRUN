using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DecorationCtrl : MonoBehaviour
{
    [Serializable]
    public class DecorationItem
    {
        public GameObject Go;
        public CustomVector Height;
    }

    [Serializable]
    public class CustomVector
    {
        public float Min;
        public float Max;
    }

    [Header("Bereich nach Links/Rechts in dem Objekte gespawnt werden")]
    public int DefineSegmentWidth = 20;

    [Header("Beide Seiten (Sonst nur rechts)")]
    public bool BothSides = false;

    public GameObject ParentGameObject;
    public float ZDistanceToNextObj = 10;
    public int MaxPartsToSpawn = 150;
    public int OffsetToPlayer = 100;

    [Header("Elemente")] public List<DecorationItem> DecorationParts;

    private List<GameObject> _activeParts = new List<GameObject>();
    private float _segmentBoundsZMax = 0;
    private float _segmentBoundsZMin = 0;
    private float _segmentBoundsXMax = 0;
    private float _segmentBoundsXMin = 0;

    private Quaternion GetRandomRotation()
    {
        return Quaternion.Euler(
            Random.Range(-70.0f, -108.0f),
            Random.Range(-40.0f, 40.0f),
            Random.Range(160.0f, 250.0f)
        );
    }

    private Vector3 GetRandomPosition(float minZ, float maxZ, int decorationIndex)
    {
        var tmpPosition = new Vector3(
            GetRandomXValue(),
            Random.Range(DecorationParts[decorationIndex].Height.Min, DecorationParts[decorationIndex].Height.Max), 
            Random.Range(minZ, maxZ)
        );

        _activeParts.ForEach(part =>
        {
            while (Math.Abs(Vector3.Distance(part.transform.position, tmpPosition)) <= 5)
            {
                tmpPosition = GetRandomPosition(minZ, maxZ, decorationIndex);
            }
        });

        return tmpPosition;
    }

    private float GetRandomXValue()
    {
        float tmpXValue = 0;

        while (tmpXValue <= GlobalDataHandler.TrackWidth && tmpXValue >= -GlobalDataHandler.TrackWidth)
        {
            tmpXValue = Random.Range(_segmentBoundsXMin, _segmentBoundsXMax);
        }

        return tmpXValue;
    }

    private void Start()
    {
        CalculateSegmentBounds();
        SpawnDecorationParts();
    }

    private void FixedUpdate()
    {
        CalculateSegmentBounds();
        CheckForDecorationsOutOfBoundsAndRemove();
        SpawnDecorationParts();
    }

    private void SpawnDecorationParts()
    {
        if (_activeParts.Count >= MaxPartsToSpawn) return;

        float zTmpMin;

        if (_activeParts.Count == 0)
        {
            zTmpMin = _segmentBoundsZMin;
        }
        else
        {
            zTmpMin = _activeParts[_activeParts.Count - 1].transform.position.z + GlobalDataHandler.TrackWidth;
        }
        
        float zTmpMax = zTmpMin + ZDistanceToNextObj;

        if (zTmpMax >= _segmentBoundsZMax) return;

        var decorationIndex = Random.Range(0, DecorationParts.Count);
        var go = Instantiate(DecorationParts[decorationIndex].Go, GetRandomPosition(zTmpMin, zTmpMax , decorationIndex), GetRandomRotation());
        go.transform.SetParent(ParentGameObject.transform);
        _activeParts.Add(go);
    }

    private void CalculateSegmentBounds()
    {
        var playerPos = GlobalDataHandler.GetPlayerPosition();
        _segmentBoundsXMin = BothSides ? -GlobalDataHandler.TrackWidth - DefineSegmentWidth : GlobalDataHandler.TrackWidth;
        _segmentBoundsXMax = GlobalDataHandler.TrackWidth + DefineSegmentWidth;
        _segmentBoundsZMin = playerPos - OffsetToPlayer;
        _segmentBoundsZMax = playerPos + OffsetToPlayer;
    }

    private void CheckForDecorationsOutOfBoundsAndRemove()
    {
        _activeParts.RemoveAll(part => part.transform.position.z <= _segmentBoundsZMin);
    }
    
}