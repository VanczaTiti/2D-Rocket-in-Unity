using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class control : MonoBehaviour
{
    public float TrustFactor;
    public float EngineMovementSpeed;
    public float WingDragCoefficient;
    public float ForwardDragCoefficient;
    public Text altitudeText;
    public Text driftText;
    public Text velocityYText;
    public Text velocityXText;
    public Text rotationText;
    public Text angleText;
    public Text trustText;



    private Rigidbody2D rb;
    private float trust = 1/100;
    private float engineAngle=0;//rad

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.centerOfMass = new Vector2(0, 0);//átrakja a tömegközéppontot a lokális koordinátarendszerben (0,0) a befoglaló téglalap közepe
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        //Controls
        trust += Input.GetAxis("Vertical") * TrustFactor;
        if (trust > 10) trust = 10;
        if (trust < 0) trust = 0;
        engineAngle -= Input.GetAxis("Horizontal") * EngineMovementSpeed;
        if (engineAngle > 1) engineAngle = 1;
        if (engineAngle < -1) engineAngle = -1;

        //Texts
        altitudeText.text = "Altitude: " + (rb.position.y-25).ToString("#0.00") + " [m]";
        driftText.text = "Drift: " + rb.position.x.ToString("#0.00") + " [m]";
        velocityYText.text = "Velocity Y: " + (rb.velocity.y ).ToString("#0.00") + " [m/s]";
        velocityXText.text = "Velocity X: " + (rb.velocity.x ).ToString("#0.00") + " [m/s]";
        rotationText.text = "Rotation: " + rb.rotation.ToString("#0.00") + "[°]";

        angleText.text = "Engine Angle: " + (engineAngle/Mathf.PI*180).ToString("#0.00") + "[°]";
        trustText.text = "Trust : " + trust.ToString("#0.00");

        //Movement
        float rot = rb.rotation / 180 * Mathf.PI;
        float sinRot = Mathf.Sin(rot);
        float cosRot = Mathf.Cos(rot);
        //motor:
        //erőt és támadási helyét adjuk meg globális koordinátarendszerben:
        rb.AddForceAtPosition(new Vector2(trust * Mathf.Sin(-rot + engineAngle), trust * Mathf.Cos(-rot + engineAngle)), rb.position + new Vector2(25* sinRot, -25 * cosRot)); //rb.position a befoglaló téglalap középpontját adja vissza, rb.rotation a rakéta rotációját szögben
                      
        //Légellenállás:
        //Szárnyra merőleges
        float wingDragForce = WingDragCoefficient * (rb.velocity.x * cosRot  + rb.velocity.y * sinRot ) * Mathf.Abs(rb.velocity.x * cosRot + rb.velocity.y * sinRot);
        rb.AddForceAtPosition(new Vector2(-wingDragForce*cosRot, -wingDragForce * sinRot),rb.position + new Vector2(5 * sinRot+3* cosRot, -5 * cosRot + 3 * sinRot)); //rb.position a befoglaló téglalap középpontját adja vissza, rb.rotation a rakéta rotációját szögben
        //menetirányú légellenállás
        float forwardDragForce = ForwardDragCoefficient * (-rb.velocity.x * sinRot + rb.velocity.y * cosRot) * Mathf.Abs(rb.velocity.x * sinRot - rb.velocity.y * cosRot);
        rb.AddForceAtPosition(new Vector2(forwardDragForce * sinRot, -wingDragForce * cosRot), rb.position); //rb.position a befoglaló téglalap középpontját adja vissza, rb.rotation a rakéta rotációját szögben

    }
}
