using GibFrame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fray.UI
{
    /// <summary>
    ///   Base module to automatically handle the display of cooldowns
    /// </summary>
    public class CooldownUI : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text amountText;
        [SerializeField] private SerializableInterface<ICooldownOwner> preBinded;
        private ICooldownOwner cooldownHolder;

        public void Bind(ICooldownOwner cooldownHolder) => this.cooldownHolder = cooldownHolder;

        private void Awake()
        {
            if (!image)
            {
                image = GetComponent<Image>();
            }
            if (!amountText)
            {
                amountText = GetComponentInChildren<TMP_Text>();
            }
            if (preBinded.Value != null)
            {
                Bind(preBinded.Value);
            }
        }

        private void Update()
        {
            if (cooldownHolder != null)
            {
                image.fillAmount = cooldownHolder.GetCooldownPercentage();
                amountText.text = cooldownHolder is IMultiCooldownOwner multiCooldown && amountText ? multiCooldown.GetResourcesAmount().ToString() : "";
            }
        }
    }
}