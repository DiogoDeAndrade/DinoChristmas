using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Karaoke : MonoBehaviour
{
    [SerializeField] TextAsset          lyricsFile;
    [SerializeField] AudioClip          song;
    [SerializeField] CanvasGroup        karokeDisplay;
    [SerializeField] TextMeshProUGUI    karaokeText;
    [SerializeField] bool               recordMode;

    class SongData
    {
        public float    startTime;
        public float    endTime;
        public string   lyrics;
    }
    
    private List<SongData> songData;
    private float currentSongTime;
    private int currentLyricsIndex;

    void Start()
    {
        var lines = lyricsFile.text.Split('\n', System.StringSplitOptions.None);

        songData = new List<SongData>();
        foreach (var line in lines)
        {
            if (line == "") continue;
            var data = line.Split(':');
            if (data.Length == 3)
            {
                songData.Add(new SongData()
                {
                    startTime = float.Parse(data[0]),
                    endTime = float.Parse(data[1]),
                    lyrics = CleanString(data[2])
                });
            }
            else
            {
                songData.Add(new SongData()
                {
                    startTime = float.MaxValue,
                    endTime = float.MaxValue,
                    lyrics = CleanString(line)
                });
            }
        }

        if (recordMode)
        {
            foreach (var sd in songData) sd.startTime = sd.endTime = float.MaxValue;
        }

        SoundManager.PlayMusic(song, 1.0f, 1.0f);
        currentSongTime = 0.0f;
        currentLyricsIndex = 0;
    }

    void Update()
    {
        currentSongTime += Time.deltaTime;

        if (songData.Count > currentLyricsIndex)
        {
            var sd = songData[currentLyricsIndex];

            if (recordMode)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (sd.startTime == float.MaxValue) sd.startTime = currentSongTime;
                    else if (sd.endTime == float.MaxValue)
                    {
                        sd.endTime = currentSongTime;
                        currentLyricsIndex++;
                        if (songData.Count > currentLyricsIndex)
                        {
                            sd = songData[currentLyricsIndex];
                            sd.startTime = currentSongTime;
                        }
                    }

                    SaveData();
                }
            }

            if ((sd.startTime <= currentSongTime) &&
                (sd.endTime >= currentSongTime))
            {
                karokeDisplay.alpha = 1.0f;
                karaokeText.text = sd.lyrics;
            }
            else
            {
                karokeDisplay.alpha = 0.0f;
                if (sd.endTime < currentSongTime)
                {
                    currentLyricsIndex++;
                }
            }
        }
        else
        {
            karokeDisplay.alpha = 0.0f;
        }
    }

    void SaveData()
    {
#if UNITY_EDITOR
        // Get the path of the TextAsset in the Assets folder
        string assetPath = AssetDatabase.GetAssetPath(lyricsFile);

        using (StreamWriter writer = new StreamWriter(assetPath))
        {
            foreach (var data in songData)
            {
                // Format each line as startTime:endTime:lyrics
                string line = $"{data.startTime}:{data.endTime}:{data.lyrics}";
                writer.WriteLine(line);
            }
        }
#endif
    }

    public string CleanString(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        // Remove all \r and \n characters
        string cleanedString = input.Replace("\r", "").Replace("\n", "");
        return cleanedString;
    }
}
