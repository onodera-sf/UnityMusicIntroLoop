using UnityEngine;

public class ButtonEvent : MonoBehaviour
{
  [SerializeField] private IntroLoopAudio IntroLoopAudio;

  public void OnClickPlay()
  {
    IntroLoopAudio.Play();
  }
  public void OnClickPause()
  {
    IntroLoopAudio.Pause();
  }
  public void OnClickStop()
  {
    IntroLoopAudio.Stop();
  }
}
