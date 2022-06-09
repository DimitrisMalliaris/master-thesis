using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] Agent _Agent;

    [SerializeField] float MaxHealth = 100f;
    [SerializeField] float CurrentHealth = 100f;
    [SerializeField] float HealthRegen = 1f;
    [SerializeField] float HealthRegenInterval = 1f;
    [SerializeField] bool CanHeal = true;
    [SerializeField] public bool IsAlive = true;

    [SerializeField] int HealingCooldownDamageMultiplier = 3;
    [SerializeField] int HealingCooldownMultiplier = 0;

    [Header("StatusUI")]
    public Image HealthBar;
    public ParticleSystem BloodParticleSystem;

    private void Start()
    {
        _Agent = GetComponent<Agent>();
    }

    private void FixedUpdate()
    {
        // Check if alive & has missing health 
        if (IsAlive && CurrentHealth < 100)
            Heal(HealthRegen);
    }

    private void OnHealthChanged()
    {
        if (_Agent.DebugMode)
            Debug.Log($"<color=red> {gameObject.name} </color> Health changed");

        // Update UI
        if (HealthBar)
            HealthBar.fillAmount = CurrentHealth / MaxHealth;
        else if (_Agent.DebugMode)
            Debug.LogWarning($"{transform.name}'s healthbar not set");

        if (HealthBar.fillAmount <= .5f)
            HealthBar.color = Color.red;
        else
            HealthBar.color = Color.green;
    }

    public void TakeDamage(float incomingDamage)
    {
        // Check if alive
        if (!IsAlive)
            return;

        // Trigger particles
        if(BloodParticleSystem)
            BloodParticleSystem.Play();

        // Change health
        CurrentHealth = Mathf.Max(CurrentHealth - incomingDamage, 0f);
        OnHealthChanged();

        // Check if dead
        if (CurrentHealth <= 0f)
            Die();

        OnHealingCooldown(HealingCooldownDamageMultiplier);
    }

    public void Heal(float incomingHealing)
    {
        // Check if healing on cooldown & alive
        if (!CanHeal || !IsAlive)
            return;

        // Change health
        CurrentHealth = Mathf.Min(CurrentHealth + incomingHealing, 100f);
        OnHealthChanged();

        // Regen on cooldown
        OnHealingCooldown();
    }

    private void Die()
    {
        IsAlive = false;
        _Agent.Die();
    }

    private void HealingCooldownTick()
    {
        HealingCooldownMultiplier = Mathf.Max(HealingCooldownMultiplier - 1, 0);

        if(HealingCooldownMultiplier > 0)
            Invoke(nameof(HealingCooldownTick), HealthRegenInterval);
        else
            CanHeal = true;
    }

    private void OnHealingCooldown(int multiplier = 1)
    {
        if(HealingCooldownMultiplier < multiplier)
            HealingCooldownMultiplier = multiplier;

        // if already on cooldown
        if (!CanHeal)
            return;

        CanHeal = false;
        Invoke(nameof(HealingCooldownTick), HealthRegenInterval);
    }

    // Debug damage on click
    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0) && _Agent.DebugMode)
            TakeDamage(10);
    }
}
