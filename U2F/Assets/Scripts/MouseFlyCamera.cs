using UnityEngine;



public class MouseFlyCamera : MonoBehaviour
{
    public float flySpeed = 1f;
    public float rotSpeed = 200;
    public int yLimit = 60;
    /// <summary>
    /// 移动、旋转阻尼
    /// </summary>
    public float Dampening = 5.0f;

    private bool recordRot;
    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private Quaternion desiredRotation;

    private Vector3 desiredPos;
    private bool isMouseWheelEnable = true;
    void OnEnable()
    {
        if (!GetComponent<Rigidbody>())
        {
            Rigidbody rig = gameObject.AddComponent<Rigidbody>();
            rig.useGravity = false;
            rig.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rig.freezeRotation = true;
        }
        if (!GetComponent<SphereCollider>())
        {
            SphereCollider col = gameObject.AddComponent<SphereCollider>();
            col.radius = 0.1f;
        }
        GetComponent<SphereCollider>().enabled = true;
        GetComponent<Rigidbody>().detectCollisions = true;
    }
    void OnDisable()
    {
        GetComponent<SphereCollider>().enabled = false;
        GetComponent<Rigidbody>().detectCollisions = false;
    }
    /// <summary>
    /// 消息接收器MessageReceiver,外部通知相机的滚轮开关
    /// </summary>
    /// <param name="isEnable"></param>
    void EnableMouseWheel(bool isEnable)
    {

        isMouseWheelEnable = isEnable;
    }

    /// <summary>
    /// 相机不能穿透other物体
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            desiredPos = contact.point + contact.normal.normalized * (GetComponent<SphereCollider>().radius + 0.01f);
        }
    }

    void Awake()
    {
        desiredPos = transform.position;
        desiredRotation = transform.rotation;
    }

    void LateUpdate()
    {
        #region 键盘WASD移动相机
        if (Input.GetKey(KeyCode.W))
        {
            desiredPos += transform.forward * flySpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            desiredPos += transform.forward * -flySpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            desiredPos += transform.right * -flySpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            desiredPos += transform.right * flySpeed * Time.deltaTime;
        }
        #endregion

        #region 鼠标中键移动相机
        if (isMouseWheelEnable && Input.mouseScrollDelta.y != 0)
        {
            desiredPos += transform.forward * flySpeed * 2 * Input.mouseScrollDelta.y * Time.deltaTime;
        }
        
        #endregion
           
        if (transform.position != desiredPos)
        {
            transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * Dampening);
        }


        #region 鼠标右键旋转相机
        if (Input.GetMouseButton(1))
        {
            if (!recordRot)
            {
                xDeg = transform.rotation.eulerAngles.y;
                yDeg = transform.rotation.eulerAngles.x;
                recordRot = true;
            }
            xDeg += Input.GetAxis("Mouse X") * rotSpeed * Time.deltaTime;
            yDeg -= Input.GetAxis("Mouse Y") * rotSpeed * Time.deltaTime;

            yDeg = ClampYAngle(yDeg, yLimit);
            // set camera desired rotation 
            desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
        }
        else
            recordRot = false;
        #endregion

        if (transform.rotation != desiredRotation)
            transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * Dampening);
    }
    float ClampYAngle(float yAngle, float yLimit)
    {
        if (yAngle > yLimit && yAngle <= 90)
            yAngle = yLimit;
        else if (yAngle > 90 && yAngle < 360 - yLimit)
            yAngle = 360 - yLimit;

        return yAngle;
    }

    public void MoveToDestination(GameObject targetObj)
    {
        float distance = targetObj.GetComponent<Renderer>().bounds.size.magnitude;
        float deltaY = targetObj.GetComponent<Renderer>().bounds.extents.y;
        Vector3 lookPos = targetObj.GetComponent<Renderer>().bounds.center + new Vector3(0, deltaY, 0);

        Vector3 direction = transform.position - targetObj.transform.position;
        desiredPos = targetObj.transform.position + direction.normalized * distance;
        desiredPos.y = 2;

        transform.position = Vector3.Lerp(transform.position, desiredPos, 0.4f);

        desiredRotation = Quaternion.LookRotation(lookPos - desiredPos);

    }
}