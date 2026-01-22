using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerRuntimeData
{
    public int level = 1;

    public int attack = 10;
    public int defense = 5;

    public int bulletCount = 50;

    public int maxHp = 100;
    public int hp = 100;
    // 当前装备的武器 ID
    public int weaponId = -1; // -1 = 还没拿过枪

}

