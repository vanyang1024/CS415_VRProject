using UnityEngine;

public class Target : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private int hitPoints = 1;
    public bool isShrinking;
    private float score;
    void Start()
    {
        score = 10;

    }

    // Update is called once per frame
    void Update()
    {
        if (isShrinking)
        {
            transform.localScale -= Vector3.one * Time.deltaTime/2f;
            score -= Time.deltaTime * 5f;
            if (transform.localScale.x <= 0.1f)
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.OnTargetHit(gameObject,0);
                }
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision detected with " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Target")==false)
        {
            hitPoints--;
            if (hitPoints <= 0)
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.OnTargetHit(gameObject,score);
                }
            }
        }
        
    }

    void OnTriggerEnter(Collider collision)
    {
        if (GameManager.Instance != null)
            {
                GameManager.Instance.OnTargetHit(gameObject,0);
            }
           
    }
        
    
}
