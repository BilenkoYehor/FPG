using UnityEngine.UI;
using UnityEngine;

namespace TPSShooter
{

    public class SettingsPreview : MonoBehaviour
    {

        [Header("Toggles")]
        public Toggle autoShootToggle;
        public Toggle soundToggle;

        [Header("Sliders")]
        public Slider touchpadSensitivitySlider;
        public Slider touchpadAimingSensitivitySlider;

        [Header("Sounds")]
        public AudioSource click;
        public AudioSource mainSound;

        [Header("AudioListener")]
        public AudioListener audioListener;

        void Start()
        {
            // sets toggle values
            autoShootToggle.isOn = SaveLoad.IsAutoShoot;
            soundToggle.isOn = SaveLoad.IsSoundOn;

            // sets slider values
            touchpadSensitivitySlider.value = SaveLoad.TouchpadSensitivity;
            touchpadAimingSensitivitySlider.value = SaveLoad.TouchpadAimingSensitivity;
        }

        /// <summary>
        /// Touchpad sensitivity.
        /// </summary>
        public void TouchpadSensitivityChanged()
        {
            SaveLoad.TouchpadSensitivity = touchpadSensitivitySlider.value;
        }

        /// <summary>
        /// Touchpad aiming sensitivity.
        /// </summary>
        public void TouchpadAimingSensitivityChanged()
        {
            SaveLoad.TouchpadAimingSensitivity = touchpadAimingSensitivitySlider.value;
        }
        /// <summary>
        /// AutoShoot.
        /// </summary>
        public void AutoShootToggleChanged()
        {
            SaveLoad.IsAutoShoot = autoShootToggle.isOn;

            click.Play();
        }

        /// <summary>
        /// Sound.
        /// </summary>
        public void SoundToggleChanged()
        {
            SaveLoad.IsSoundOn = soundToggle.isOn;

            if (SaveLoad.IsSoundOn)
            {
                // audioListener.enabled = true;
                AudioListener.volume = 1;
            }
            else
            {
                //  audioListener.enabled = false;
                AudioListener.volume = 0;
            }
            click.Play();
        }
    }
}