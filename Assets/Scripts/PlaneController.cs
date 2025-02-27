using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlaneController : MonoBehaviour {
    [Header("Plane Stats")]
    [Tooltip("How much the throttle ramps up or down.")]
    public float throttleIncrement = 1f;
    [Tooltip("Maximum engine thrust when at 100% throttle.")]
    public float maxThrust = 300f;
    [Tooltip("How responsive the plane is when rolling, pitching and yawing.")]
    public float responsiveness = 3f;
    [Tooltip("How much lift force this plane generates as it gains speed.")]
    public float lift = 300f;

    public GameObject explosion = null;

    Rigidbody rb;
    [SerializeField] Text text;

    private float throttle;
    private float roll;
    private float pitch;
    private float yaw;

    private float time = 0.0f;
    private float fuelConsumption = 0.2f; // liters/second

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private float responseModifier {
        get {
            return (rb.mass / 5f) * responsiveness;
        }
    }

    private void HandleInputs(){
        roll = Input.GetAxis("Roll");
        pitch = Input.GetAxis("Pitch");
        yaw = Input.GetAxis("Yaw");

        if (Input.GetKey(KeyCode.Space)) {
            throttle += throttleIncrement;
        } else if(Input.GetKey(KeyCode.LeftControl)) {
            throttle -= throttleIncrement;
        }
            
        throttle = Mathf.Clamp(throttle, 0f, 100f);
        
    }

    private void Update() {
        time += Time.deltaTime;
        FuelManager.fuel -= Time.deltaTime * fuelConsumption;
        if(FuelManager.fuel < 0) { // GameOver
            gameObject.SetActive(false); // hides player
            Instantiate(explosion, transform.position, Quaternion.identity); // instantiate particle system
        }
        PlayerPrefs.SetFloat("score", time);
        HandleInputs();
        UpdateStats();
    }

    private void FixedUpdate() {
        rb.AddForce(transform.forward * maxThrust * throttle);
        rb.AddTorque(transform.up * yaw * responseModifier);
        rb.AddTorque(-transform.right * pitch * responseModifier);
        rb.AddTorque(-transform.forward * roll * responseModifier);

        rb.AddForce(Vector3.up * rb.velocity.magnitude * lift);
    }

    private void UpdateStats() {
        text.text = "Throttle: " + throttle.ToString("F0") + "%\n";
        text.text += "Speed: " + (rb.velocity.magnitude * 13.6f).ToString("F0") + "km/h\n";
        text.text += "Altitude: " + transform.position.y.ToString("F0") + " m\n";
        text.text += "Score: " + time.ToString("F1") + "s\n";
        text.text += "Fuel: " + FuelManager.fuel.ToString("F1") + "l";
    }

}
