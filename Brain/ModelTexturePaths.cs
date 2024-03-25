namespace game_2.Brain
{
    public class ModelTexturePaths
    {
        public string _DiffusePath = string.Empty;
        public string _SpecularPath = string.Empty;
        public string _NormalPath = string.Empty;
        public string _HeightPath = string.Empty;
        public string _MetallicPath = string.Empty;
        public string _RoughnnesPath = string.Empty;
        public string _LightMap = string.Empty;
        public string _EmissivePath = string.Empty;
        public string _AmbientOcclusionPath = string.Empty;

        public ModelTexturePaths(
         string _DiffusePath,
         string _SpecularPath,
         string _NormalPath,
         string _HeightPath,
         string _MetallicPath,
         string _RoughnnesPath,
         string _LightMap,
         string _EmissivePath,
         string _AmbientOcclusionPath)
        {
            this._DiffusePath = _DiffusePath;
            this._SpecularPath = _SpecularPath;
            this._NormalPath = _NormalPath;
            this._HeightPath = _HeightPath;
            this._MetallicPath = _MetallicPath;
            this._RoughnnesPath = _RoughnnesPath;
            this._LightMap = _LightMap;
            this._EmissivePath = _EmissivePath;
            this._AmbientOcclusionPath = _AmbientOcclusionPath;
        }

        public ModelTexturePaths(ModelTexturePaths modelTexturePaths)
        {
            _DiffusePath = modelTexturePaths._DiffusePath;
            _SpecularPath = modelTexturePaths._SpecularPath;
            _NormalPath = modelTexturePaths._NormalPath;
            _HeightPath = modelTexturePaths._HeightPath;
            _MetallicPath = modelTexturePaths._MetallicPath;
            _RoughnnesPath = modelTexturePaths._RoughnnesPath;
            _LightMap = modelTexturePaths._LightMap;
            _EmissivePath = modelTexturePaths._EmissivePath;
            _AmbientOcclusionPath = modelTexturePaths._AmbientOcclusionPath;
        }

        public ModelTexturePaths()
        {

        }
    }
}
