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
        FindObjectOfType<CombatUIManager>().Show(false);
        FindObjectOfType<PlayerController>().moveEnabled = false;
        transform.GetChild(0).gameObject.SetActive(true);
        FindObjectOfType<RollerManager>().EnableRolling((dice) => {
            _activeIndex = 0;
            loadout.AddRange(dice);
            Reload();
            transform.GetChild(0).gameObject.SetActive(false);
            FindObjectOfType<CombatUIManager>().Show(true);
            FindObjectOfType<RollerManager>().EndRolling();
            FindObjectOfType<CombatController>().inCombat = true;
            FindObjectOfType<PlayerController>().moveEnabled = true;
            FindObjectOfType<PlayerController>().fireEnabled = true;
        }, false, 6);
    }

    public void HideLoadout() {
        foreach (var die in loadout) {
            // Is it a real die?
            if (die.faces.Count > 0) {
                FindObjectOfType<RollerManager>().AddDie(die);
            }
        }
        loadout.Clear();
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void Reload() {
        // In case there are fewer than 6 dice:
        int startIndex = _activeIndex;
        while (loadout[_activeIndex].faces == null) {
            if (startIndex == _activeIndex && loadout[_activeIndex].faces == null) {
                if (loadout[_activeIndex - 1].faces != null) {
                    _activeIndex--;
                    break;
                }
                Debug.LogError("Error: No dice found.");
                return;
            }
            _activeIndex++;
            if (_activeIndex > loadout.Count)
            {
                _activeIndex = 0;
            }
        }
        // Result is 1-6 (We want 0-5):
        _activeWeapon = loadout[_activeIndex].Roll() - 1;
        _activeAttr = loadout[_activeIndex].attribute;
        stats[_activeWeapon].Equip();

        _activeIndex++;
        if (_activeIndex > loadout.Count)
        {
            _activeIndex = 0;
        }
    }
}
