using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ObstaclesList
{
    public GameObject Obstcls;
    public bool IgnoreLanePosition;
    public bool IsStoneFigure;
}

[System.Serializable]
public class CollectiblesList
{
    public GameObject Cllbls;
    public bool CanGroup;
}

public class GeneralSpawner : MonoBehaviour
{
    //Obstacles
    public ObstaclesList[] Obstacles;
    public float MinDistance = 20;
    public float MaxDistance = 30;

    //Portal
    public GameObject Portal;
    public GameObject Particles;
    public string NextLevelToLoad;
    public int PortalSpawnPosition;

    //Collectibles
    public CollectiblesList[] Collectibles;


	private int _billowOffset;
	private int[] _groupMembers = { 1, 3, 7 };
	private ObjectList _objList;
    private float _lastSpawnPosZ;
    private float _lastSpawnPosZCollectibles;
    private const float InitSpawnOffset = 100f;
    private static System.Random _rnd;

    // Use this for initialization
    private void Start()
    {
		_billowOffset = _groupMembers[_groupMembers.Length - 1];
		_rnd = new System.Random();
        _objList = GetComponent<ObjectList>();
        _lastSpawnPosZ = GlobalDataHandler.GetPlayerPosition() + _objList.MaxSpawnDistance - InitSpawnOffset;
        SpawnPortal();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!GlobalDataHandler.GetGameModus()) return;
        if (!_objList.IsListFull())
        {
            SpawnObject();
            SpawnCollectibles();
        }

        DeleteObjects();
    }

    private void SpawnObject()
    {
        //Defining the Object
        var index = DetermineNextObstacle();

        //Defining z Postion
        var z = _lastSpawnPosZ + Random.Range(MinDistance, MaxDistance);
        if (z >= PortalSpawnPosition - MaxDistance) return;

        //Defining x Position
        var x = Obstacles[index].IgnoreLanePosition ? 0 : DetermineNextLanePos();

        //Exception for the stone figures in level 3
        if (Obstacles[index].IsStoneFigure)
        {
            //TODO check ob das Funktioniert (Spiegelung der Figuren LVL3)
            x = _rnd.Next(0, 2) != 0 ? 0 : 2;
        }

        //Defining the rotation
        var rotation = Obstacles[index].IsStoneFigure
            ? new Quaternion(
                Obstacles[index].Obstcls.transform.localRotation.x * (x == 0 ? -1.0f : 1.0f),
                Obstacles[index].Obstcls.transform.localRotation.y,
                Obstacles[index].Obstcls.transform.localRotation.z,
                Obstacles[index].Obstcls.transform.localRotation.w * (x == 0 ? -1.0f : 1.0f)
            )
            : Obstacles[index].Obstcls.transform.rotation;

        //Defining the position
        var position = new Vector3(
            Obstacles[index].IsStoneFigure ? Obstacles[index].Obstcls.transform.position.x - (x == 0 ? 10 : 0) : x,
            Obstacles[index].Obstcls.transform.position.y,
            z
        );

        //Create the object
        var go = Instantiate(Obstacles[index].Obstcls, position, rotation);
        go.transform.SetParent(transform);

        //Add the object to the list
        if (_objList.AddItem(go, false))
        {
            _lastSpawnPosZ = z;
        }
        else
        {
            Destroy(go);
        }
    }

    private void SpawnPortal()
    {
        if (NextLevelToLoad == "None") return;

        //Portal
        var go = Instantiate(
            Portal,
            new Vector3(Portal.transform.position.x, Portal.transform.position.y, PortalSpawnPosition),
            Portal.transform.rotation
        );
        go.name = NextLevelToLoad;
        go.transform.SetParent(transform);

        //Particles
        var go1 = Instantiate(
            Particles,
            new Vector3(Particles.transform.position.x, Particles.transform.position.y, PortalSpawnPosition),
            Particles.transform.rotation
        );
        go1.transform.SetParent(transform);
    }

    private void SpawnCollectibles()
    {
        //Defining the Object
        var index = DetermineNextCollectible();

        var z = _lastSpawnPosZCollectibles + Random.Range(MinDistance, MaxDistance);
		if (z >= PortalSpawnPosition - MaxDistance) return;

		var x = DetermineNextLanePos();

        var gos = new List<GameObject>();

		if (Collectibles[index].CanGroup && Random.Range(0, 2) == 1)
        {
			float lastPosZ = 0;

			for (int i = 0; i < 6; i++)
            {
                if (i == 0)
                {
                    lastPosZ = z;
                }
                else
                {
                    lastPosZ += 1.5f;
                }

                var go = Instantiate(
                    Collectibles[index].Cllbls,
                    new Vector3(x, Collectibles[index].Cllbls.transform.position.y, lastPosZ),
                    Collectibles[index].Cllbls.transform.rotation
                );

                if (!GetComponent<ObjectList>().IsPositionUsed(go.transform, true))
                {
                    gos.Add(go);
		            go.GetComponent<PointController>().SetStartValue(_billowOffset);
					_billowOffset = (_billowOffset == 0) ? _groupMembers[_groupMembers.Length - 1] : _billowOffset - 1;
				}
				else
                {
                    Destroy(go);
                    break;
                }
            }

			z = lastPosZ;
        }
        else
        {
            gos.Add(Instantiate(
                Collectibles[index].Cllbls,
                new Vector3(x, Collectibles[index].Cllbls.transform.position.y, z),
                Collectibles[index].Cllbls.transform.rotation
            ));
        }

        foreach (var go in gos)
        {
            go.transform.SetParent(transform);
            //Add the object to the list
            if (_objList.AddItem(go, true))
            {
                _lastSpawnPosZCollectibles = z;
            }
            else
            {
                Destroy(go);
            }
        }

        gos.Clear();
    }

    private int DetermineNextObstacle()
    {
        return Random.Range(0, Obstacles.Length);
    }

    private int DetermineNextCollectible()
    {
		var index = Random.Range(1, Collectibles.Length);
		return Random.Range(0f, 1f) <= 0.7f ? 0 : index;

	}

    private static int DetermineNextLanePos()
    {
        var randomLineIdx = Random.Range(0, GlobalDataHandler.GetLanePositions().Length);
        return GlobalDataHandler.GetLanePositions()[randomLineIdx];
    }

//    private static Quaternion DetermineRandRotation()
//    {
//        return Quaternion.Euler(-90.0f, Random.Range(-0, 360), 0);
//    }

    private void DeleteObjects()
    {
        while (_objList.IsListFull())
        {
            _objList.RemoveLastItem();
        }
    }
}