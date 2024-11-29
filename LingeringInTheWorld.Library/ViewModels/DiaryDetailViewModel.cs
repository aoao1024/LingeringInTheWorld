using LingeringInTheWorld.Library.Models;

namespace LingeringInTheWorld.Library.ViewModels;

public class DiaryDetailViewModel : ViewModelBase
{
    public override void SetParameter(object parameter) {
        if (parameter is not Models.Diary diary) {
            return;
        }

        Diary = diary;
    }

    private Diary _diary;
    
    public Diary Diary {
        get => _diary;
        set => SetProperty(ref _diary, value);
    }
    
    // ImageBytesList 属性
    public List<byte[]> ImageBytesList => Diary?.ImageBytesList() ?? new List<byte[]>();
    
}