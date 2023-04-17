using UnityEngine;

/// <summary>
/// Class that handles the logic of the static environmental hazards the player can find such as spikes, lava...
/// </summary>
public class StaticEnvironmentalHazard : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D col)
    {
        //We only want this environmental hazards to affect the player
        if (col.gameObject.layer == (int) LayerValues.PlayerLayer)
        {
            IKillable killableComponent = col.gameObject.GetComponent<IKillable>();
            killableComponent?.Kill(gameObject);
        }
    }
}
