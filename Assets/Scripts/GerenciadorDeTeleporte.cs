using UnityEngine;
using Unity.XR.CoreUtils;

public class GerenciadorDeTeleporte : MonoBehaviour
{
    [Header("Referências do Jogador")]
    public XROrigin xrOrigin;
    public Transform cameraOuJogadorPC;

    [Header("Ponto Inicial")]
    public Transform pontoInicial;

    [Header("Pontos de Destino")]
    public Transform visaoArmazem;
    public Transform visaoChamines;
    public Transform visaoDonaTete;

    [Header("Configuração")]
    public bool iniciarNoPontoInicial = true;
    public bool ajustarRotacaoNoVR = true;

    [Tooltip("Use se a câmera do PC ficar muito baixa ou muito alta após o teleporte.")]
    public float alturaCameraPC = 1.6f;

    private void Awake()
    {
        if (xrOrigin == null)
        {
            xrOrigin = FindFirstObjectByType<XROrigin>();
        }

        if (cameraOuJogadorPC == null && Camera.main != null)
        {
            cameraOuJogadorPC = Camera.main.transform;
        }
    }

    private void Start()
    {
        if (iniciarNoPontoInicial)
        {
            IrParaPontoInicial();
        }
    }

    public void IrParaPontoInicial()
    {
        TeletransportarPara(pontoInicial);
    }

    public void IrParaArmazem()
    {
        TeletransportarPara(visaoArmazem);
    }

    public void IrParaChamines()
    {
        TeletransportarPara(visaoChamines);
    }

    public void IrParaDonaTete()
    {
        TeletransportarPara(visaoDonaTete);
    }

    private void TeletransportarPara(Transform destino)
    {
        if (destino == null)
        {
            Debug.LogWarning("Destino de teleporte não configurado no GerenciadorDeTeleporte.");
            return;
        }

        MoverXROrigin(destino);
        MoverCameraPC(destino);

        Debug.Log("Teletransportado para: " + destino.name);
    }

    private void MoverXROrigin(Transform destino)
    {
        if (xrOrigin == null || xrOrigin.Camera == null)
        {
            return;
        }

        if (ajustarRotacaoNoVR)
        {
            float anguloAtual = xrOrigin.Camera.transform.eulerAngles.y;
            float anguloDestino = destino.eulerAngles.y;
            float diferenca = Mathf.DeltaAngle(anguloAtual, anguloDestino);

            xrOrigin.RotateAroundCameraUsingOriginUp(diferenca);
        }

        Vector3 deslocamentoCamera = xrOrigin.Camera.transform.position - xrOrigin.transform.position;
        deslocamentoCamera.y = 0f;

        xrOrigin.transform.position = destino.position - deslocamentoCamera;
    }

    private void MoverCameraPC(Transform destino)
    {
        if (cameraOuJogadorPC == null)
        {
            return;
        }

        if (xrOrigin != null && cameraOuJogadorPC.IsChildOf(xrOrigin.transform))
        {
            return;
        }

        Vector3 novaPosicao = destino.position + Vector3.up * alturaCameraPC;
        cameraOuJogadorPC.position = novaPosicao;

        Vector3 rotacaoDestino = destino.eulerAngles;
        cameraOuJogadorPC.rotation = Quaternion.Euler(0f, rotacaoDestino.y, 0f);
    }
}