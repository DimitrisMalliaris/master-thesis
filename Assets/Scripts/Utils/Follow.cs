using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class Follow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Camera camera;

    [SerializeField] bool rotating = false;
    [SerializeField] float sensitivity = 0.5f;

    private void Start()
    {
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target);

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            transform.Translate(Vector3.forward);
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            transform.Translate(Vector3.back);

        if (Input.GetMouseButtonDown(1))
            rotating = true;

        if (Input.GetMouseButtonUp(1))
            rotating = false;

        if (rotating)
        {
            var distance = Vector3.Distance(transform.position, target.transform.position);

            if (Input.GetAxis("Mouse X") < 0f)
            {
                transform.RotateAround(target.transform.position, transform.up, 2f * sensitivity / distance *  Time.deltaTime);
            }
            else if (Input.GetAxis("Mouse X") > 0f)
            {
                transform.RotateAround(target.transform.position, transform.up, -2f * sensitivity / distance * Time.deltaTime);
            }

            if (Input.GetAxis("Mouse Y") < 0f)
            {
                transform.RotateAround(target.transform.position, transform.right, 2f * sensitivity / distance * Time.deltaTime);
            }
            else if (Input.GetAxis("Mouse Y") > 0f)
            {
                transform.RotateAround(target.transform.position, transform.right, -2f * sensitivity / distance * Time.deltaTime);
            }
        }
    }
}
