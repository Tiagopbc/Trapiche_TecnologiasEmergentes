using UnityEngine;

public class MovimentoCamera : MonoBehaviour
{
    [Header("Mouse")]
    public float sensibilidadeMouse = 2.0f;

    [Header("Interação")]
    public float distanciaInteracao = 10.0f;

    private float rotacaoX = 0f;
    private float rotacaoY = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 rotacaoAtual = transform.localEulerAngles;

        rotacaoY = rotacaoAtual.y;
        rotacaoX = rotacaoAtual.x;
    }

    private void Update()
    {
        ControlarCamera();
        ControlarInteracao();
    }

    private void ControlarCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensibilidadeMouse;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadeMouse;

        rotacaoY += mouseX;
        rotacaoX -= mouseY;

        rotacaoX = Mathf.Clamp(rotacaoX, -90f, 90f);

        transform.localRotation = Quaternion.Euler(rotacaoX, rotacaoY, 0f);
    }

    private void ControlarInteracao()
    {
        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }

        Ray raio = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(raio, out RaycastHit hit, distanciaInteracao))
        {
            BotaoDeNavegacao botao = hit.collider.GetComponentInParent<BotaoDeNavegacao>();

            if (botao != null)
            {
                botao.Acionar();
                return;
            }

            ControleDeAlavanca alavanca = hit.collider.GetComponentInParent<ControleDeAlavanca>();

            if (alavanca != null)
            {
                alavanca.Interagir();
            }
        }
    }
}