using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playerScript : MonoBehaviour
{
    public float playerStartSpeed = 15f;

    private Vector3 startPos;

    float playerFwdSpeed = 15f;
    public float playerFwdAcceleration = 3f;//per second

    public Vector3 playerDuckScale = new Vector3(1, .35f, 1);
    private Vector3 playerOriginalScale = Vector3.one;

    /// <summary>
    /// Max right/left speed
    /// </summary>
    public float playerTurnSpeed = 10f;
    public float playerTurnAcceleration = 30f;
    public float playerTurnSlowDownMultiplier = .3f;
    public float playerSlowSpeed = 10f;
    float currentTurnVelocity = 0;

    public float jumpForce = 20f;

    Rigidbody rb;

    float slowdownCD = 0;
    public float slowdownDuration = 4;//sekunder

    float jmpCD = 0;

    float duckTimer = 0;
    public float duckDuration = 1f;

    public LayerMask gcMask;

    private float preSlowSpeed = 0;
    private float unslowAcceleration = 0;
    public float slowedSpeed = 10f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerFwdSpeed = playerStartSpeed;
        startPos = transform.position;
        playerOriginalScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        bool canJump = Physics.Raycast(transform.position, transform.up * -1, 1f, gcMask);

        

        playerFwdSpeed += playerFwdAcceleration * Time.deltaTime;

        if(jmpCD > 0)
        {
            jmpCD -= Time.deltaTime;
        }

        if(slowdownCD > 0)
        {
            slowdownCD -= Time.deltaTime;
            UnSlow();
        }

        if(duckTimer > 0)
        {
            duckTimer -= Time.deltaTime;
            if(duckTimer <= 0)
            {
                StopDuck();
            }
        }

        if (Input.GetKey(KeyCode.A))//left
        {
            Turn(-1);
        }
        else if (Input.GetKey(KeyCode.D)) //right
        {
            Turn(1);
        }
        else
        {
            Turn(0);
        }

        if (Input.GetKey(KeyCode.Space) && canJump && jmpCD <= 0)//jump
        {
            rb.AddForce(transform.up * jumpForce);
            jmpCD = .5f;
        }

        if (Input.GetKey(KeyCode.LeftShift) && duckTimer <= 0)
        {
            Duck();
        }

        HandleRotation();
        rb.velocity = new Vector3(currentTurnVelocity, rb.velocity.y, playerFwdSpeed);
    }

    private void HandleRotation()
    {
        Vector2 fwd = new Vector2(playerFwdSpeed, currentTurnVelocity);
        fwd.Normalize();

        Vector3 newRotation = new Vector3(0, Mathf.Acos(fwd.x) * Mathf.Rad2Deg * Mathf.Sign(currentTurnVelocity), 0) ;
        transform.rotation = Quaternion.Euler(newRotation);
    }

    private void Turn(int direction)
    {
        if(direction == 0)
        {
            if (currentTurnVelocity == 0)
                return;

            float sign = Mathf.Sign(currentTurnVelocity);

            //Absoluteify
            float tempVelocity = Mathf.Abs(currentTurnVelocity);
            tempVelocity -= playerTurnAcceleration * Time.deltaTime * playerTurnSlowDownMultiplier;
            if(tempVelocity <= 0)
            {
                tempVelocity = 0;
            }

            currentTurnVelocity = sign * tempVelocity;

        }
        else
        {
            currentTurnVelocity += direction * playerTurnAcceleration * Time.deltaTime;
            if(currentTurnVelocity > playerTurnSpeed || currentTurnVelocity < -playerTurnSpeed)
            {
                currentTurnVelocity = Mathf.Sign(currentTurnVelocity) * playerTurnSpeed;
            }
        }
    }

    private void Duck()
    {
        duckTimer = duckDuration;
        transform.localScale = playerDuckScale;
    }

    private void StopDuck()
    {
        duckTimer = 0;
        transform.localScale = playerOriginalScale;
        float height = (playerOriginalScale.y - playerDuckScale.y) / 2;
        transform.position = transform.position + transform.up * height;
    }

    private void Slow()
    {
        slowdownCD = slowdownDuration;
        preSlowSpeed = playerFwdSpeed;
        playerFwdSpeed = slowedSpeed;
        unslowAcceleration = (preSlowSpeed - slowedSpeed) / slowdownDuration;
    }

    /// <summary>
    /// Will have restored the players speed to pre-slow when the slowCD is over.
    /// </summary>
    /// <returns></returns>
    private void UnSlow()
    {
        playerFwdSpeed += unslowAcceleration * Time.deltaTime;
        
    }

    private void Reset()
    {
        //transform.position = startPos;
        //playerFwdSpeed = playerStartSpeed;
        //slowdownCD = 0;
        //jmpCD = 0;
        //duckTimer = 0;
        //StopDuck();

        SceneManager.LoadScene(0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("obstacle"))
        {
            Debug.Log("OW!");
            if(slowdownCD > 0)
            {
                Debug.Log("DEAD");
                Reset();
            }
            else
            {
                Slow();
                Destroy(collision.gameObject);
            }
        }
        else if (collision.gameObject.CompareTag("fullObstacle"))
        {
            Debug.Log("DEAD");
            Reset();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("coin"))
        {
            Destroy(other.gameObject);
        }
    }
}
