using UnityEngine;

/// <summary>
/// �C���g���t�����[�v BGM �𐧌䂷��N���X�ł��B
/// </summary>
/// <remarks>
/// WebGL �ł� PlayScheduled �ōĐ�����ƃ��[�v���Ȃ��̂ł��̑Ή������Ă���B
/// WebGL �ł͂Q�� AudioSource �����݂ɍĐ��B
/// </remarks>
public class IntroLoopAudio : MonoBehaviour
{
  /// <summary>BGM �̃C���g�������̉����f�[�^�B</summary>
  [SerializeField] private AudioClip AudioClipIntro;

  /// <summary>BGM �̃��[�v�����̉����f�[�^�B</summary>
  [SerializeField] private AudioClip AudioClipLoop;

  /// <summary>BGM �̃C���g�������� AudioSource�B</summary>
  private AudioSource _introAudioSource;

  /// <summary>BGM �̃��[�v������ AudioSource�B</summary>
  private AudioSource[] _loopAudioSources = new AudioSource[2];

  /// <summary>�ꎞ��~�����ǂ����B</summary>
  private bool _isPause;

  /// <summary>���݂̍Đ����郋�[�v�����̃C���f�b�N�X�B</summary>
  private int _nowPlayIndex = 0;

  /// <summary>���[�v�����Ɏg�p���� AudioSource �̐��B</summary>
  private int _loopSourceCount = 0;

  /// <summary>�Đ����ł��邩�ǂ����B�ꎞ��~�A��A�N�e�B�u�̏ꍇ�� false ��Ԃ��B</summary>
  private bool IsPlaying
    => (_introAudioSource.isPlaying || _introAudioSource.time > 0)
      || (_loopAudioSources[0].isPlaying || _loopAudioSources[0].time > 0)
      || (_loopAudioSources[1] != null && (_loopAudioSources[1].isPlaying || _loopAudioSources[1].time > 0));

  /// <summary>���݃A�N�e�B�u�ōĐ����Ă��郋�[�v���� AudioSource�B</summary>
  private AudioSource LoopAudioSourceActive
    => _loopAudioSources[1] != null && _loopAudioSources[1].time > 0 ? _loopAudioSources[1] : _loopAudioSources[0];

  /// <summary>���݂̍Đ����� (s)�B</summary>
  public float time
    => _introAudioSource == null ? 0
      : _introAudioSource.time > 0 ? _introAudioSource.time
      : LoopAudioSourceActive.time > 0 ? AudioClipIntro.length + LoopAudioSourceActive.time
      : 0;


  void Start()
  {
    _loopSourceCount = 2;   // WebGL �łȂ���� 1 �ł��悢

    // AudioSource �����g�ɒǉ�
    _introAudioSource = gameObject.AddComponent<AudioSource>();
    _loopAudioSources[0] = gameObject.AddComponent<AudioSource>();
    if (_loopSourceCount >= 2)
    {
      _loopAudioSources[1] = gameObject.AddComponent<AudioSource>();
    }

    _introAudioSource.clip = AudioClipIntro;
    _introAudioSource.loop = false;
    _introAudioSource.playOnAwake = false;

    _loopAudioSources[0].clip = AudioClipLoop;
    _loopAudioSources[0].loop = _loopSourceCount == 1;
    _loopAudioSources[0].playOnAwake = false;
    if (_loopAudioSources[1] != null)
    {
      _loopAudioSources[1].clip = AudioClipLoop;
      _loopAudioSources[1].loop = false;
      _loopAudioSources[1].playOnAwake = false;
    }
  }

  void Update()
  {
    // WebGL �̂��߂̃��[�v�؂�ւ�����
    if (_loopSourceCount >= 2)
    {
      // �I������P�b�O���玟�̍Đ��̃X�P�W���[����o�^����
      if (_nowPlayIndex == 0 && _loopAudioSources[0].time >= AudioClipLoop.length - 1)
      {
        _loopAudioSources[1].PlayScheduled(AudioSettings.dspTime + (AudioClipLoop.length - _loopAudioSources[0].time));
        _nowPlayIndex = 1;
      }
      else if (_nowPlayIndex == 1 && _loopAudioSources[1].time >= AudioClipLoop.length - 1)
      {
        _loopAudioSources[0].PlayScheduled(AudioSettings.dspTime + (AudioClipLoop.length - _loopAudioSources[1].time));
        _nowPlayIndex = 0;
      }
    }
  }

  public void Play()
  {
    // �N���b�v���ݒ肳��Ă��Ȃ��ꍇ�͉������Ȃ�
    if (_introAudioSource == null || _loopAudioSources == null) return;

    // Pause ���� isPlaying �� false
    // �W���@�\�����ł͈ꎞ��~�������ʕs�\
    if (_isPause)
    {
      _introAudioSource.UnPause();
      if (_introAudioSource.isPlaying)
      {
        // �C���g�����Ȃ烋�[�v�J�n���Ԃ��c�莞�ԂōĐݒ�
        _loopAudioSources[0].Stop();
        _loopAudioSources[0].PlayScheduled(AudioSettings.dspTime + AudioClipIntro.length - _introAudioSource.time);
      }
      else
      {
        if (_loopSourceCount >= 2)
        {
          // WebGL �̏ꍇ�͐؂�ւ����������s
          if (_loopAudioSources[0].time > 0)
          {
            _loopAudioSources[0].UnPause();
            if (_loopAudioSources[0].time >= AudioClipLoop.length - 1)
            {
              _loopAudioSources[1].Stop();
              _loopAudioSources[1].PlayScheduled(AudioSettings.dspTime + (AudioClipLoop.length - _loopAudioSources[0].time));
              _nowPlayIndex = 1;
            }
          }
          else
          {
            _loopAudioSources[1].UnPause();
            if (_loopAudioSources[1].time >= AudioClipLoop.length - 1)
            {
              _loopAudioSources[0].Stop();
              _loopAudioSources[0].PlayScheduled(AudioSettings.dspTime + (AudioClipLoop.length - _loopAudioSources[0].time));
              _nowPlayIndex = 0;
            }
          }
        }
        else
        {
          // WebGL �ȊO�� UnPause ���邾��
          _loopAudioSources[0].UnPause();
        }
      }
    }
    else if (IsPlaying == false)
    {
      // �ŏ�����Đ�
      Stop();
      _introAudioSource.Play();

      // �C���g���̎��Ԃ��o�߂�����ɍĐ��ł���悤�ɂ���
      // �ݒ肷�鎞�Ԃ̓Q�[���Y�����Ԃł̐ݒ�ƂȂ�
      _loopAudioSources[0].PlayScheduled(AudioSettings.dspTime + AudioClipIntro.length);
    }

    _isPause = false;
  }

  /// <summary>BGM ���ꎞ��~���܂��B</summary>
  public void Pause()
  {
    if (_introAudioSource == null || _loopAudioSources == null) return;

    _introAudioSource.Pause();
    _loopAudioSources[0].Pause();
    if (_loopAudioSources[1] != null) _loopAudioSources[1].Pause();

    _isPause = true;
  }

  /// <summary>BGM ���~���܂��B</summary>
  public void Stop()
  {
    if (_introAudioSource == null || _loopAudioSources == null) return;

    _introAudioSource.Stop();
    _loopAudioSources[0].Stop();
    if (_loopAudioSources[1] != null) _loopAudioSources[1].Stop();

    _isPause = false;
  }
}
