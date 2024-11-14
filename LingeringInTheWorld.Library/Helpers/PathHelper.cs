namespace LingeringInTheWorld.Library.Helpers;


//文件名 -> 文件存储的位置
public static class PathHelper {
    private static string _localFolder = string.Empty;

    private static string LocalFolder {
        get
        {
            if (!string.IsNullOrEmpty(_localFolder)) {
                return _localFolder;
            }
            
            _localFolder =
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder
                        .LocalApplicationData), nameof(LingeringInTheWorld));

            //如果文件夹不存在，创建
            if (!Directory.Exists(_localFolder)) {
                Directory.CreateDirectory(_localFolder);
            }

            return _localFolder;
        }
    }

    public static string GetLocalFilePath(string fileName) {
        return Path.Combine(LocalFolder, fileName);
    }
}
