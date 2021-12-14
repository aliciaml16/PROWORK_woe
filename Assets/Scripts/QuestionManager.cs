using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionManager : MonoBehaviour
{
    [Range(0.0f, 100.0f)]
    public float lifetime = 100;
    private float initialBarWidth;
    public Image lifetimeBar; // Lifetime bar (green)

    public Gradient greenGradient;
    public Gradient blueGradient;
    public Gradient whiteGradient;

    private Renderer worldRenderer;
    public Renderer cloudsRenderer;

    void Start()
    {
        initialBarWidth = lifetimeBar.transform.localScale.x;
        worldRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        lifetimeBar.transform.localScale = new Vector3((lifetime * initialBarWidth) / 100, 0.1561234f, 0);
        lifetimeBar.color = greenGradient.Evaluate(lifetime/100);
        worldRenderer.materials[1].SetColor("_Color", greenGradient.Evaluate(lifetime / 100));
        worldRenderer.materials[0].SetColor("_Color", blueGradient.Evaluate(lifetime / 100));
        cloudsRenderer.material.SetColor("_Color", whiteGradient.Evaluate(lifetime / 100));
    }

    private void OnTriggerEnter(Collider other)
    {
        // We detect if we collide with the box and we deactivate it
        if (other.gameObject.CompareTag("box"))
        {
            other.gameObject.SetActive(false);
        }

        // We detect if we collide with the good or bad element
        if (other.gameObject.CompareTag("good"))
        {
            other.gameObject.SetActive(false);

            lifetime += 5; // we add 5 point to the lifetime

            if (lifetime > 100) {
                lifetime = 100;
            }
        } else if (other.gameObject.CompareTag("bad"))
        {
            other.gameObject.SetActive(false);
            lifetime -= 10; // we take 10 point to the lifetime
        }
    }
}
