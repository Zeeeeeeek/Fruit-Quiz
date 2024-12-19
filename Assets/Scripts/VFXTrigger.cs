using UnityEngine;

public class VFXTrigger : MonoBehaviour
{
    public GameObject vfxPrefab; // Prefab del VFX

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TriggerVFX();
        }
    }

    private void TriggerVFX()
    {
        if (vfxPrefab == null) return;
        var effect = Instantiate(vfxPrefab, transform.position, Quaternion.identity);
        Destroy(effect, 8f);
    }
}