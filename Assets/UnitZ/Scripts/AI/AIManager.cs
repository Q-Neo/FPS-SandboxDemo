

/// <summary>
// This class made for reducing call of FindGameObjectsWithTag function in every AI objects
// FindGameObjectsWithTag is too much cost in update loop.
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AIManager : MonoBehaviour
{
    public Dictionary<string, TargetCollector> TargetList = new Dictionary<string, TargetCollector>();
    public int TargetTypeCount = 0;
    public float UpdateInterval = 0.1f;
    private float timeTmp;
    public string PlayerTag = "Player";

    void Start()
    {
        TargetList = new Dictionary<string, TargetCollector>();
    }
    //在对象启动时调用，初始化TargetList字典。
    [HideInInspector]
    public float scaleSize = 10;
    [HideInInspector]
    public Vector3 MapOffset;
    [HideInInspector]
    public float MapSize;
    //成员变量和属性：
    //TargetList：存储目标的字典，键为目标标签，值为目标收集器。
    //TargetTypeCount：目标类型的计数器。
    //UpdateInterval：更新间隔，控制目标检测的频率。
    //timeTmp：用于跟踪时间的临时变量。
    //PlayerTag：玩家标签，默认为"Player"。
    //scaleSize、MapOffset、MapSize：用于地图坐标转换的参数。
    public Vector3 GetPositionFromIndex(int index)
    {
        float z = index / MapSize;
        float x = index % MapSize;
        return (MapOffset + (new Vector3(x - (MapSize / 2), 0, z - (MapSize / 2)))) / scaleSize;
    }

    public int GetIndexFromPosition(Vector3 pos)
    {
        pos = pos * scaleSize;
        pos = new Vector3(pos.x + (MapSize / 2), 0, pos.z + (MapSize / 2));
        return (int)(((int)pos.z * MapSize) + (int)pos.x);
    }
    //分别用于根据索引获取位置和根据位置获取索引，实现了地图坐标的转换。
    public void Clear()
    {
        foreach (var target in TargetList)
        {
            if (target.Value != null)
                target.Value.Clear();
        }
        TargetList.Clear();
        TargetList = new Dictionary<string, TargetCollector>(0);
    }
    //清空目标列表。
    public TargetCollector FindTargetTag(string tag)
    {

        if (TargetList.ContainsKey(tag))
        {
            TargetCollector targetcollector;
            if (TargetList.TryGetValue(tag, out targetcollector))
            {
                targetcollector.IsActive = true;
                return targetcollector;
            }
            else
            {
                return null;
            }
        }
        else
        {
            TargetList.Add(tag, new TargetCollector(tag));
        }
        return null;
    }
    //根据标签查找目标收集器，若存在则将其激活，若不存在则创建一个新的目标收集器。
    void Update()
    {
        if (Time.time > timeTmp + UpdateInterval)
        {
            int count = 0;

            foreach (var target in TargetList)
            {
                if (target.Value != null)
                {
                    if (target.Value.IsActive)
                    {
                        target.Value.SetTarget(target.Key);
                        target.Value.IsActive = false;
                        count += 1;
                    }
                }
            }
            TargetTypeCount = count;
            timeTmp = Time.time;
        }
    }
    //在每帧更新时检查目标列表，根据是否激活来设置目标并更新目标类型计数。
    public bool IsPlayerAround(Vector3 position, float distance)
    {
        TargetCollector player = FindTargetTag(PlayerTag);
        if (player != null && player.Targets.Length > 0)
        {
            for (int i = 0; i < player.Targets.Length; i++)
            {
                if (player.Targets[i] != null)
                {
                    if (Vector3.Distance(position, player.Targets[i].transform.position) <= distance)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

}
//检查玩家是否在指定位置附近，通过查找玩家标签来实现。

public class TargetCollector
{
    public GameObject[] Targets;
    public bool IsActive;

    public TargetCollector(string tag)
    {
        SetTarget(tag);
    }
    public void Clear()
    {
        Targets = null;
    }
    public void SetTarget(string tag)
    {
        Targets = (GameObject[])GameObject.FindGameObjectsWithTag(tag);
    }

}
//用于收集特定标签的目标对象，并提供清空和设置目标的方法
