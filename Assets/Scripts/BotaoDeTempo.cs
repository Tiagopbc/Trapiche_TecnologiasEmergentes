using System.Collections;
using UnityEngine;
using TMPro;

public class BotaoDeTempo : MonoBehaviour
{
    public enum TipoDeVisaoTemporal
    {
        Historica,
        Contemporanea
    }

    [Header("Referências")]
    public GerenciadorDoTempo gerenciadorDoTempo;

    [Header("Tipo de visão que este botão ativa")]
    public TipoDeVisaoTemporal tipoDeVisaoTemporal;

    [Header("Texto Visual")]
    public TMP_Text textoDoBotao;

    [Header("Feedback Visual")]
    [Tooltip("Renderer do corpo principal da placa.")]
    public Renderer corpoDoBotao;

    [Tooltip("Renderer da borda da placa.")]
    public Renderer bordaDoBotao;

    [Tooltip("Cor normal do corpo do botão.")]
    public Color corCorpoNormal = new Color(0.184f, 0.165f, 0.141f);

    [Tooltip("Cor do corpo quando o usuário aponta para o botão.")]
    public Color corCorpoSelecionado = new Color(0.31f, 0.25f, 0.16f);

    [Tooltip("Cor normal da borda do botão.")]
    public Color corBordaNormal = new Color(0.69f, 0.55f, 0.34f);

    [Tooltip("Cor da borda quando o usuário aponta para o botão.")]
    public Color corBordaSelecionada = new Color(0.84f, 0.70f, 0.42f);

    [Header("Animação de Clique")]
    [Tooltip("Distância local que o botão recua ao ser pressionado.")]
    public float deslocamentoAoPressionar = -0.20f;

    [Tooltip("Duração da animação de pressionar.")]
    public float duracaoAnimacaoClique = 0.45f;

    [Header("Controle")]
    public float tempoEntreCliques = 0.7f;

    private bool podeAcionar = true;
    private bool estaSelecionado = false;
    private bool estaAnimandoClique = false;

    private Vector3 posicaoInicialLocal;

    private Material materialCorpoInstanciado;
    private Material materialBordaInstanciado;

    private void Awake()
    {
        posicaoInicialLocal = transform.localPosition;
        PrepararMateriais();
        AplicarCoresNormais();
        AtualizarTextoInicial();
    }

    private void PrepararMateriais()
    {
        if (corpoDoBotao != null)
        {
            materialCorpoInstanciado = corpoDoBotao.material;
        }

        if (bordaDoBotao != null)
        {
            materialBordaInstanciado = bordaDoBotao.material;
        }
    }

    private void AtualizarTextoInicial()
    {
        if (textoDoBotao == null)
        {
            return;
        }

        if (tipoDeVisaoTemporal == TipoDeVisaoTemporal.Historica)
        {
            textoDoBotao.text = "VISÃO HISTÓRICA";
        }
        else
        {
            textoDoBotao.text = "VISÃO CONTEMPORÂNEA";
        }
    }

    private void OnMouseEnter()
    {
        Selecionar();
    }

    private void OnMouseExit()
    {
        Deselecionar();
    }

    private void OnMouseDown()
    {
        Acionar();
    }

    public void Selecionar()
    {
        estaSelecionado = true;
        AplicarCoresSelecionadas();
    }

    public void Deselecionar()
    {
        estaSelecionado = false;
        AplicarCoresNormais();
    }

    public void Acionar()
    {
        if (!podeAcionar || estaAnimandoClique)
        {
            return;
        }

        if (gerenciadorDoTempo == null)
        {
            Debug.LogWarning("GerenciadorDoTempo não configurado no botão: " + gameObject.name);
            return;
        }

        StartCoroutine(ExecutarClique());
    }

    private IEnumerator ExecutarClique()
    {
        podeAcionar = false;
        estaAnimandoClique = true;

        yield return AnimarPressionamento();

        if (tipoDeVisaoTemporal == TipoDeVisaoTemporal.Historica)
        {
            gerenciadorDoTempo.MostrarVisaoHistorica();
        }
        else
        {
            gerenciadorDoTempo.MostrarVisaoContemporanea();
        }

        Invoke(nameof(LiberarClique), tempoEntreCliques);

        estaAnimandoClique = false;
    }

    private IEnumerator AnimarPressionamento()
    {
        Vector3 posicaoInicial = posicaoInicialLocal;
        Vector3 posicaoPressionada = posicaoInicialLocal + transform.forward * deslocamentoAoPressionar;

        float metadeDaDuracao = duracaoAnimacaoClique / 2f;
        float tempo = 0f;

        while (tempo < metadeDaDuracao)
        {
            tempo += Time.deltaTime;
            float progresso = Mathf.Clamp01(tempo / metadeDaDuracao);

            transform.localPosition = Vector3.Lerp(posicaoInicial, posicaoPressionada, progresso);

            yield return null;
        }

        tempo = 0f;

        while (tempo < metadeDaDuracao)
        {
            tempo += Time.deltaTime;
            float progresso = Mathf.Clamp01(tempo / metadeDaDuracao);

            transform.localPosition = Vector3.Lerp(posicaoPressionada, posicaoInicial, progresso);

            yield return null;
        }

        transform.localPosition = posicaoInicial;
    }

    private void AplicarCoresNormais()
    {
        if (materialCorpoInstanciado != null)
        {
            DefinirCorMaterial(materialCorpoInstanciado, corCorpoNormal);
        }

        if (materialBordaInstanciado != null)
        {
            DefinirCorMaterial(materialBordaInstanciado, corBordaNormal);
        }
    }

    private void AplicarCoresSelecionadas()
    {
        if (materialCorpoInstanciado != null)
        {
            DefinirCorMaterial(materialCorpoInstanciado, corCorpoSelecionado);
        }

        if (materialBordaInstanciado != null)
        {
            DefinirCorMaterial(materialBordaInstanciado, corBordaSelecionada);
        }
    }

    private void DefinirCorMaterial(Material material, Color cor)
    {
        if (material == null)
        {
            return;
        }

        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", cor);
            return;
        }

        if (material.HasProperty("_Color"))
        {
            material.SetColor("_Color", cor);
        }
    }

    private void LiberarClique()
    {
        podeAcionar = true;

        if (estaSelecionado)
        {
            AplicarCoresSelecionadas();
        }
        else
        {
            AplicarCoresNormais();
        }
    }
}