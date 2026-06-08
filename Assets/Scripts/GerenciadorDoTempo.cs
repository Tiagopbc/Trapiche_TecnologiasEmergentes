using System.Collections;
using UnityEngine;

public class GerenciadorDoTempo : MonoBehaviour
{
    public enum VisaoDoTempo
    {
        Historica,
        Contemporanea
    }

    [Header("Pastas dos Cenários")]
    public GameObject pastaHistorica;
    public GameObject pastaContemporanea;

    [Header("Configuração Inicial")]
    public VisaoDoTempo visaoInicial = VisaoDoTempo.Historica;

    [Header("Transição")]
    public float duracaoTransicao = 1.2f;
    public AnimationCurve curvaTransicao = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Máscara de Tela")]
    public bool usarMascaraDeTela = true;
    public CanvasGroup mascaraDeTela;

    [Range(0f, 1f)]
    public float intensidadeMaximaMascara = 0.9f;

    [Header("Estado Atual")]
    public VisaoDoTempo visaoAtual;

    private bool estaTransicionando = false;

    private void Start()
    {
        AplicarVisaoImediata(visaoInicial);
        DefinirAlphaMascara(0f);
    }

    public void MostrarVisaoHistorica()
    {
        IniciarTransicao(VisaoDoTempo.Historica);
    }

    public void MostrarVisaoContemporanea()
    {
        IniciarTransicao(VisaoDoTempo.Contemporanea);
    }

    public void MostrarPassado()
    {
        MostrarVisaoHistorica();
    }

    public void MostrarPresente()
    {
        MostrarVisaoContemporanea();
    }

    public void AlternarVisaoTemporal()
    {
        if (visaoAtual == VisaoDoTempo.Historica)
        {
            MostrarVisaoContemporanea();
        }
        else
        {
            MostrarVisaoHistorica();
        }
    }

    private void IniciarTransicao(VisaoDoTempo novaVisao)
    {
        if (estaTransicionando)
        {
            return;
        }

        if (novaVisao == visaoAtual)
        {
            return;
        }

        StartCoroutine(TransicionarPara(novaVisao));
    }

    private IEnumerator TransicionarPara(VisaoDoTempo novaVisao)
    {
        estaTransicionando = true;

        if (usarMascaraDeTela && mascaraDeTela != null)
        {
            yield return FecharMascara();
        }

        AplicarVisaoImediata(novaVisao);

        yield return null;
        yield return new WaitForEndOfFrame();

        if (usarMascaraDeTela && mascaraDeTela != null)
        {
            yield return AbrirMascara();
        }

        estaTransicionando = false;

        Debug.Log("Visão temporal atual: " + visaoAtual);
    }

    private IEnumerator FecharMascara()
    {
        float metadeDaDuracao = duracaoTransicao / 2f;
        float tempo = 0f;

        while (tempo < metadeDaDuracao)
        {
            tempo += Time.deltaTime;

            float progresso = Mathf.Clamp01(tempo / metadeDaDuracao);
            float suavizado = curvaTransicao.Evaluate(progresso);

            DefinirAlphaMascara(suavizado * intensidadeMaximaMascara);

            yield return null;
        }

        DefinirAlphaMascara(intensidadeMaximaMascara);
    }

    private IEnumerator AbrirMascara()
    {
        float metadeDaDuracao = duracaoTransicao / 2f;
        float tempo = 0f;

        while (tempo < metadeDaDuracao)
        {
            tempo += Time.deltaTime;

            float progresso = Mathf.Clamp01(tempo / metadeDaDuracao);
            float suavizado = curvaTransicao.Evaluate(progresso);

            DefinirAlphaMascara((1f - suavizado) * intensidadeMaximaMascara);

            yield return null;
        }

        DefinirAlphaMascara(0f);
    }

    private void AplicarVisaoImediata(VisaoDoTempo visao)
    {
        visaoAtual = visao;

        if (pastaHistorica != null)
        {
            pastaHistorica.SetActive(visaoAtual == VisaoDoTempo.Historica);
        }

        if (pastaContemporanea != null)
        {
            pastaContemporanea.SetActive(visaoAtual == VisaoDoTempo.Contemporanea);
        }

        DefinirAlphaMascara(0f);
    }

    private void DefinirAlphaMascara(float alpha)
    {
        if (mascaraDeTela == null)
        {
            return;
        }

        mascaraDeTela.alpha = alpha;
        mascaraDeTela.interactable = false;
        mascaraDeTela.blocksRaycasts = false;
    }
}