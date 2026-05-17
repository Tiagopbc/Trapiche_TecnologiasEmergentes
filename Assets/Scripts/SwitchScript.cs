using UnityEngine;

public class SwitchScript : MonoBehaviour
{
    public GameObject CasaraoNovo;
    public GameObject CasaraoAntigo;

    void Start()
    {
        // Garante que o cenário comece sempre com o casarão antigo ativo
        if (CasaraoNovo != null && CasaraoAntigo != null)
        {
            CasaraoNovo.SetActive(false);
            CasaraoAntigo.SetActive(true);
        }
    }

    void Update()
    {
        
    }

    // Adicionado 'public' para o Unity listar a função no botão
    public void SwitchBuild()
    {
        if (CasaraoNovo != null && CasaraoAntigo != null)
        {
            if (CasaraoNovo.activeSelf)
            {
                CasaraoNovo.SetActive(false);
                CasaraoAntigo.SetActive(true);
            } 
            else 
            {
                CasaraoNovo.SetActive(true);
                CasaraoAntigo.SetActive(false);
            }
        }
    }
}