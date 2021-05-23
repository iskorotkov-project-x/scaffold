using Model;

namespace Business
{
    public interface ITemplater
    {
        Project Substitute(Context ctx, Project project);
    }
}
