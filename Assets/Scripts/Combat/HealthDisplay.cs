using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour {
    [SerializeField] private Health health = null;
    [SerializeField] private GameObject healthBarParent = null;
    [SerializeField] private Image healthBarImage = null;

    private void Awake() {
        health.ClientOnHealthChanged += HandleHealthChanged;
    }

    private void OnDestroy() {
        health.ClientOnHealthChanged -= HandleHealthChanged;
    }

    private void OnMouseEnter() {
        healthBarParent.SetActive(true);
    }

    private void OnMouseExit() {
        healthBarParent.SetActive(false);
    }

    private void HandleHealthChanged(int currentHealth, int maxHealth) {
        healthBarImage.fillAmount = (float)currentHealth / maxHealth;
    }
}