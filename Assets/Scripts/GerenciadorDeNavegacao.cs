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

    [Header("Placas Informativas")]
    public GameObject placaInformativa01;
    public GameObject placaInformativa02;
    public GameObject placaInformativa03;

    public TMP_Text textoPlacaInformativa01;
    public TMP_Text textoPlacaInformativa02;
    public TMP_Text textoPlacaInformativa03;

    [Header("Textos do Ponto Inicial")]
    [TextArea(4, 8)]
    public string pontoInicialTexto01 =
        "<b>O Complexo Trapiche</b>\n" +
        "Você está no ponto de partida de uma experiência de visitação pelo Complexo Trapiche. Este espaço representa uma antiga área ligada à circulação de pessoas, mercadorias e memórias. Ao longo do percurso, observe como o tempo transforma a paisagem, os usos e os significados de um lugar.";

    [TextArea(4, 8)]
    public string pontoInicialTexto02 =
        "<b>O Trapiche no passado</b>\n" +
        "No passado, áreas como esta tinham papel estratégico para o comércio e para o abastecimento local. Armazéns, embarcações, trabalhadores e mercadorias compunham uma paisagem movimentada. O Trapiche era um ponto de encontro entre o mar, a cidade e a economia de sua época.";

    [TextArea(4, 8)]
    public string pontoInicialTexto03 =
        "<b>O Trapiche hoje</b>\n" +
        "Hoje, o Complexo Trapiche pode ser compreendido como espaço de memória, preservação e ressignificação. Mesmo quando suas funções originais mudam, seus elementos arquitetônicos continuam contando histórias sobre trabalho, comércio, tecnologia e vida urbana.";

    [Header("Textos do Armazém")]
    [TextArea(4, 8)]
    public string armazemTexto01 =
        "<b>O Armazém Central</b>\n" +
        "O armazém era uma das estruturas mais importantes do complexo. Ele servia como local de guarda, organização e circulação de produtos. Sua posição no conjunto revela a importância da logística e do controle das mercadorias que chegavam, permaneciam temporariamente e depois seguiam para outros destinos.";

    [TextArea(4, 8)]
    public string armazemTexto02 =
        "<b>O armazém no passado</b>\n" +
        "Na visão histórica, o armazém representa o período em que o Trapiche funcionava como espaço de intensa movimentação. Caixotes, sacos, ferramentas, tecidos e outros produtos passavam por esse tipo de construção. O ambiente era marcado pelo trabalho manual e pelo fluxo constante de pessoas.";

    [TextArea(4, 8)]
    public string armazemTexto03 =
        "<b>O armazém hoje</b>\n" +
        "Na visão contemporânea, o armazém deixa de ser apenas um espaço de armazenamento e passa a ser visto como patrimônio. Suas paredes, formas e marcas materiais ajudam a compreender como a cidade se transformou. Mesmo sem a função original, ele continua sendo referência para a memória do lugar.";

    [Header("Textos das Chaminés")]
    [TextArea(4, 8)]
    public string chaminesTexto01 =
        "<b>As Grandes Chaminés</b>\n" +
        "As chaminés são marcas visuais da presença de atividades produtivas no complexo. Elas remetem a um tempo em que máquinas, caldeiras e sistemas movidos a vapor faziam parte da paisagem do trabalho. Sua altura não era apenas funcional: também indicava poder técnico e transformação industrial.";

    [TextArea(4, 8)]
    public string chaminesTexto02 =
        "<b>Marcos na paisagem</b>\n" +
        "Mesmo quando deixam de cumprir sua função original, as chaminés permanecem como sinais fortes na paisagem. Elas ajudam o visitante a reconhecer o passado industrial do lugar e funcionam como monumentos silenciosos. Olhar para elas é perceber como a arquitetura guarda vestígios do trabalho e da tecnologia.";

    [Header("Textos do Armazém Dona Teté")]
    [TextArea(4, 8)]
    public string donaTeteTexto01 =
        "<b>Armazém Dona Teté</b>\n" +
        "O Armazém Dona Teté representa uma dimensão mais cotidiana e comunitária do complexo. Além da função comercial, esse tipo de espaço podia servir como ponto de encontro, conversa e convivência. Ele aproxima a história do Trapiche da vida das pessoas que circularam por esse lugar.";

    [TextArea(4, 8)]
    public string donaTeteTexto02 =
        "<b>Dona Teté no passado</b>\n" +
        "Na visão histórica, o armazém é imaginado como um espaço vivo, frequentado por trabalhadores, moradores e visitantes. Entre mercadorias, balcões e conversas, o lugar reunia práticas comerciais e relações sociais. Dona Teté simboliza essa memória afetiva ligada à hospitalidade e ao comércio local.";

    [TextArea(4, 8)]
    public string donaTeteTexto03 =
        "<b>Dona Teté hoje</b>\n" +
        "Na visão contemporânea, o Armazém Dona Teté pode ser lido como um lugar de memória. Mesmo transformado pelo tempo, ele preserva sentidos ligados à comunidade, ao pertencimento e à identidade local. Sua presença mostra que o patrimônio também está nas pessoas e nas lembranças.";

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

        AtualizarPlacasInformativas();
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

    private void AtualizarPlacasInformativas()
    {
        if (localAtual == LocalDeNavegacao.PontoInicial)
        {
            DefinirTextosDasPlacas(
                pontoInicialTexto01,
                pontoInicialTexto02,
                pontoInicialTexto03,
                true,
                true,
                true
            );

            return;
        }

        if (localAtual == LocalDeNavegacao.Armazem)
        {
            DefinirTextosDasPlacas(
                armazemTexto01,
                armazemTexto02,
                armazemTexto03,
                true,
                true,
                true
            );

            return;
        }

        if (localAtual == LocalDeNavegacao.Chamines)
        {
            DefinirTextosDasPlacas(
                chaminesTexto01,
                chaminesTexto02,
                string.Empty,
                true,
                true,
                false
            );

            return;
        }

        if (localAtual == LocalDeNavegacao.DonaTete)
        {
            DefinirTextosDasPlacas(
                donaTeteTexto01,
                donaTeteTexto02,
                donaTeteTexto03,
                true,
                true,
                true
            );
        }
    }

    private void DefinirTextosDasPlacas(
        string texto01,
        string texto02,
        string texto03,
        bool exibirPlaca01,
        bool exibirPlaca02,
        bool exibirPlaca03
    )
    {
        if (placaInformativa01 != null)
        {
            placaInformativa01.SetActive(exibirPlaca01);
        }

        if (placaInformativa02 != null)
        {
            placaInformativa02.SetActive(exibirPlaca02);
        }

        if (placaInformativa03 != null)
        {
            placaInformativa03.SetActive(exibirPlaca03);
        }

        if (textoPlacaInformativa01 != null)
        {
            textoPlacaInformativa01.text = texto01;
        }

        if (textoPlacaInformativa02 != null)
        {
            textoPlacaInformativa02.text = texto02;
        }

        if (textoPlacaInformativa03 != null)
        {
            textoPlacaInformativa03.text = texto03;
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