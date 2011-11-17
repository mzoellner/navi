using System.ComponentModel;

namespace Navi.KinectEngine
{
    public class AsyncStateData
    {
        public readonly AsyncOperation AsyncOperation;
        public volatile bool Canceled;
        public volatile bool Running = true;

        public AsyncStateData()
        {
            AsyncOperation = AsyncOperationManager.CreateOperation(null);
        }
    }
}
