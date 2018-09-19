using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float speed = 4f;
    private void Start()
    {
        Camera.main.transform.position = transform.position - transform.forward * 4 + Vector3.up * 2.5f;
        Camera.main.transform.parent = transform;
        ClientController.Instance.StartUpdateCoroutine();
    }
    private void Update()
    {
        transform.Translate(Input.GetAxis("Vertical") * transform.forward * speed * Time.deltaTime + Input.GetAxis("Horizontal") * transform.right * speed * Time.deltaTime, Space.World);

        if (Input.GetKey(KeyCode.Q))
            transform.Rotate(transform.up, -16 * speed * Time.deltaTime);
        if (Input.GetKey(KeyCode.E))
            transform.Rotate(transform.up, 16 * speed * Time.deltaTime);
    }


    public void Kill()
    {
        Camera.main.transform.parent = null;
        Destroy(gameObject);
    }
}
