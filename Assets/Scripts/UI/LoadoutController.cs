using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutController : MonoBehaviour
{
    public List<Dice> loadout;
    public List<Weapon> stats;
    private int _activeWeapon;
    private int _activeIndex;
    public string activeAttr;
    public void ShowLoadout() {
        FindObjectOfType<CombatUIManager>().Show(false);
        FindObjectOfType<PlayerController>().moveEnabled = false;
        transform.GetChild(0).gameObject.SetActive(true);
        FindObjectOfType<RollerManager>().EnableRolling((dice) => {
            _activeIndex = 0;
            foreach (var die in dice) {
                if (die.faces != null) 
                {
                    loadout.Add(die);
                }
            }
            _activeWeapon = loadout[_activeIndex].attachedDragAndDrop.face - 1;
            activeAttr = loadout[_activeIndex].attribute;
            FindObjectOfType<CombatUIManager>().transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Dice Assets/" + (loadout[_activeIndex].attribute != "" ? loadout[_activeIndex].attribute : "white") + "_Die_" + (_activeWeapon + 1));
            stats[_activeWeapon].Equip();
            _activeIndex++;
            if (_activeIndex >= loadout.Count)
            {
                _activeIndex = 0;
            }
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
        // Result is 1-6 (We want 0-5):
        _activeWeapon = loadout[_activeIndex].Roll() - 1;
        activeAttr = loadout[_activeIndex].attribute;
        FindObjectOfType<CombatUIManager>().transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Dice Assets/" + (loadout[_activeIndex].attribute != ""? loadout[_activeIndex].attribute : "white") + "_Die_" + (_activeWeapon + 1));
        stats[_activeWeapon].Equip();

        _activeIndex++;
        if (_activeIndex >= loadout.Count)
        {
            _activeIndex = 0;
        }
    }
}
