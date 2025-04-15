using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ZoomDetection : MonoBehaviour
{
    public float camSpeed = 4f;
    public float panSpeed;
    public float minZ = -50f;
    public float maxZ = -25f;

    private PlayerInputs _playerInputs;
    private Coroutine _zoomCoroutine;
    private Transform _camPos;
    private void Awake()
    {
        _playerInputs = new PlayerInputs();
    }

    private void OnEnable()
    {
        _playerInputs.Enable();
        _camPos = Camera.main.transform;
    }

    private void OnDisable()
    {
        _playerInputs.Disable();

    }

    private void Start()
    {
        _playerInputs.Controls.SecondaryTouchContact.started += _ => ZoomStart();
        _playerInputs.Controls.SecondaryTouchContact.canceled += _ => ZoomEnd();

        _playerInputs.Controls.PrimaryFingerPosition.started += _ => PanDetect();

    }

    private void ZoomStart()
    {
        _zoomCoroutine = StartCoroutine(ZoomDetect());
    }

    private void ZoomEnd()
    {
        StopCoroutine(_zoomCoroutine);
    }

    IEnumerator ZoomDetect()
    {
        float previousDistance = 0f, distance = 0f; 

        while (true)
        {
            distance = Vector2.Distance(_playerInputs.Controls.PrimaryFingerPosition.ReadValue<Vector2>(), _playerInputs.Controls.SecondaryFingerPosition.ReadValue<Vector2>());
            //Detection
            //Zoom out
            if(distance > previousDistance )
            {
                Vector3 targetPosition = _camPos.position;
                targetPosition.z += 1;
                _camPos.position = Vector3.Slerp(_camPos.position, targetPosition, camSpeed * Time.deltaTime);
            }

            else if ( distance < previousDistance)
            {
                Vector3 targetPosition = _camPos.position;
                targetPosition.z -= 1;
                _camPos.position = Vector3.Slerp(_camPos.position, targetPosition, camSpeed * Time.deltaTime);
            }
            //keep track of previous distance
            previousDistance = distance;

            yield return null;
        }
    }

    private void LateUpdate()
    {
        Vector3 currentPosition = _camPos.position;
        currentPosition.z = Mathf.Clamp(currentPosition.z, minZ, maxZ); 

        _camPos.position = currentPosition;

    }

    private void PanDetect() {
        
        Vector3 currentPositon = _camPos.position;
        Vector2 fingerPos = _playerInputs.Controls.PrimaryFingerPosition.ReadValue<Vector2>();

        if (fingerPos.x <= -1)
        {
            currentPositon.x = fingerPos.x; 
        }

    }


}
