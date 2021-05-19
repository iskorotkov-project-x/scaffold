using Model;

namespace Business
{
    public interface ILoader
    {
        Template Load(string language, string template);
    }
}
