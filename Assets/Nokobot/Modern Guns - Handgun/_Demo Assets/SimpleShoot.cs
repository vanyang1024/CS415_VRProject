using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityEngine.XR.Interaction.Toolkit.Interactables
{
[AddComponentMenu("Nokobot/Modern Guns/Simple Shoot")]
public class SimpleShoot : MonoBehaviour
{
    [Header("Prefab Refrences")]
    public GameObject bulletPrefab;
    public GameObject casingPrefab;
    public GameObject muzzleFlashPrefab;

    [Header("Location Refrences")]
    [SerializeField] private Animator gunAnimator;
    [SerializeField] private Transform barrelLocation;
    [SerializeField] private Transform casingExitLocation;

    [Header("Settings")]
    [Tooltip("Specify time to destory the casing object")] [SerializeField] private float destroyTimer = 2f;
    [Tooltip("Bullet Speed")] [SerializeField] private float shotPower = 500f;
    [Tooltip("Casing Ejection Speed")] [SerializeField] private float ejectPower = 150f;
    [Tooltip("The Line Renderer component on this GameObject.")]public LineRenderer lineRenderer;

    [SerializeField] public LayerMask hitMask;

    private XRGrabInteractable grabInteractable;

    void Start()
    {
        if (barrelLocation == null)
            barrelLocation = transform;

        if (gunAnimator == null)
            gunAnimator = GetComponentInChildren<Animator>();
        
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.enabled = false;
            lineRenderer.startWidth = 0.005f;
            lineRenderer.endWidth = 0.005f;
            SetLineColor(Color.red);
        
        grabInteractable = transform.parent.parent.GetComponent<XRGrabInteractable>();
    }



    public void SetLineColor(Color color)
    {
        // Create a new Gradient
        Gradient gradient = new Gradient();
        
        // Define the color keys (start and end color of the line)
        // We set both to the same bright red color
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        
        // Key 0: Start of the line (Time = 0)
        colorKeys[0] = new GradientColorKey(color, 0.0f);
        
        // Key 1: End of the line (Time = 1)
        colorKeys[1] = new GradientColorKey(color, 1.0f);

        // Define the alpha (opacity) keys (optional, usually set to fully opaque)
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0] = new GradientAlphaKey(1.0f, 0.0f); // Fully opaque at start
        alphaKeys[1] = new GradientAlphaKey(1.0f, 1.0f); // Fully opaque at end

        // Apply the keys to the gradient
        gradient.SetKeys(colorKeys, alphaKeys);

        // Assign the new gradient to the Line Renderer
        lineRenderer.colorGradient = gradient;
        
        // For a bright red, ensure the color you pass in is bright, 
        // e.g., Color.red or new Color(1f, 0f, 0f, 1f)
    }

    void Update()
    {
        //If you want a different input, change it here
        if (Input.GetButtonDown("Fire1")&& grabInteractable.isSelected)
        {
            //Calls animation on the gun that has the relevant animation events that will fire
            gunAnimator.SetTrigger("Fire");
        }
        
        if(Physics.Raycast(barrelLocation.position, barrelLocation.forward , out RaycastHit hitInfo , hitMask))
        {
            Debug.DrawRay(barrelLocation.position, barrelLocation.forward * hitInfo.distance, Color.red);
            Vector3 rayEndPoint = hitInfo.point;
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, barrelLocation.position); // Start point
            lineRenderer.SetPosition(1, rayEndPoint); // End point
        }
        else
        {
            lineRenderer.enabled = false;
        }

    }


    //This function creates the bullet behavior
    void Shoot()
    {
        if (muzzleFlashPrefab)
        {
            //Create the muzzle flash
            GameObject tempFlash;
            tempFlash = Instantiate(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation);

            //Destroy the muzzle flash effect
            Destroy(tempFlash, destroyTimer);
        }

        //cancels if there's no bullet prefeb
        if (!bulletPrefab)
        { return; }

        // Create a bullet and add force on it in direction of the barrel
        Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation).GetComponent<Rigidbody>().AddForce(barrelLocation.forward * shotPower);

    }

    //This function creates a casing at the ejection slot
    void CasingRelease()
    {
        //Cancels function if ejection slot hasn't been set or there's no casing
        if (!casingExitLocation || !casingPrefab)
        { return; }

        //Create the casing
        GameObject tempCasing;
        tempCasing = Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation) as GameObject;
        //Add force on casing to push it out
        tempCasing.GetComponent<Rigidbody>().AddExplosionForce(Random.Range(ejectPower * 0.7f, ejectPower), (casingExitLocation.position - casingExitLocation.right * 0.3f - casingExitLocation.up * 0.6f), 1f);
        //Add torque to make casing spin in random direction
        tempCasing.GetComponent<Rigidbody>().AddTorque(new Vector3(0, Random.Range(100f, 500f), Random.Range(100f, 1000f)), ForceMode.Impulse);

        //Destroy casing after X seconds
        Destroy(tempCasing, destroyTimer);
    }

}
}