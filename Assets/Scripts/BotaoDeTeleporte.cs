using UnityEngine;

public class BotaoDeTeleporte : MonoBehaviour
{
    public enum DestinoTeleporte
    {
        Armazem,
        Chamines,
        DonaTete
    }

    [Header("Referências")]
    public GerenciadorDeTeleporte gerenciadorDeTeleporte;

    [Header("Destino")]
    public DestinoTeleporte destino;

    [Header("Controle de Clique")]
    public float tempoEntreCliques = 0.5f;

    private bool podeAcionar = true;

    private void OnMouseDown()
    {
        Acionar();
    }

    public void Acionar()
    {
        if (!podeAcionar)
        {
            return;
        }

        if (gerenciadorDeTeleporte == null)
        {
            Debug.LogWarning("GerenciadorDeTeleporte não configurado no botão: " + gameObject.name);
            return;
        }

        podeAcionar = false;

        switch (destino)
        {
            case DestinoTeleporte.Armazem:
                gerenciadorDeTeleporte.IrParaArmazem();
                break;

            case DestinoTeleporte.Chamines:
                gerenciadorDeTeleporte.IrParaChamines();
                break;

            case DestinoTeleporte.DonaTete:
                gerenciadorDeTeleporte.IrParaDonaTete();
                break;
        }

        Invoke(nameof(LiberarClique), tempoEntreCliques);
    }

    private void LiberarClique()
    {
        podeAcionar = true;
    }
}