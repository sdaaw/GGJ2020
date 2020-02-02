using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    ///Play sound with same name as "name" parameter
    public static void PlayASource(string name)
    {
        foreach (SoundAsset s in Instance.sounds)
        {
            if (name == s.aName)
            {
                Instance.StartCoroutine("WaitAndDelete", s);
            }
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator WaitAndDelete(SoundAsset s)
    {
        GameObject sTemp = GameObject.Instantiate(s.aSource.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
        yield return new WaitForSeconds(s.aSource.clip.length + 1);
        Destroy(sTemp);
    }

    public List<SoundAsset> sounds = new List<SoundAsset>();

    [System.Serializable]
    public class SoundAsset
    {
        public AudioSource aSource;
        public string aName;
    }
}
