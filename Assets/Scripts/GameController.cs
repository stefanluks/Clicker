using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public bool debug = false;

    [Header("Bolas")]
    [SerializeField] private GameObject _bolaComum;
    [SerializeField] private GameObject _bolaDanger;

    [Header("Interface")]
    [SerializeField] private Text _HUD_pontos;
    [SerializeField] private Button _HUD_btn_pause;
    [SerializeField] private Text _HUD_debug;
    [SerializeField] private Text _HUD_tempo;
    [SerializeField] private GameObject _menuPause;
    [SerializeField] private GameObject _ranking;
    [SerializeField] private GameObject _menuOpcoes;
    [SerializeField] private GameObject _menuPrincipal;

    [Header("-- RANKING --")]
    [SerializeField] private List<Text> _fieldsRanking;
    
    [Header("-- MENU OPÇÕES --")]
    private bool _musica = false, efeitoSonoro = false;
    [SerializeField] private Image imgLigadoMusica;
    [SerializeField] private Image imgMuteMusica;
    [SerializeField] private Image imgLigadoEfeito;
    [SerializeField] private Image imgMuteEfeito;

    [Header("-- AUDIOS --")]
    [SerializeField] private AudioSource _clickBola;
    [SerializeField] private AudioSource _clickBtns;
    [SerializeField] private AudioSource _trilahSonora;

    [Header("-- Menu Pause --")]
    [SerializeField] private Text _pontosMenuPause;

    // variaveis privadas não acessivéis
    private List<GameObject> _bolasNoJogo = new List<GameObject>();
    private string abc = "abcdefghijklmnopqrstuvwxyz0123456789";
    private bool pause = false, start = false;
    private int cont = 499, tempo = 500, bolas = 0, limiteBolas = 10, pontos = 0, flag = 100;
    private string telaAnterior = "menu";
    private int linguaID = 1;

    void Awake(){ 
    }

    void Start(){
        LinguagemControle._instancia.UpdateLangInterface(linguaID);
        _bolasNoJogo = new List<GameObject>();
        _menuPrincipal.SetActive(true);
        _menuPause.SetActive(false);
        _menuOpcoes.SetActive(false);
        _ranking.SetActive(false);
        _HUD_btn_pause.gameObject.SetActive(false);
        _HUD_pontos.gameObject.SetActive(false);
        _HUD_tempo.gameObject.SetActive(false);
        _HUD_debug.gameObject.SetActive(false);
    }

    void Update(){
        if(start && !pause){
            ControleBolas();
            Click();
            ControleDeTempo();
            _HUD_pontos.text = LinguagemControle._instancia.GetHudTextTop(pontos,bolas,limiteBolas);
            _HUD_tempo.text = "   "+ LinguagemControle._instancia.GetTempo(tempo - cont);
            _pontosMenuPause.text =  LinguagemControle._instancia.GetPontuacao(pontos);
        }
        if(debug){
            _HUD_debug.gameObject.SetActive(true);
            _HUD_debug.text = LinguagemControle._instancia.GetDebug(cont, tempo, _musica, debug, efeitoSonoro, limiteBolas);
        }else{
            _HUD_debug.gameObject.SetActive(false);
        }
        _clickBola.mute = efeitoSonoro;
        _clickBtns.mute = efeitoSonoro;
        _trilahSonora.mute = _musica;
    }

    public void Pause(){
        pause = !pause;
        if(pause){
            _HUD_btn_pause.gameObject.SetActive(false);
            _HUD_pontos.gameObject.SetActive(false);
            _menuPause.SetActive(true);
        }else{
            _HUD_btn_pause.gameObject.SetActive(true);
            _HUD_pontos.gameObject.SetActive(true);
            _menuPause.SetActive(false);
        }
    }

    public void Sair(){
        Application.Quit();
    }

    public void Jogar(){
        start = true;
        _menuPrincipal.SetActive(false);
        _HUD_btn_pause.gameObject.SetActive(true);
        _HUD_pontos.gameObject.SetActive(true);
        _HUD_tempo.gameObject.SetActive(true);
        telaAnterior = "jogo";
    }

    public void Opcoes(){
        _menuPrincipal.SetActive(false);
        _menuOpcoes.SetActive(true);
    }

    public void RankingPage(){
        _ranking.SetActive(true);
        _menuPrincipal.SetActive(false);
        StartCoroutine(RedeController._instancia.GetRanking(ConfigurarRaking));
    }

    void DestruirBolas(){
        foreach (GameObject bola in _bolasNoJogo)
        {
            Destroy(bola);
        }
        _bolasNoJogo = new List<GameObject>();
    }

    public void OPVoltar(){
        if(telaAnterior == "menu") VoltarParaMenu();
        else  _menuOpcoes.SetActive(false);
    }

    public void VoltarParaMenu(){
        start = false;
        pause = false;
        DestruirBolas();
        cont = 998;
        tempo = 1000;
        bolas = 0;
        pontos = 0;
        flag = 100;
        telaAnterior = "menu";
        _menuPrincipal.SetActive(true);
        _menuOpcoes.SetActive(false);
        _menuPause.SetActive(false);
        _ranking.SetActive(false);
        _HUD_btn_pause.gameObject.SetActive(false);
        _HUD_pontos.gameObject.SetActive(false);
        _HUD_tempo.gameObject.SetActive(false);
    }

    void ConfigurarRaking(Ranking lista){
        for(int i=0;i< 10; i++)
        {
            if(i >= lista.rankingGeral.Count) _fieldsRanking[i].gameObject.SetActive(false);
            else _fieldsRanking[i].text = lista.rankingGeral[i].nome_do_jogador +"\n"+"Pontos:"+lista.rankingGeral[i].pontos_do_jogador;
        }
    }

    void ControleDeTempo(){
        if(pontos > flag){
            flag*=2;
            if(tempo > 100) tempo -= 150;
            else tempo -= 10;
        }
    }

    void AdicionarBola(){
        System.Random randNum = new System.Random();
        Vector3 pos = new Vector3(randNum.Next(-17,17), randNum.Next(9, 28), _bolaComum.transform.position.z);
        // if(randNum.Next(0,1) == 0){
        //     pos = new Vector3(-pos.x, pos.y, pos.z);
        // }
        GameObject nova = Instantiate(_bolaComum, pos, Quaternion.identity);
        _bolasNoJogo.Add(nova);
        bolas++;
    }

    void ControleBolas(){
        if(cont == tempo){
            AdicionarBola();
            cont = 0;
        }else{
            cont+=1;
        }
    }

    void Click(){
        if(Input.GetMouseButtonDown(0)){
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100f))
            {
                if(hit.transform.gameObject.tag == "bola"){
                    pontos+=8;
                    bolas--;
                    _clickBola.Play();
                    Destroy(hit.transform.gameObject);
                    // if(bolas == 0){
                    //     cont = 0;
                    //     AdicionarBola();
                    // }
                }
            }
        }
    }

    public void OPmusicaONOFF(){
        _musica = !_musica;
        if(_musica){
            imgLigadoMusica.gameObject.SetActive(false);
            imgMuteMusica.gameObject.SetActive(true);
        }else{
            imgLigadoMusica.gameObject.SetActive(true);
            imgMuteMusica.gameObject.SetActive(false);
        }
    }
    
    public void OPefeitoONOFF(){
        efeitoSonoro = !efeitoSonoro;
        if(efeitoSonoro){
            imgLigadoEfeito.gameObject.SetActive(false);
            imgMuteEfeito.gameObject.SetActive(true);
        }else{
            imgLigadoEfeito.gameObject.SetActive(true);
            imgMuteEfeito.gameObject.SetActive(false);
        }
    }

    public void ToqueBotao(){
        _clickBtns.Play();
    }

    public void SelecionarLingua(){
        int max = LinguagemControle._instancia.GetQntLangs()-1;
        if(linguaID < max) linguaID ++;
        else linguaID = 0;
        LinguagemControle._instancia.UpdateLangInterface(linguaID);
    }
}
