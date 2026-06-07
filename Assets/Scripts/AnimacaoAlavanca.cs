using UnityEngine;
using System.Collections;

public class AnimacaoAlavanca : MonoBehaviour
{
    private bool estaAnimando = false;

    [Header("Configuração do Giro")]
    [Tooltip("Modifique os valores de X, Y ou Z no Inspector do Unity para achar a direção perfeita.")]
    public Vector3 eixoEAngulo = new Vector3(0f, 0f, -45f); 

    public void Puxar()
    {
        // Só permite puxar se ela já não estiver se mexendo
        if (!estaAnimando)
        {
            StartCoroutine(MoverAlavanca());
        }
    }

    IEnumerator MoverAlavanca()
    {
        estaAnimando = true;

        Quaternion rotacaoInicial = transform.localRotation;
        
        // Agora o script usa o valor customizado que você colocar no Unity
        Quaternion rotacaoFinal = rotacaoInicial * Quaternion.Euler(eixoEAngulo); 

        // 1. Movimento de puxar para trás
        float tempo = 0;
        while (tempo < 1f)
        {
            tempo += Time.deltaTime * 4f; // Velocidade do puxão
            transform.localRotation = Quaternion.Lerp(rotacaoInicial, rotacaoFinal, tempo);
            yield return null;
        }

        // 2. Espera uma fração de segundo ativada
        yield return new WaitForSeconds(0.3f);

        // 3. Movimento de voltar ao topo (mola)
        tempo = 0;
        while (tempo < 1f)
        {
            tempo += Time.deltaTime * 4f;
            transform.localRotation = Quaternion.Lerp(rotacaoFinal, rotacaoInicial, tempo);
            yield return null;
        }

        estaAnimando = false;
    }
}