using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waves : MonoBehaviour {


    public int playerID = -1;
    public PersistentData.DIFFICULTY difficulty = PersistentData.DIFFICULTY.MEDIUM;

    public List<Building> productionBuildings;
    public GameObject rallyPoint;

    [Header("Difficulty settings")]
    public int[] secsBeforeFirstWave = new int[3];
    public int[] produceUnitPerSecs = new int[3];
    public int[] unitsFirstWave = new int[3];
    public int[] extraUnitsPerWave = new int[3];

    private bool firstWave = true;
    private int unitsCurrentWave = 0;
    private int unitsPerWave = 0;
    float timeSinceLastWave = 0.0f;
    float timeSinceLastProduction = 0.0f;

    Transform _rallyPointTransform;

    private List<UnitStateController> units = new List<UnitStateController>();

    private void Awake()
    {
        if(PersistentData.instance != null)
        {
            difficulty = PersistentData.instance.difficulty;
        }

        unitsPerWave = unitsFirstWave[(int)difficulty];
        _rallyPointTransform = rallyPoint.transform;

        foreach(Building b in productionBuildings)
        {
            b.belongsToWavesAI = this;
        }
    }

    void Update()
    {
        timeSinceLastProduction += Time.deltaTime;
        if (timeSinceLastProduction >= produceUnitPerSecs[(int)difficulty] && unitsCurrentWave < unitsPerWave)
        {
            timeSinceLastProduction = 0.0f;
            ProduceUnit();
        }

        if (firstWave)
        {
            timeSinceLastWave += Time.deltaTime;

            if (timeSinceLastWave >= secsBeforeFirstWave[(int)difficulty] && unitsCurrentWave >= unitsPerWave)
            {
                firstWave = false;
                SendWave();
            }
        }

        else
        {
            if (unitsCurrentWave >= unitsPerWave)
            {
                SendWave();
            }
        }

        UpdateUnitsInWave();
    }

    public void ProduceUnit()
    {
        int bIndex = (int)Mathf.FloorToInt(Random.value * productionBuildings.Count);
        Building building = productionBuildings[bIndex];
        building.rallyPointPos = new Vector3(_rallyPointTransform.position.x - 0.5f + (Random.value * 1f), _rallyPointTransform.position.y - 0.5f + (Random.value * 1f), 0.0f);

        int unitIndex = (int)Mathf.Floor(Random.value * building.productionButtonsData.Length);
        building.ProduceInstantly(unitIndex);
    }

    public void AddUnitToWave(UnitStateController unit)
    {
        units.Add(unit);
        unit.belongsToWavesAI = this;
        unitsCurrentWave++;
    }

    public void SendWave()
    {
        timeSinceLastWave = 0.0f;
        unitsCurrentWave = 0;
        unitsPerWave += extraUnitsPerWave[(int)difficulty];

        foreach (UnitStateController unit in units)
        {
            unit.waveAttackMode = true;

            if (unit.currentState == unit.idleState)
            {
                AttackPlayer(unit);
            }
        }
    }

    public void UpdateUnitsInWave()
    {
        foreach (UnitStateController unit in units)
        {
            if (unit.currentState == unit.idleState)
            {
                if(unit.waveAttackMode == true)
                {
                    AttackPlayer(unit);
                }
            }
        }
    }

    void AttackPlayer(UnitStateController unit)
    {
        if(PlayerManager.instance.myCivilizationCenter)
        {
            unit.MoveToInAttackMode(PlayerManager.instance.myCivilizationCenter._transform.position);
        }
    }

    public void RemoveUnit(UnitStateController unit)
    {
        units.Remove(unit);
    }

    public void RemoveBuilding(Building building)
    {
        productionBuildings.Remove(building);
    }
}
