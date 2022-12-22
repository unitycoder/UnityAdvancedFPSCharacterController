using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISettings : MonoBehaviour
{
    public static UISettings Instance;
    public PlayerManager playerManager;
    public Toggle toggleCrouch;
    public Toggle invertYAxis;
    public Toggle reduceMotion;
    public Toggle moveCamera;
    public Toggle bobX;
    public Toggle bobY;
    public Slider sensX;
    public Slider sensY;
    public Slider smoothing;
    private void Awake() {
        Instance = this;
    }
    private void OnEnable()
    {
        toggleCrouch.onValueChanged.AddListener(delegate
            {
                ToggleCallback(toggleCrouch);
            }
        );
        invertYAxis.onValueChanged.AddListener(delegate
            {
                ToggleCallback(invertYAxis);
            }
        );
        reduceMotion.onValueChanged.AddListener(delegate
            {
                ToggleCallback(reduceMotion);
            }
        );
        moveCamera.onValueChanged.AddListener(delegate
            {
                ToggleCallback(moveCamera);
            }
        );
        bobX.onValueChanged.AddListener(delegate
            {
                ToggleCallback(bobX);
            }
        );
        bobY.onValueChanged.AddListener(delegate
            {
                ToggleCallback(bobY);
            }
        );
        sensX.onValueChanged.AddListener(delegate
            {
                SliderCallback(sensX, sensX.value);
            }
        );
        sensY.onValueChanged.AddListener(delegate
            {
                SliderCallback(sensY, sensY.value);
            }
        );
        smoothing.onValueChanged.AddListener(delegate
            {
                SliderCallback(smoothing, smoothing.value);
            }
        );
    }
    private void ToggleCallback(Toggle selectedToggle)
    {
        if (selectedToggle == toggleCrouch)
        {
            playerManager.playerMovement.toggleCrouch = toggleCrouch.isOn;
        }
        if (selectedToggle == invertYAxis)
        {
            playerManager.cameraManager.invertYAxis = invertYAxis.isOn;
        }
        if (selectedToggle == reduceMotion)
        {
            playerManager.cameraManager.reduceMotion = reduceMotion.isOn;
        }
        if (selectedToggle == moveCamera)
        {
            playerManager.cameraManager.moveCamera = moveCamera.isOn;
        }
        if (selectedToggle == bobX)
        {
            playerManager.cameraManager.moveBobX = bobX.isOn;
        }
        if (selectedToggle == bobX)
        {
            playerManager.cameraManager.moveBobY = bobY.isOn;
        }
    }

    private void SliderCallback(Slider selectedSlider, float value)
    {
        if(selectedSlider == sensX)
        {
            playerManager.cameraManager.sensX = value;
        }

        if(selectedSlider == sensY)
        {
            playerManager.cameraManager.sensY = value;
        }

        if(selectedSlider == smoothing)
        {
            playerManager.cameraController.damp = value;
        }
    }
}
