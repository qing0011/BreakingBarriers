using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponReward : MonoBehaviour
{
    public int[] weaponIds;
    public GameObject getEff;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (weaponIds == null || weaponIds.Length == 0)
        {
            Debug.LogError("WeaponReward：weaponIds 为空！");
            return;
        }

        int index = Random.Range(0, weaponIds.Length);
        int weaponId = weaponIds[index];

        PlayerObj player = other.GetComponent<PlayerObj>();
        if (player == null) return;

        // 换枪 + 满子弹
        player.PickWeapon(weaponId);

        // 特效
        if (getEff != null)
        {
            GameObject eff = Instantiate(getEff, transform.position, transform.rotation);
            AudioSource audioS = eff.GetComponent<AudioSource>();
            if (audioS != null)
            {
                audioS.volume = GameDataMgr.Instance.musicData.soundValue;
                audioS.mute = !GameDataMgr.Instance.musicData.soundOpen;
            }
        }

        Destroy(gameObject);
    }
}