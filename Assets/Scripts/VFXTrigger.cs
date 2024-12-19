using UnityEngine;

public class VFXTrigger : MonoBehaviour
{
    public GameObject vfxPrefab; 

    public void TriggerVFX()
    {
        if (vfxPrefab == null) return;
        var effect = Instantiate(vfxPrefab, transform.position, Quaternion.identity);
        Destroy(effect, 8f);
    }
}