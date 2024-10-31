using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PowerupJump : MonoBehaviour
{
    private Light2D light;
    public enum PowerupType
    {
        Speed,
        Light,
        Dash,
        Jump
    }
    public PowerupType type;
    public float powerUpDuration = 5f; // Duration of the speed boost

    private void Start()
    {
        light = GetComponent<Light2D>();
    }

    public void ResetPowerup()
    {
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            switch (type)
            {
                case PowerupType.Speed:
                    player.ActivateSpeedBoost(light.color);
                    break;
                case PowerupType.Light:
                    player.ActivateLightBoost(light.color);
                    break;
                case PowerupType.Dash:
                    player.ActivateDashBoost(light.color);
                    break;
                case PowerupType.Jump:
                    player.ActivateJumpBoost(light.color);
                    break;
            }
            gameObject.SetActive(false);
        }
    }
}
