using TMPro;
using UnityEngine;

namespace Code
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private TMP_Text _ammoText;

        public void UpdateAmmoText(int currentAmmo)
        {
            _ammoText.text = $"{currentAmmo}";
        }

        public void UpdateAmmoText(int currentAmmo, int totalAmmo)
        {
            _ammoText.text = $"{currentAmmo}/{totalAmmo}";
        }
    }
}
