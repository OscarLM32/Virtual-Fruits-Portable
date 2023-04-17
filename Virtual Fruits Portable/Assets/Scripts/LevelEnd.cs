using System.Collections;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    private IEnumerator OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == (int)LayerValues.PlayerLayer)
        {
            GetComponent<Animator>().enabled = true;
            //AudioManager.Instance.Play(gameObject,SoundList.Victory);
            yield return new WaitForSeconds(1f);
            GameplayEvents.LevelEndReached?.Invoke();
        }
    }
}
