namespace UI.Servicers
{
    public interface ILocalizationServicer
    {
        string Translated(string key);
        string Translated(string key, params object[] args);
        string Translated(params object[] args);
    }
}