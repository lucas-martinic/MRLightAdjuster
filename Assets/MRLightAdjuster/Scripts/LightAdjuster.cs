using UnityEngine;

public class LightAdjuster : MonoBehaviour
{
    [SerializeField] Transform lightGizmo;
    [SerializeField] Transform lightIcon;
    [SerializeField] Transform adjustSphere;
    [SerializeField] Transform sliders;

    [SerializeField] Slider3D intensitySlider;
    [SerializeField] Slider3D warmthSlider;
    [SerializeField] Slider3D shadowSlider;

    [SerializeField] int numberOfClicksToOpen = 2;
    private int clickCounter;

    [Header("Automatic assign")]
    [SerializeField] Light directionalLight;
    [SerializeField] Transform leftHandAnchor;
    [SerializeField] Transform mainCamera;

    private bool isAdjusterOpen;
    private void Awake()
    {
        OpenAdjuster(false);
        intensitySlider.OnValueChanged += IntensityChanged;
        warmthSlider.OnValueChanged += WarmthChanged;
        shadowSlider.OnValueChanged += ShadowChanged;
    }

    private void IntensityChanged(float newValue)
    {
        directionalLight.intensity = newValue;
    }

    private void ShadowChanged(float newValue)
    {
        directionalLight.shadowStrength = newValue;
    }

    private void WarmthChanged(float newValue)
    {
        directionalLight.colorTemperature = Mathf.Clamp(newValue * 20000f, 1500f, 20000f);
    }

    //Automatically find for required elements in the scene.
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
                    directionalLight.useColorTemperature = true;
                    break;
                }
            }
        }
        if(mainCamera == null)
        {
            mainCamera = Camera.main.transform;
        }
        if(leftHandAnchor == null)
        {
            leftHandAnchor = GameObject.Find("LeftHandAnchor").transform;
            if (!leftHandAnchor) Debug.LogWarning("There's no rig in the scene.");
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
        //Open/close adjuster
        if (OVRInput.GetDown(OVRInput.Button.Start, OVRInput.Controller.Hands))
        {
            clickCounter++;
            if(clickCounter >= numberOfClicksToOpen)
            {
                OpenAdjuster(!isAdjusterOpen);
                clickCounter = 0;
            }
        }

        if (lightGizmo && adjustSphere)
        {
            Vector3 direction = (lightGizmo.position - adjustSphere.position).normalized;
            lightGizmo.forward = direction;
            directionalLight.transform.forward = direction;
        }
        if(lightIcon && mainCamera)
        {
            Vector3 direction = (mainCamera.position - lightIcon.position).normalized;
            lightIcon.forward = direction;
        }
    }

    private void OpenAdjuster(bool open)
    {
        isAdjusterOpen = open;
        if (open)
        {
            transform.position = leftHandAnchor.transform.position;
            transform.position += Vector3.up * 0.2f;
            if (sliders && mainCamera)
            {
                Vector3 direction = (mainCamera.position - lightIcon.position).normalized;
                sliders.forward = Vector3.ProjectOnPlane(-direction, Vector3.up);
            }
        }
        shadowSlider.SetValue(directionalLight.shadowStrength);
        intensitySlider.SetValue(directionalLight.intensity);
        warmthSlider.SetValue(Mathf.Clamp01(directionalLight.colorTemperature / 20000f));
        lightGizmo.gameObject.SetActive(open);
        adjustSphere.gameObject.SetActive(open);
        sliders.gameObject.SetActive(open);
    }

    private void OnDestroy()
    {
        intensitySlider.OnValueChanged -= IntensityChanged;
        warmthSlider.OnValueChanged -= WarmthChanged;
        shadowSlider.OnValueChanged -= ShadowChanged;
    }
}