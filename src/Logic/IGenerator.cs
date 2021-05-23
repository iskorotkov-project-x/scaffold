using Model;

namespace Business
{
    public interface IGenerator
    {
        Project Generate(string pathToProject, Template template);
    }
}
