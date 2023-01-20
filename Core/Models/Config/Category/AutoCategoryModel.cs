namespace Core.Models.Config.Category
{
    public class AutoCategoryModel
    {
        // [Config(Name = "匹配规则", Placeholder = "匹配规则，输入文件夹路径或正则表达式")]
        public string RegexRule;

        // [Config(Name = "目标分类", Placeholder = "进程名称，不需要输入.exe。支持正则表达式")]
        public int CategoryID;

        public string CategoryName;

    }
}