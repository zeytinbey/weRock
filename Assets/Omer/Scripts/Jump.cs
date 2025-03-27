using UnityEngine;

public class Jump : MonoBehaviour
{
    private Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (anim != null)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                anim.SetTrigger("Jump");
            }
            {
                // W tuþuna basýlý mý?
                bool isPressingW = Input.GetKey(KeyCode.W);

                // Animator parametresini ayarla
                anim.SetBool("Running", isPressingW);

            }
        }
    }
}

