using UnityEngine;

public class LightAdjuster : MonoBehaviour
{
    [SerializeField] Transform lightGizmo;
    [SerializeField] Transform adjustSphere;

    [SerializeField] Light directionalLight;
    [SerializeField] Transform mainCamera;

    private void OnValidate()
    {
        if(directionalLight == null)
        {
            var lights = FindObjectsByType<Light>(FindObjectsSortMode.None);
            foreach (var item in lights)
            {
                if(item.type == LightType.Directional)
                {
                    directionalLight = item;
                    break;
                }
            }
        }

        if(mainCamera == null)
        {
            mainCamera = Camera.main.transform;
        }
    }

    private void OnEnable()
    {
        if(directionalLight != null)
        {
            lightGizmo.rotation = directionalLight.transform.rotation;
            adjustSphere.position = lightGizmo.position - lightGizmo.forward * 0.1f;
        }
    }

    private void Update()
    {
        if(lightGizmo && mainCamera)
        {
            Vector3 direction = (lightGizmo.position - adjustSphere.position).normalized;
            lightGizmo.forward = direction;
            directionalLight.transform.forward = direction;
        }
    }
}