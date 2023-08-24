using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.OpenXR.Input;

public class HapticOutput : MonoBehaviour
{

    public List<UnityEngine.XR.InputDevice> devices = new List<UnityEngine.XR.InputDevice>();

    private List<UnityEngine.XR.InputDevice> hapticControllers = new List<UnityEngine.XR.InputDevice>();
    private bool devicesFound => hapticControllers.Count > 0;

    private void Update()
    {
        if (!devicesFound)
        {
            UnityEngine.XR.InputDevices.GetDevices(devices);

            foreach (var device in devices)
            {
                UnityEngine.XR.HapticCapabilities capabilities;
                if (device.TryGetHapticCapabilities(out capabilities))
                {
                    if (capabilities.supportsImpulse)
                    {
                        hapticControllers.Add(device);
                    }
                }
            }
        }
        

    }
    public void SendHapticResponse(float amplitude, float duration)
    {

        foreach (var device in hapticControllers)
        {
            uint channel = 0;
            device.SendHapticImpulse(channel, amplitude, duration);
      
        }
    }
}
