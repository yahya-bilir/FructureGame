using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    void Update()
    {
        if (Camera.main == null) return;

        transform.LookAt(Camera.main.transform);

        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }
}