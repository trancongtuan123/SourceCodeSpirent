using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ranorex.Core.Repository;

namespace RxAgent.Commands
{
    public abstract class RanorexItemHandler
    {
        public abstract RanorexStepExecutionResponse Execute(Dictionary<string, object> arguments);

        public RepoItemInfo CreateItemInfo(RepoGenBaseFolder repo, String target, int timeout)
        {
            return new RepoItemInfo(repo, "UI Element", target, timeout, null, "3529d992-a9f6-4df9-927a-3e7e1e08ca8b");
        }

        public Ranorex.Unknown CreateAdapter(RepoItemInfo itemInfo)
        {
            try
            {
                return itemInfo.CreateAdapter<Ranorex.Unknown>(true);
            }
            catch (Exception e)
            {
                throw new Exception("Unable to find element with path: " + itemInfo.AbsolutePath);
            }

        }

        public string GetImage64(Ranorex.Unknown adapter)
        {
            return adapter.CaptureCompressedImage().ToBase64String();
        }
    }
}
