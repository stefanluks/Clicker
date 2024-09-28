using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public bool debug = false;
    private int JOGO_ID = 13;

    [Header("Bolas")]
    [SerializeField] private GameObject _bolaComum;
    [SerializeField] private GameObject _bolaDanger;

    [Header("Telas")]
    [SerializeField] private GameObject _menuPause;
    [SerializeField] private GameObject _ranking;
    [SerializeField] private GameObject _menuOpcoes;
    [SerializeField] private GameObject _menuPrincipal;
    [SerializeField] private GameObject _menuGameOver;
    [SerializeField] private GameObject _tutorial;

    [Header("HUD")]
    [SerializeField] private Text _HUD_pontos;
    [SerializeField] private Button _HUD_btn_pause;
    [SerializeField] private Text _HUD_debug;
    [SerializeField] private Text _HUD_tempo;

    [Header("-- RANKING --")]
    [SerializeField] private List<Text> _fieldsRanking;
    
    [Header("-- MENU OPÇÕES --")]
    private bool _musica = false, efeitoSonoro = false;
    [SerializeField] private Image imgLigadoMusica;
    [SerializeField] private Image imgMuteMusica;
    [SerializeField] private Image imgLigadoEfeito;
    [SerializeField] private Image imgMuteEfeito;

    [Header("-- GAME OVER --")]
    [SerializeField] private Text _pontosGameOver;
    [SerializeField] private Image _msg_sucesso_GameOver;
    [SerializeField] private Image _msg_falha_GameOver;
    [SerializeField] private InputField nomeJogador;


    [Header("-- AUDIOS --")]
    [SerializeField] private AudioSource _clickBola;
    [SerializeField] private AudioSource _clickBtns;
    [SerializeField] private AudioSource _trilahSonora;

    [Header("-- Menu Pause --")]
    [SerializeField] private Text _pontosMenuPause;

    // variaveis privadas não acessivéis
    private List<GameObject> _bolasNoJogo = new List<GameObject>();
    private bool pause = false, start = false, gameover = false, tutorial = true;
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
        _menuGameOver.SetActive(false);
        _tutorial.SetActive(false);
        _HUD_btn_pause.gameObject.SetActive(false);
        _HUD_pontos.gameObject.SetActive(false);
        _HUD_tempo.gameObject.SetActive(false);
        _HUD_debug.gameObject.SetActive(false);
    }

    void Update(){
        if(!gameover){
            if(start && !pause){
                ControleBolas();
                Click();
                ControleDeTempo();
                _HUD_pontos.text = LinguagemControle._instancia.GetHudTextTop(pontos,bolas,limiteBolas);
                _HUD_tempo.text = "   "+ LinguagemControle._instancia.GetTempo(tempo - cont);
                _pontosMenuPause.text =  LinguagemControle._instancia.GetPontuacao(pontos);
                _pontosGameOver.text =  LinguagemControle._instancia.GetPontuacao(pontos);
            }
            if(debug){
                _HUD_debug.gameObject.SetActive(true);
                _HUD_debug.text = LinguagemControle._instancia.GetDebug(cont, tempo, _musica, debug, efeitoSonoro, limiteBolas);
            }else{
                _HUD_debug.gameObject.SetActive(false);
            }
            if(bolas >= limiteBolas) gameover = true;
        }else{
            _menuGameOver.SetActive(true);
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
        if(tutorial){
            _tutorial.SetActive(true);
            tutorial = false;
        }else{
            _tutorial.SetActive(false);
            start = true;
        }
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

    public void Salvar(){
        SalvarPontuacao();
    }

    [System.Obsolete]
    public void SalvarPontuacao(){
        Pontuacao jogador = new Pontuacao();
        jogador.nome = nomeJogador.text;
        jogador.pontos = ""+pontos;
        jogador.jogo = JOGO_ID;
        StartCoroutine(RedeController._instancia.SavePontuacao(jogador, ResultadoSave));
    }

    void ResultadoSave(bool result){
        if(result) _msg_sucesso_GameOver.gameObject.SetActive(true);
        else _msg_falha_GameOver.gameObject.SetActive(true);
        Invoke("DesativarMsg", 1f);
    }

    void DesativarMsg(){
        _msg_falha_GameOver.gameObject.SetActive(false);
        _msg_sucesso_GameOver.gameObject.SetActive(false);
    }


    [System.Obsolete]
    public void RankingPage(){
        _ranking.SetActive(true);
        _menuPrincipal.SetActive(false);
        StartCoroutine(RedeController._instancia.GetRanking(JOGO_ID, ConfigurarRaking));
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

    public void Reiniciar(){
        VoltarParaMenu();
        _menuPrincipal.SetActive(false);
        start = true;
        telaAnterior = "jogo";
        _HUD_btn_pause.gameObject.SetActive(true);
        _HUD_pontos.gameObject.SetActive(true);
        _HUD_tempo.gameObject.SetActive(true);
    }

    public void VoltarParaMenu(){
        start = false;
        pause = false;
        gameover = false;
        DestruirBolas();
        cont = 0;
        tempo = 500;
        bolas = 0;
        pontos = 0;
        flag = 100;
        telaAnterior = "menu";
        _menuPrincipal.SetActive(true);
        _menuGameOver.SetActive(false);
        _menuOpcoes.SetActive(false);
        _menuPause.SetActive(false);
        _ranking.SetActive(false);
        _HUD_btn_pause.gameObject.SetActive(false);
        _HUD_pontos.gameObject.SetActive(false);
        _HUD_tempo.gameObject.SetActive(false);
    }

    void ConfigurarRaking(Ranking lista){
        if(lista != null){
            for(int i=0;i< 10; i++)
            {
                if(i >= lista.content.ranking.Count) _fieldsRanking[i].gameObject.SetActive(false);
                else _fieldsRanking[i].text = lista.content.ranking[i].nome +"\n"+"Pontos:"+lista.content.ranking[i].pontos;
            }
        }
    }

    void ControleDeTempo(){
        if(pontos > flag){
            flag*=2;
            limiteBolas = flag/10;
            if(tempo > 200) tempo -= 150;
            else if(tempo > 100) tempo -= 50;
            else if(tempo > 50) tempo -= 5;
            else tempo = 50;
        }
    }

    void AdicionarBola(){
        System.Random randNum = new System.Random();
        Vector3 pos = new Vector3(randNum.Next(-17,17), randNum.Next(9, 28), _bolaComum.transform.position.z);
        GameObject prefab = _bolaComum;
        int sorte = randNum.Next(1,15);
        Debug.Log("SORTE: "+sorte);
        if(sorte == 1) prefab = _bolaDanger;
        GameObject nova = Instantiate(prefab, pos, Quaternion.identity);
        _bolasNoJogo.Add(nova);
        bolas++;
    }

    void ControleBolas(){
        if(cont >= tempo){
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
                    pontos+=5;
                    bolas--;
                    _clickBola.Play();
                    Destroy(hit.transform.gameObject);
                }
                if(hit.transform.gameObject.tag == "bola-danger"){
                    AdicionarBola();
                    AdicionarBola();
                    AdicionarBola();
                    _clickBola.Play();
                    Destroy(hit.transform.gameObject);
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
