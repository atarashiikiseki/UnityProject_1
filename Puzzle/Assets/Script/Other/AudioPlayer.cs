using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//随机播放音乐的类，单例
public class AudioPlayer : MonoBehaviour
{
    //静态实例
    public static AudioPlayer instance;

    //音乐播放器
    public AudioSource musicPlayer;
    //可播放的音乐
    public List<AudioClip> audioClips;
    //前一个播放的音乐的序号
    private int preMusicNo = -1;
    //是否正在播放音乐
    public bool IsPlaying
    {
        get => musicPlayer.isPlaying;
    }
    //是否被暂停
    private bool isPaused = false; 

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    void Start()
    {
        musicPlayer.clip = RandomMusic();
        if (!isPaused)
            musicPlayer.Play();
    }
    //播放音乐
    public void Play()
    {
        musicPlayer.Play();
        isPaused = false;
    }
    //暂停音乐
    public void Pause()
    {
        musicPlayer.Pause();
        isPaused = true;
    }  
    //随机选择一首音乐
    public AudioClip RandomMusic()
    {
        int musicAmount = audioClips.Count;
        if (musicAmount == 0)
            return null;
        if (musicAmount == 1)
            return audioClips[0];
        //不连续播放相同的音乐
        int musicNo = Random.Range(0,musicAmount);
        while(musicNo == preMusicNo)
            musicNo = Random.Range(0, musicAmount);
        preMusicNo = musicNo;
        return audioClips[musicNo];
    }
    private void FixedUpdate()
    {     
        //音乐播放完成时，切换音乐
        if(!musicPlayer.isPlaying && !isPaused)
        {
            musicPlayer.clip = RandomMusic();
            musicPlayer.Play();
        }
    }    
}
