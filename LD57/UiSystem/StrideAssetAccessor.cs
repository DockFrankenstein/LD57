using AssetManagementBase;
using Stride.Core.Serialization.Contents;

namespace LD57.UiSystem
{
    public class StrideAssetAccessor : IAssetAccessor
    {
        public StrideAssetAccessor(ContentManager content)
        {
            Content = content;
        }

        public string Name => "Myra Stride Asset Accessor";

        public ContentManager Content { get; set; }

        private static string ConvertMyraToStrideName(string myraName) =>
            myraName
            .Substring(1, myraName.Length - 1)
            .Split('.')
            .First();

        public bool Exists(string assetName) =>
            Content.Exists(ConvertMyraToStrideName(assetName));

        public Stream Open(string assetName)
        {
            var path = ConvertMyraToStrideName(assetName);
            var result = Content.OpenAsStream(path, Stride.Core.IO.StreamFlags.None);
            return result;
        }
    }
}
