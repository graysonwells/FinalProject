using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour {
    public AudioSource audioSource;
    public AudioClip clip;
    public float volume = 0.5f;

    void OnTriggerStay2D (Collider2D other) {
        RubyController controller = other.GetComponent<RubyController> ();

        if (controller != null) {
            controller.ChangeHealth (-1);
            audioSource.PlayOneShot (clip, volume);
        }
    }

}