using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
#if ENABLE_WINMD_SUPPORT
using Windows.Storage;
using Windows.Foundation;
#endif

namespace TestProject
{
    public class TestKnownFolders : MonoBehaviour
    {
        [SerializeField] private GameObject dialogPrefab;

#if ENABLE_WINMD_SUPPORT
        private async void Start()
        {
            var fileName = "test.txt";
            var contents = "TestTestTest";
            var title = string.Empty;
            var message = string.Empty;
            if (await KnownFolders.Objects3D.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting).AsTask() is { } file)
            {
                await FileIO.WriteTextAsync(file, contents).AsTask();
                var path = Path.Combine(KnownFolders.Objects3D.Path, fileName);
                if (File.Exists(path))
                {
                    var c = File.ReadAllText(path);
                    title = "Success";
                    message = $"file contents: {c}";
                }
                else
                {
                    title = "Failed";
                    message = $"can't access with File.Exists: {fileName}";
                }
            }
            else
            {
                title = "Failed";
                message = "$can't create file: {fileName}";
            }

            Debug.Log(message);
            Dialog.Open(dialogPrefab, DialogButtonType.Close, title, message, true);
        }
    }

    public static class IAsyncExtension
    {
        public static Task<T> AsTask<T>(this IAsyncOperation<T> operation)
        {
            var tcs = new TaskCompletionSource<T>();
            operation.Completed = delegate
            {
                switch (operation.Status)
                {
                    case AsyncStatus.Completed:
                        tcs.SetResult(operation.GetResults());
                        break;
                    case AsyncStatus.Error:
                        tcs.SetException(operation.ErrorCode);
                        break;
                    case AsyncStatus.Canceled:
                        tcs.SetCanceled();
                        break;
                }
            };
            return tcs.Task;
        }

        public static Task AsTask(this IAsyncAction action)
        {
            var tcs = new TaskCompletionSource<bool>();
            action.Completed = delegate
            {
                switch (action.Status)
                {
                    case AsyncStatus.Completed:
                        tcs.SetResult(true);
                        break;
                    case AsyncStatus.Error:
                        tcs.SetException(action.ErrorCode);
                        break;
                    case AsyncStatus.Canceled:
                        tcs.SetCanceled();
                        break;
                }
            };
            return tcs.Task;
        }
#endif
    }
}