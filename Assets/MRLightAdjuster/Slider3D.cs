using Oculus.Interaction;
using System;
using UnityEngine;

public class Slider3D : MonoBehaviour
{
    [SerializeField] Transform sphere;
    [SerializeField] OneGrabTranslateTransformer translateTransformer;
    private float lastValue;

    public event Action<float> OnValueChanged;
    public void SetValue(float newValue)
    {
        sphere.localPosition = new Vector3(Mathf.Lerp(translateTransformer.Constraints.MinX.Value, translateTransformer.Constraints.MaxX.Value, newValue), 0, 0);
    }

    private void Update()
    {
        if(lastValue != sphere.localPosition.x)
        {
            OnValueChanged(Mathf.InverseLerp(translateTransformer.Constraints.MinX.Value, translateTransformer.Constraints.MaxX.Value, sphere.localPosition.x));
        }
        lastValue = sphere.localPosition.x;
    }
}
