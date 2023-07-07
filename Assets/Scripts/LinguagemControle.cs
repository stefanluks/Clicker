using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Atributos{
    public string menu_titulo;
    public string menu_btnjogar;
    public string menu_btnRank;
    public string menu_btnOpcoes;
    public string menu_btnSair;
    public string opcoes_titulo;
    public string opcoes_btnVoltar;
    public string opcoes_btnmusica;
    public string opcoes_btnEfeitoSonoro;
    public string opcoes_btnlang;
    public string rank_titulo;
    public string rank_btnVoltar;
    public string pause_titulo;
    public string pause_pontos;
    public string pause_btnVoltar;
    public string pause_btnOpcoes;
    public string pause_btnVoltarPMenu;
    public string hud_pontos;
    public string hud_bolas;
    public string hud_debug;
    public string hud_tempo;
    public string seusPontos;
}

[System.Serializable]
public class Lingua{
    public string nome;
    public int id;
    public Atributos atributos;
}

public class LinguagemControle : MonoBehaviour
{

    public static LinguagemControle _instancia {get; set;}
    
    private int idBase = 0;
    [SerializeField] private List<Lingua> linguasDisponiveis;
    [SerializeField] private Lingua linguaSelecionada;

    [Header("-- MENU --")]
    [SerializeField] private Text m_titulo;
    [SerializeField] private Text btnjogar;
    [SerializeField] private Text btnRank;
    [SerializeField] private Text btnOpcoes;
    [SerializeField] private Text btnSair;
    [Header("-- OPções --")]
    [SerializeField] private Text op_titulo;
    [SerializeField] private Text btnVoltar;
    [SerializeField] private Text btnmusica;
    [SerializeField] private Text btnEfeitoSonoro;
    [SerializeField] private Text btnlang;
    [Header("-- Ranking --")]
    [SerializeField] private Text r_titulo;
    [SerializeField] private Text r_btnVoltar;
    [Header("-- Menu Pause --")]
    [SerializeField] private Text p_titulo;
    [SerializeField] private Text p_btnVoltar;
    [SerializeField] private Text p_btnOpcoes;
    [SerializeField] private Text p_btnVoltarPMenu;
    
    [Header("-- HUD do jogo --")]
    private string h_pontos;
    private string h_debug;
    private string h_tempo;
    private string h_bolas;


    private void Awake() {
        if (_instancia != null && _instancia != this)
        {
            Destroy(this.gameObject);
        } else {
            _instancia = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        linguaSelecionada = linguasDisponiveis[idBase];
    }

    public int GetQntLangs(){
        return linguasDisponiveis.Count;
    }

    public void UpdateLangInterface(int linguaID){
        linguaSelecionada = linguasDisponiveis[linguaID];
        // Atualizar itens do menu
        m_titulo.text = linguaSelecionada.atributos.menu_titulo;
        btnjogar.text = linguaSelecionada.atributos.menu_btnjogar;
        btnRank.text = linguaSelecionada.atributos.menu_btnRank;
        btnOpcoes.text = linguaSelecionada.atributos.menu_btnOpcoes;
        btnSair.text = linguaSelecionada.atributos.menu_btnSair;
        // Atualizar menu de opções
        op_titulo.text = linguaSelecionada.atributos.opcoes_titulo;
        btnVoltar.text = linguaSelecionada.atributos.opcoes_btnVoltar;
        btnmusica.text = linguaSelecionada.atributos.opcoes_btnmusica;
        btnEfeitoSonoro.text = linguaSelecionada.atributos.opcoes_btnEfeitoSonoro;
        btnlang.text = linguaSelecionada.atributos.opcoes_btnlang;
        // Atualizando tela de ranking
        r_titulo.text = linguaSelecionada.atributos.rank_titulo;
        r_btnVoltar.text = linguaSelecionada.atributos.rank_btnVoltar;
        // Atuliazando tela de pause
        p_titulo.text = linguaSelecionada.atributos.pause_titulo;
        p_btnVoltar.text = linguaSelecionada.atributos.pause_btnVoltar;
        p_btnOpcoes.text = linguaSelecionada.atributos.pause_btnOpcoes;
        p_btnVoltarPMenu.text = linguaSelecionada.atributos.pause_btnVoltarPMenu;
        // Atualizando HUD
        h_pontos = linguaSelecionada.atributos.hud_pontos;
        h_debug = linguaSelecionada.atributos.hud_debug;
        h_tempo = linguaSelecionada.atributos.hud_tempo;
        h_bolas = linguaSelecionada.atributos.hud_bolas;
    }

    public string GetHudTextTop(int pontos, int bolas, int limiteBolas){
        return "   "+h_pontos+": "+pontos+"\n   "+h_bolas+": "+bolas+"/"+limiteBolas;
    }

    public string GetTempo(int tempo){
        return h_tempo+": "+ tempo;
    }

    public string GetPontuacao(int pontos){
        return linguaSelecionada.atributos.seusPontos + ": " + pontos;
    }

    public string GetDebug(int cont, int tempo, bool musica, bool bug, bool efeitoSonoro, int limiteB){
        string[] lista = linguaSelecionada.atributos.hud_debug.Split(", ");
        string spawn = lista[0];
        string debug = lista[1];
        string music = lista[2];
        string efeito =lista[3];
        string limite =lista[4];
        return spawn+": "+cont+"/"+tempo+"\n"+debug+": "+bug+"\n"+music+": "+musica+"\n"+efeito+": "+efeitoSonoro+"\n"+limite+":"+limiteB;
    }
}
