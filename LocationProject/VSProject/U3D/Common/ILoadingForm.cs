
namespace Base.Common
{
    /// <summary>
    /// 等待界面的接口
    /// </summary>
    public interface ILoadingForm
    {
        /// <summary>
        /// 显示等待界面
        /// </summary>
        /// <param name="text"></param>
        /// <param name="showTime"></param>
        /// <param name="moveable"></param>
        /// <param name="parentForm"></param>
        void ShowLoading(string text, bool showTime, bool moveable,object parentForm);
        /// <summary>
        /// 关闭等待界面
        /// </summary>
        void CloseLoading();
    }
}
