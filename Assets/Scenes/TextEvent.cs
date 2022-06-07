using UnityEngine;
using UnityEngine.UI;

public class TextEvent : MonoBehaviour
{
  [SerializeField] private IntroLoopAudio IntroLoopAudio;

  private Text _text;

  // Start is called before the first frame update
  void Start()
  {
    _text = GetComponent<Text>();
  }

  // Update is called once per frame
  void Update()
  {
    _text.text = @$"AudioPlayTime : {IntroLoopAudio.time}";
  }
}
