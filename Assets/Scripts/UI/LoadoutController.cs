using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadoutController : MonoBehaviour
{
    public List<Dice> loadout;
    public List<Weapon> stats;
    private int _activeWeapon;
    private int _activeIndex;
    private string _activeAttr;
    public void ShowLoadout() {
        transform.GetChild(0).gameObject.SetActive(true);
        FindObjectOfType<RollerManager>().EnableRolling((dice) => {
            _activeIndex = 0;
            loadout.AddRange(dice);
            _activeWeapon = loadout[0].Roll();
            _activeAttr = loadout[0].attribute;
            stats[_activeWeapon].Equip();
            transform.GetChild(0).gameObject.SetActive(false);
            FindObjectOfType<RollerManager>().EndRolling();
            FindObjectOfType<CombatController>().inCombat = true;
        });
    }

    public void Reload() {
        _activeIndex++;
        if (_activeIndex > loadout.Count)
        {
            _activeIndex = 0;
        }
        _activeWeapon = loadout[_activeIndex].Roll();
        _activeAttr = loadout[_activeIndex].attribute;
        stats[_activeWeapon].Equip();
    }
}
