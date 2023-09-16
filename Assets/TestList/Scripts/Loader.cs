using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class Loader : MonoBehaviour
{
    public static Loader instance;
    
    void Awake()
    {
        instance = this;
    }
    
    //public async UniTask<List<User>> LoadUsers(string url)
    //{
    //    UnityWebRequest request = await UnityWebRequest.Get(url)
    //        .SendWebRequest()
    //        .WithCancellation(this.GetCancellationTokenOnDestroy());

    //    if (request.result == UnityWebRequest.Result.ConnectionError)
    //    {
    //        Debug.Log($"ERROR {request.error}");
    //        return null;
    //    }

    //    string jsonText = request.downloadHandler.text;
        
    //    string jsonList = "{\"users\":" + jsonText + "}";

    //    ResponseLisrt response = JsonUtility.FromJson<ResponseLisrt>(jsonList);

    //    List<User> list = response.users.ToList();
    //    ListSorter.Sorting(list, 0, list.Count-1);

    //    request.Dispose();

    //    return list;
    //}

    public async UniTask<Texture2D> LoadAvatar(string url)
    {
        UnityWebRequest request = await UnityWebRequestTexture.GetTexture(url)
            .SendWebRequest()
            .WithCancellation(this.GetCancellationTokenOnDestroy());

        if(request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log($"ERROR {request.error}");
            return null;
        }

        Texture2D avatar = DownloadHandlerTexture.GetContent(request);
        
        request.Dispose();

        return avatar;
    }
}
