using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Must Use 'JSON .NET For Unity' in Unity Asset store for storing complex data types like Dictionary etc.
// Or make your own Dictionary using serializable data types such as List

[System.Serializable]
public class GameData
{
    public string lastPlayTime;
    public int playerLevel;
    public int maxHealCount;
    public int statPoints;
    public int attackLevel;
    // ���ݷ� ����
    public int enduranceLevel;
    // �ִ� ü�� �� ����
    public int poiseLevel;
    // ü��(posture) �� �˹� ����
    public int lastSavePoint;
    public bool[] bossDefeat;
    // public SerializableDictionary<string, bool> mapOpened
    // public bool[] abilityGained; ��
    // �Ʒ��� ���� �Լ��� �ۼ��ؼ� Guid�� ���� �� �ִ�. ���� ContextMenu�� ���ؼ� ���� ����ų� Start �Ǵ� Awake���� �ڵ����� ����ǵ��� �������.
    /*[ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }
    
    public void LoadData(GameData data)
    {
        // ���� ������ ����
        data.dictionaryName.TryGetValue(id, out value);
        // do something
        
        // ��� ������ ����
        foreach(KeyValuePair<T1, T2> pair in data.dictionaryName)
        {
            // pair.Key �Ǵ� pair.Value�� ���� Ű�� �����Ϳ� ����
            // do something
        }
    }

    public void SaveData(ref GameData data)
    {
        if (data.dictionaryName.ContainsKey(id))
        {
            data.dictionaryName.Remove(id);
        }
        data.dictionaryName.Add(id, value);
    }
    */

    public GameData()
    {
        this.lastPlayTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        this.playerLevel = 1;
        this.maxHealCount = 3;
        this.statPoints = 0;
        this.attackLevel = 1;
        this.enduranceLevel = 1;
        this.poiseLevel = 1;
        this.lastSavePoint = 0;
        bossDefeat = new bool[System.Enum.GetValues(typeof(BossIndex)).Length];
        // mapOpened = new Dictionary<string, bool>();
    }
}

public enum BossIndex
{
    Chatterbox, StrongHold, SnowWhite, StormBringer, Harvester, WaterDear, Rose
}