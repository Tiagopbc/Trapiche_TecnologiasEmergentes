using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ControleDeAlavanca : MonoBehaviour
{
    [Header("Estado Atual")]
    public bool estaAtivada = false;

    [Header("Controle de Interação")]
    [Tooltip("Tempo mínimo entre uma interação e outra, para evitar clique duplicado no PC ou no VR.")]
    public float tempoEntreInteracoes = 0.5f;

    [Header("O que essa alavanca faz?")]
    public UnityEvent aoLigar;
    public UnityEvent aoDesligar;

    private bool podeInteragir = true;

    private void OnMouseDown()
    {
        Interagir();
    }

    public void Interagir()
    {
        if (!podeInteragir)
        {
            return;
        }

        StartCoroutine(ProcessarInteracao());
    }

    private IEnumerator ProcessarInteracao()
    {
        podeInteragir = false;
        estaAtivada = true;

        Debug.Log(gameObject.name + " foi acionada.");

        aoLigar.Invoke();

        yield return new WaitForSeconds(tempoEntreInteracoes);

        podeInteragir = true;
    }

    public void Desativar()
    {
        estaAtivada = false;

        Debug.Log(gameObject.name + " foi desativada.");

        aoDesligar.Invoke();
    }
}