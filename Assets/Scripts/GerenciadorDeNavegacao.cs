using System.Collections;
using UnityEngine;
using Unity.XR.CoreUtils;
using TMPro;

public class GerenciadorDeNavegacao : MonoBehaviour
{
    public enum LocalDeNavegacao
    {
        PontoInicial,
        Armazem,
        Chamines,
        DonaTete
    }

    [Header("Referências do Jogador")]
    public XROrigin xrOrigin;
    public Transform cameraOuJogadorPC;

    [Header("Pontos da Experiência")]
    public Transform pontoInicial;
    public Transform visaoArmazem;
    public Transform visaoChamines;
    public Transform visaoDonaTete;

    [Header("Painel de Escolha")]
    public Transform painelDeEscolha;

    [Header("Posições do Painel")]
    public Transform painelAnchorPontoInicial;
    public Transform painelAnchorArmazem;
    public Transform painelAnchorChamines;
    public Transform painelAnchorDonaTete;

    [Header("Botões")]
    public BotaoDeNavegacao botaoArmazem;
    public BotaoDeNavegacao botaoChamines;
    public BotaoDeNavegacao botaoDonaTete;

    [Header("Texto Informativo")]
    public TMP_Text textoInformativo;

    [TextArea(4, 8)]
    public string textoPontoInicial =
        "<b>Bem-vindo ao Trapiche Histórico!</b>\n" +
        "Você está prestes a embarcar em uma jornada através do tempo. Este antigo porto já foi o coração pulsante do comércio local. Explore o cenário e utilize as Alavancas Temporais para alternar entre as glórias do passado e as marcas do presente. O tempo está em suas mãos!";

    [TextArea(4, 8)]
    public string textoArmazem =
        "<b>O Armazém Central</b>\n" +
        "Construído à beira-mar, este armazém era o ponto de parada obrigatório para navios de carga. Aqui eram estocadas mercadorias preciosas como café, especiarias, tecidos e ferramentas antes de seguirem viagem. No passado, o barulho das carroças e caixotes não parava nunca!";

    [TextArea(4, 8)]
    public string textoChamines =
        "<b>As Grandes Chaminés</b>\n" +
        "Testemunhas da era industrial do complexo. Estas chaminés faziam parte das antigas caldeiras e máquinas a vapor que processavam produtos e geravam energia para o Trapiche no século passado. Olhando para cima, você quase consegue ver a fumaça do progresso cortando o céu.";

    [TextArea(4, 8)]
    public string textoDonaTete =
        "<b>Armazém Dona Teté</b>\n" +
        "Mais do que um comércio, o armazém de Dona Teté era o ponto de encontro da comunidade. Entre sacos de grãos e produtos coloniais, os marinheiros e moradores se reuniam aqui para tomar um café e conversar. Dona Teté era famosa por sua hospitalidade e por saber todos os segredos do Trapiche.";

    [Header("Configuração")]
    public bool iniciarNoPontoInicial = true;
    public bool ajustarRotacaoNoVR = true;
    public float alturaCameraPC = 1.6f;

    [Header("Transição entre Pontos")]
    public bool usarTransicaoEntrePontos = true;

    [Tooltip("CanvasGroup da imagem usada para cobrir a tela durante o teleporte.")]
    public CanvasGroup mascaraDeTransicao;

    [Tooltip("Duração total da transição entre pontos.")]
    public float duracaoTransicao = 1.2f;

    [Tooltip("Alpha máximo da máscara. Use 1 para cobrir totalmente a tela.")]
    [Range(0f, 1f)]
    public float intensidadeMaximaMascara = 0.85f;

    [Tooltip("Aumento temporário do Field of View para criar sensação de avanço. Para VR, use 0.")]
    public float aumentoFovDuranteTransicao = 0f;

    [Tooltip("Curva de suavização da transição.")]
    public AnimationCurve curvaTransicao = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private LocalDeNavegacao localAtual = LocalDeNavegacao.PontoInicial;
    private bool estaTeleportando = false;

    private Camera cameraPrincipal;
    private float fovOriginal = 60f;

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

        if (Camera.main != null)
        {
            cameraPrincipal = Camera.main;
            fovOriginal = cameraPrincipal.fieldOfView;
        }
    }

    private void Start()
    {
        DefinirAlphaMascara(0f);
        RestaurarFovOriginal();

        if (iniciarNoPontoInicial)
        {
            StartCoroutine(TeletransportarEAtualizarPainel(pontoInicial, LocalDeNavegacao.PontoInicial, false));
        }
        else
        {
            AtualizarEstadoDosBotoes();
        }
    }

    public void AcionarBotao(BotaoDeNavegacao botaoAcionado)
    {
        if (botaoAcionado == null || estaTeleportando)
        {
            return;
        }

        LocalDeNavegacao destinoEscolhido = DefinirDestinoDoBotao(botaoAcionado);
        Transform pontoDestino = ObterTransformDoLocal(destinoEscolhido);

        if (pontoDestino == null)
        {
            Debug.LogWarning("Destino não configurado: " + destinoEscolhido);
            return;
        }

        StartCoroutine(TeletransportarEAtualizarPainel(pontoDestino, destinoEscolhido, usarTransicaoEntrePontos));
    }

    private LocalDeNavegacao DefinirDestinoDoBotao(BotaoDeNavegacao botaoAcionado)
    {
        if (localAtual != LocalDeNavegacao.PontoInicial && botaoAcionado.destinoOriginal == localAtual)
        {
            return LocalDeNavegacao.PontoInicial;
        }

        return botaoAcionado.destinoOriginal;
    }

    private IEnumerator TeletransportarEAtualizarPainel(
        Transform pontoDestino,
        LocalDeNavegacao novoLocal,
        bool usarTransicao
    )
    {
        estaTeleportando = true;

        if (usarTransicao)
        {
            yield return ExecutarPrimeiraMetadeDaTransicao();
        }

        TeletransportarPara(pontoDestino);

        yield return null;
        yield return new WaitForEndOfFrame();

        localAtual = novoLocal;

        PosicionarPainelNoAnchor(localAtual);
        AtualizarEstadoDosBotoes();

        if (usarTransicao)
        {
            yield return ExecutarSegundaMetadeDaTransicao();
        }
        else
        {
            DefinirAlphaMascara(0f);
            RestaurarFovOriginal();
        }

        Debug.Log("Usuário foi para: " + localAtual);

        estaTeleportando = false;
    }

    private IEnumerator ExecutarPrimeiraMetadeDaTransicao()
    {
        float metadeDaDuracao = duracaoTransicao / 2f;
        float tempo = 0f;

        float fovDestino = fovOriginal + aumentoFovDuranteTransicao;

        while (tempo < metadeDaDuracao)
        {
            tempo += Time.deltaTime;

            float progresso = Mathf.Clamp01(tempo / metadeDaDuracao);
            float suavizado = curvaTransicao.Evaluate(progresso);

            DefinirAlphaMascara(suavizado * intensidadeMaximaMascara);
            DefinirFov(Mathf.Lerp(fovOriginal, fovDestino, suavizado));

            yield return null;
        }

        DefinirAlphaMascara(intensidadeMaximaMascara);
        DefinirFov(fovDestino);
    }

    private IEnumerator ExecutarSegundaMetadeDaTransicao()
    {
        float metadeDaDuracao = duracaoTransicao / 2f;
        float tempo = 0f;

        float fovTransicao = fovOriginal + aumentoFovDuranteTransicao;

        while (tempo < metadeDaDuracao)
        {
            tempo += Time.deltaTime;

            float progresso = Mathf.Clamp01(tempo / metadeDaDuracao);
            float suavizado = curvaTransicao.Evaluate(progresso);

            DefinirAlphaMascara((1f - suavizado) * intensidadeMaximaMascara);
            DefinirFov(Mathf.Lerp(fovTransicao, fovOriginal, suavizado));

            yield return null;
        }

        DefinirAlphaMascara(0f);
        RestaurarFovOriginal();
    }

    private void DefinirAlphaMascara(float alpha)
    {
        if (mascaraDeTransicao == null)
        {
            return;
        }

        mascaraDeTransicao.alpha = alpha;
        mascaraDeTransicao.interactable = false;
        mascaraDeTransicao.blocksRaycasts = false;
    }

    private void DefinirFov(float novoFov)
    {
        if (cameraPrincipal == null)
        {
            return;
        }

        cameraPrincipal.fieldOfView = novoFov;
    }

    private void RestaurarFovOriginal()
    {
        if (cameraPrincipal == null)
        {
            return;
        }

        cameraPrincipal.fieldOfView = fovOriginal;
    }

    private void AtualizarEstadoDosBotoes()
    {
        ConfigurarBotao(botaoArmazem, LocalDeNavegacao.Armazem, "ARMAZÉM");
        ConfigurarBotao(botaoChamines, LocalDeNavegacao.Chamines, "CHAMINÉS");
        ConfigurarBotao(botaoDonaTete, LocalDeNavegacao.DonaTete, "DONA TETÉ");

        AtualizarTextoInformativo();
    }

    private void ConfigurarBotao(BotaoDeNavegacao botao, LocalDeNavegacao localDoBotao, string textoOriginal)
    {
        if (botao == null)
        {
            return;
        }

        botao.gameObject.SetActive(true);

        if (localAtual != LocalDeNavegacao.PontoInicial && localAtual == localDoBotao)
        {
            botao.DefinirTexto("PONTO INICIAL");
        }
        else
        {
            botao.DefinirTexto(textoOriginal);
        }
    }

    private void AtualizarTextoInformativo()
    {
        if (textoInformativo == null)
        {
            return;
        }

        if (localAtual == LocalDeNavegacao.PontoInicial)
        {
            textoInformativo.text = textoPontoInicial;
            return;
        }

        if (localAtual == LocalDeNavegacao.Armazem)
        {
            textoInformativo.text = textoArmazem;
            return;
        }

        if (localAtual == LocalDeNavegacao.Chamines)
        {
            textoInformativo.text = textoChamines;
            return;
        }

        if (localAtual == LocalDeNavegacao.DonaTete)
        {
            textoInformativo.text = textoDonaTete;
        }
    }

    private Transform ObterTransformDoLocal(LocalDeNavegacao local)
    {
        if (local == LocalDeNavegacao.PontoInicial)
        {
            return pontoInicial;
        }

        if (local == LocalDeNavegacao.Armazem)
        {
            return visaoArmazem;
        }

        if (local == LocalDeNavegacao.Chamines)
        {
            return visaoChamines;
        }

        if (local == LocalDeNavegacao.DonaTete)
        {
            return visaoDonaTete;
        }

        return null;
    }

    private Transform ObterAnchorDoPainel(LocalDeNavegacao local)
    {
        if (local == LocalDeNavegacao.PontoInicial)
        {
            return painelAnchorPontoInicial;
        }

        if (local == LocalDeNavegacao.Armazem)
        {
            return painelAnchorArmazem;
        }

        if (local == LocalDeNavegacao.Chamines)
        {
            return painelAnchorChamines;
        }

        if (local == LocalDeNavegacao.DonaTete)
        {
            return painelAnchorDonaTete;
        }

        return null;
    }

    private void TeletransportarPara(Transform destino)
    {
        if (destino == null)
        {
            return;
        }

        MoverXROrigin(destino);
        MoverCameraPC(destino);
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
        cameraOuJogadorPC.rotation = Quaternion.Euler(0f, destino.eulerAngles.y, 0f);
    }

    private void PosicionarPainelNoAnchor(LocalDeNavegacao local)
    {
        if (painelDeEscolha == null)
        {
            Debug.LogWarning("PainelDeEscolha não configurado no GerenciadorDeNavegacao.");
            return;
        }

        Transform anchor = ObterAnchorDoPainel(local);

        if (anchor == null)
        {
            Debug.LogWarning("Anchor do painel não configurado para: " + local);
            return;
        }

        painelDeEscolha.position = anchor.position;
        painelDeEscolha.rotation = anchor.rotation;

        Debug.Log("Painel movido para o anchor de: " + local);
    }
}